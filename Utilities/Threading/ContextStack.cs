#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: ContextStack.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    ///   Holds a stack of objects against a logical call stack on a thread.
    /// </summary>
    /// <remarks>
    ///   <para>Although it can take any object type, it is never safe to modify objects that exist on the stack,
    ///   as they are shared across many threads. Only the stack structure itself is thread-safe. When used with value
    ///   types it is entirely safe.</para>
    ///   <para>Unlike previous systems, this only stores <see cref="string"/>s against the thread call context, the
    ///   data of which is merely an index into a lookup dictionary. This means that it will always serialize (for
    ///   example when passing across an app domain) without any issues.</para>
    ///   <para>This works because it stores a string against the thread context, which is a value type, as such
    ///   when a new thread is spun up, the string value is copied to the new thread - acting as a snapshot of the
    ///   thread state at the point the new thread was created.</para>
    /// </remarks>
    /// <typeparam name="T">The type of objects in the stack.</typeparam>
    [UsedImplicitly]
    public class ContextStack<T>
    {
        /// <summary>
        ///   This holds a unique random key (<see cref="System.Guid"/>), that makes it impossible to guess
        ///   where the object is stored in the <see cref="System.Runtime.Remoting.Messaging.CallContext">call
        ///   context</see>, so the only access point is this instance.
        /// </summary>
        [NotNull] private readonly string _contextKey = Guid.NewGuid().ToString();

        /// <summary>
        ///   Holds all active objects by their index.
        /// </summary>
        [NotNull] private readonly ConcurrentDictionary<long, T> _objects = new ConcurrentDictionary<long, T>();

        /// <summary>
        ///   Caches the current stack against the Thread Local Storage
        ///   (http://msdn.microsoft.com/en-us/library/windows/desktop/ms686749).
        /// </summary>
        private readonly ThreadLocal<KeyValuePair<string, IEnumerable<T>>> _stackCache =
            new ThreadLocal<KeyValuePair<string, IEnumerable<T>>>();

        /// <summary>
        ///   The counter, which is used to generate unique indices for objects (more memory efficient and
        ///   quicker than GUIDs, but requires more thought to prevent collision).
        /// </summary>
        private long _counter;

        /// <summary>
        ///   Gets the current stack.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<T> CurrentStack
        {
            get
            {
                // Get the stack of long keys.
                string stack = CallContext.LogicalGetData(_contextKey) as string;
                if (string.IsNullOrEmpty(stack))
                    return Enumerable.Empty<T>();

                // Try to get the stack out of the current TLS.
                KeyValuePair<string, IEnumerable<T>> kvp = _stackCache.Value;
                if (kvp.Key == stack)
                    return kvp.Value;

                // Look up actual objects.
                string[] objectKeys = stack.Split('|');
                List<T> objects = new List<T>(objectKeys.Length);
                foreach (string objectKey in objectKeys)
                {
                    long key;
                    if (!long.TryParse(objectKey, out key))
                        continue;
                    T value;
                    if (!_objects.TryGetValue(key, out value))
                        continue;
                    objects.Add(value);
                }

                // Cache the stack against this thread.
                _stackCache.Value = new KeyValuePair<string, IEnumerable<T>>(stack, objects);

                return objects;
            }
        }

        /// <summary>
        ///   Gets the current top of the stack.
        /// </summary>
        [CanBeNull]
        [UsedImplicitly]
        public T Current
        {
            get { return CurrentStack.LastOrDefault(); }
        }

        /// <summary>
        ///   Adds the entry to the top of the stack.
        /// </summary>
        /// <param name="entry">The entry.</param>
        [NotNull]
        [UsedImplicitly]
        public IDisposable Region(T entry)
        {
            return new Disposer(this, entry);
        }

        /// <summary>
        ///   Creates a region in which the stack does not exist.
        ///   This is useful for security when passing off to a set of untrusted code.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public IDisposable Clean()
        {
            return new Cleaner(this);
        }

        #region Nested type: Cleaner
        /// <summary>
        ///   Used to remove stack from the logical call context for a period.
        /// </summary>
        private class Cleaner : IDisposable
        {
            /// <summary>
            ///   The stack value before we started.
            /// </summary>
            private readonly string _oldStack;

            /// <summary>
            ///   The owner stack.
            /// </summary>
            private readonly ContextStack<T> _stack;

            /// <summary>
            ///   The managed thread id.
            /// </summary>
            private readonly int _threadId;

            /// <summary>
            ///   The value held in the TLS.
            /// </summary>
            private readonly KeyValuePair<string, IEnumerable<T>> _tls;

            /// <summary>
            ///   Initializes a new instance of the <see cref="object"/> class.
            /// </summary>
            public Cleaner([NotNull] ContextStack<T> stack)
            {
                _stack = stack;
                _threadId = Thread.CurrentThread.ManagedThreadId;

                _oldStack = CallContext.LogicalGetData(_stack._contextKey) as string;
                if (_oldStack != null)
                    CallContext.FreeNamedDataSlot(_stack._contextKey);
                _tls = _stack._stackCache.Value;

                // Clear the stack cache.
                _stack._stackCache.Value = default(KeyValuePair<string, IEnumerable<T>>);
            }

            #region IDisposable Members
            /// <summary>
            ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            /// <exception cref="InvalidOperationException">
            ///   Cannot close the cleaner region as it was created on another thread.
            /// </exception>
            public void Dispose()
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                if (threadId != _threadId)
                    throw new InvalidOperationException(
                        string.Format(
                            Resources.ContextStack_Dispose_CannotCloseCleanerRegion,
                            _threadId, threadId));

                if (_oldStack != null)
                    CallContext.LogicalSetData(_stack._contextKey, _oldStack);

                // Restore the stack cache.
                _stack._stackCache.Value = _tls;
            }
            #endregion
        }
        #endregion

        #region Nested type: Disposer
        /// <summary>
        ///   Used to start and end a region.
        /// </summary>
        private class Disposer : IDisposable
        {
            /// <summary>
            ///   The key.
            /// </summary>
            private readonly long _key;

            /// <summary>
            ///   The stack value before we started.
            /// </summary>
            private readonly string _oldStack;

            /// <summary>
            ///   The owner stack.
            /// </summary>
            private readonly ContextStack<T> _stack;

            /// <summary>
            ///   The managed thread id.
            /// </summary>
            private readonly int _threadId;

            /// <summary>
            ///   Initializes a new instance of the <see cref="ContextStack&lt;T&gt;.Disposer"/> class.
            /// </summary>
            /// <param name="stack">The stack.</param>
            /// <param name="value">The value.</param>
            public Disposer([NotNull] ContextStack<T> stack, T value)
            {
                _stack = stack;
                _threadId = Thread.CurrentThread.ManagedThreadId;
                // We use Interlocked.Increment, as it's thread safe and never overflows (wraps around).
                // by the time it wraps back to itself it's highly likely that the objects are no longer in use.
                // However, the while loop ensures we get a nice blank spot.
                T used;
                do
                {
                    _key = Interlocked.Increment(ref stack._counter);
                } while (stack._objects.TryGetValue(_key, out used));

                stack._objects.AddOrUpdate(_key, value, (k, v) => value);

                _oldStack = CallContext.LogicalGetData(stack._contextKey) as string;
                CallContext.LogicalSetData(stack._contextKey,
                                           (_oldStack == null ? string.Empty : _oldStack + "|") + _key);
            }

            #region IDisposable Members
            /// <summary>
            ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            /// <exception cref="InvalidOperationException">
            ///   Cannot close the region as it was created on another thread.
            /// </exception>
            public void Dispose()
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;

                // Try to clear the object from the stack.
                T value;
                _stack._objects.TryRemove(_key, out value);

                if (_oldStack == null)
                    CallContext.FreeNamedDataSlot(_stack._contextKey);
                else
                    CallContext.LogicalSetData(_stack._contextKey, _oldStack);

                // Clear the stack cache.
                _stack._stackCache.Value = default(KeyValuePair<string, IEnumerable<T>>);
            }
            #endregion
        }
        #endregion
    }
}