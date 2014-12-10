#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    // Lazy<T> is generic, but not all of its state needs to be generic.  Avoid creating duplicate
    // objects per instantiation by putting them here.
    internal static class ResettableLazyHelpers
    {
        // Dummy object used as the value of _threadSafeObj if in PublicationOnly mode.
        [NotNull]
        internal static readonly object PublicationOnlySentinel = new object();
    }

    /// <summary>
    /// Provides support for lazy initialization.
    /// </summary>
    /// <typeparam name="T">Specifies the type of element being lazily initialized.</typeparam>
    /// <remarks>
    /// <para>
    /// By default, all public and protected members of <see cref="ResettableLazy{T}"/> are thread-safe and may be used
    /// concurrently from multiple threads.  These thread-safety guarantees may be removed optionally and per instance
    /// using parameters to the type's constructors.
    /// </para>
    /// <para>This code is based off the source code for the <see cref="Lazy{T}"/> class.</para>
    /// </remarks>
    [Serializable]
    [ComVisible(false)]
    [DebuggerDisplay("ThreadSafetyMode={Mode}, IsValueCreated={IsValueCreated}, IsValueFaulted={IsValueFaulted}, Value={ValueForDebugDisplay}")]
    public class ResettableLazy<T>
    {
        #region Inner classes
        /// <summary>
        /// wrapper class to box the initialized value, this is mainly created to avoid boxing/unboxing the value each time the value is called in case T is 
        /// a value type
        /// </summary>
        [Serializable]
        private class Boxed
        {
            internal readonly T Value;

            internal Boxed(T value)
            {
                Value = value;
            }
        }
        #endregion

        private static readonly Func<T> _ctorFunc;

        /// <summary>
        /// Initializes the <see cref="ResettableLazy{T}"/> class.
        /// </summary>
        static ResettableLazy()
        {
            ConstructorInfo ctor = typeof(T).GetConstructor(Array<Type>.Empty);

            if (ctor != null)
            {
                _ctorFunc = ctor.Func<T>();
                return;
            }

            if (typeof(T).IsValueType)
                _ctorFunc = Expression.Lambda<Func<T>>(Expression.Constant(default(T))).Compile();
        }

        /// <summary>
        /// Gets the function for the default constructor for the type <typeparamref name="T"/>.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">The lazily-initialized type does not have a public, parameterless constructor.</exception>
        [NotNull]
        private static Func<T> GetCtorFunc()
        {
            if (_ctorFunc == null)
                // TODO Translate
                throw new ArgumentException("The lazily-initialized type does not have a public, parameterless constructor.");
            return _ctorFunc;
        }

        /// <summary>
        /// The boxed value of the lazy.
        /// </summary>
        /// <remarks>
        /// <para>If <see cref="_boxed"/> == <see langword="null"/>, the value is not yet created.</para>
        /// <para>If <see cref="_boxed"/> is <see cref="Boxed"/>, the value is created.</para>
        /// <para>If <see cref="_boxed"/> is <see cref="ExceptionDispatchInfo"/>, the value is an exception.</para>
        /// </remarks>
        private object _boxed;

        /// <summary>
        /// The factory delegate that returns the value.
        /// </summary>
        [NonSerialized]
        [NotNull]
        private readonly Func<T> _valueFactory;

        /// <summary>
        /// In None and ExecutionAndPublication modes, this will be set to <see langword="true"/> to avoid recursive calls.
        /// </summary>
        private bool _initialized;

        /// <summary>
        /// The object used for thread safety. Will be <see langword="null"/> if not thread safe, <see cref="ResettableLazyHelpers.PublicationOnlySentinel"/> if PublicationOnly mode, or an object if ExecutionAndPublication mode.
        /// </summary>
        [NonSerialized]
        private readonly object _threadSafeObj;

        /// <summary>
        /// Whether the value should be created when <see cref="ToString"/> is called.
        /// </summary>
        [NonSerialized]
        private readonly bool _createOnToString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResettableLazy{T}"/> class that 
        /// uses <typeparamref name="T"/>'s default constructor for lazy initialization.
        /// </summary>
        /// <remarks>
        /// An instance created with this constructor may be used concurrently from multiple threads.
        /// </remarks>
        public ResettableLazy()
            : this(GetCtorFunc())
        {
        }

        /// <summary>
        /// Initializes a new instance of the  <see cref="ResettableLazy{T}" /> class that uses 
        /// <typeparamref name="T" />'s default constructor for lazy initialization.
        /// </summary>
        /// <param name="createOnToString">if set to <see langword="true" /> the value will be created when <see cref="ToString"/> is called.</param>
        /// <remarks>
        /// An instance created with this constructor may be used concurrently from multiple threads.
        /// </remarks>
        public ResettableLazy(bool createOnToString)
            : this(GetCtorFunc(), LazyThreadSafetyMode.ExecutionAndPublication, createOnToString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResettableLazy{T}" /> class that uses 
        /// <typeparamref name="T" />'s default constructor and a specified thread-safety mode.
        /// </summary>
        /// <param name="isThreadSafe">true if this instance should be usable by multiple threads concurrently; false if the instance will only be used by one thread at a time.</param>
        /// <param name="createOnToString">if set to <see langword="true" /> the value will be created when <see cref="ToString"/> is called.</param>
        public ResettableLazy(bool isThreadSafe, bool createOnToString = false)
            : this(GetCtorFunc(), isThreadSafe ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None,
                createOnToString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResettableLazy{T}" /> class that uses 
        /// <typeparamref name="T" />'s default constructor and a specified thread-safety mode.
        /// </summary>
        /// <param name="mode">The lazy thread-safety mode mode</param>
        /// <param name="createOnToString">if set to <see langword="true" /> the value will be created when <see cref="ToString" /> is called.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="mode" /> mode contains an invalid valuee</exception>
        public ResettableLazy(LazyThreadSafetyMode mode, bool createOnToString = false)
            : this(GetCtorFunc(), mode, createOnToString)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ResettableLazy{T}" /> class
        /// that uses a specified initialization function and a specified thread-safety mode.
        /// </summary>
        /// <param name="valueFactory">The <see cref="T:System.Func{T}" /> invoked to produce the lazily-initialized value when it is needed.</param>
        /// <param name="isThreadSafe">true if this instance should be usable by multiple threads concurrently; false if the instance will only be used by one thread at a time.</param>
        /// <param name="createOnToString">if set to <see langword="true" /> the value will be created when <see cref="ToString"/> is called.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="valueFactory" /> is a null reference (Nothing in Visual Basic).</exception>
        public ResettableLazy([NotNull] Func<T> valueFactory, bool isThreadSafe, bool createOnToString = false)
            : this(
                valueFactory,
                isThreadSafe ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None,
                createOnToString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResettableLazy{T}" /> class
        /// that uses a specified initialization function and a specified thread-safety mode.
        /// </summary>
        /// <param name="valueFactory">The <see cref="T:System.Func{T}" /> invoked to produce the lazily-initialized value when it is needed.</param>
        /// <param name="mode">The lazy thread-safety mode.</param>
        /// <param name="createOnToString">if set to <see langword="true" /> the value will be created when <see cref="ToString"/> is called.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="valueFactory" /> is
        /// a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="mode" /> mode contains an invalid value.</exception>
        public ResettableLazy(
            [NotNull] Func<T> valueFactory,
            LazyThreadSafetyMode mode = LazyThreadSafetyMode.ExecutionAndPublication,
            bool createOnToString = false)
        {
            Contract.Requires<ArgumentNullException>(valueFactory != null, "valueFactory");

            _threadSafeObj = GetObjectFromMode(mode);
            _valueFactory = valueFactory;
            _createOnToString = createOnToString;
        }

        /// <summary>
        /// Static helper function that returns an object based on the given mode. it also throws an exception if the mode is invalid
        /// </summary>
        [CanBeNull]
        private static object GetObjectFromMode(LazyThreadSafetyMode mode)
        {
            if (mode == LazyThreadSafetyMode.ExecutionAndPublication)
                return new object();
            if (mode == LazyThreadSafetyMode.PublicationOnly)
                return ResettableLazyHelpers.PublicationOnlySentinel;
            if (mode != LazyThreadSafetyMode.None)
                throw new ArgumentOutOfRangeException("mode");

            return null; // None mode
        }

        /// <summary>
        /// Forces initialization during serialization.
        /// </summary>
        /// <param name="context">The StreamingContext for the serialization operation.</param>
        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            // Force initialization
            // ReSharper disable once UnusedVariable
            T dummy = Value;
        }

        /// <summary>
        /// Creates and returns a string representation of this instance.
        /// </summary>
        /// <returns>
        /// The result of calling <see cref="System.Object.ToString" /> on the <see cref="Value" />.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <see cref="Value" /> is null.</exception>
        public override string ToString()
        {
            if (_createOnToString)
            {
                T value = Value;

                return ReferenceEquals(value, null) ? string.Empty : value.ToString();
            }

            object boxed = _boxed;

            Boxed b = boxed as Boxed;
            if (b != null)
                return ReferenceEquals(b.Value, null) ? string.Empty : b.Value.ToString();

            ExceptionDispatchInfo edi = boxed as ExceptionDispatchInfo;
            if (edi != null)
            {
                edi.Throw();
                // TODO Translate
                return "Value has exception";
            }

            // TODO Translate
            return "Value is not created.";
        }

        /// <summary>
        /// Gets the value of the Lazy&lt;T&gt; for debugging display purposes.
        /// </summary>
        /// <value>
        /// The value if created; otherwise <see langword="default{T}"/>.
        /// </value>
        [UsedImplicitly]
        internal T ValueForDebugDisplay
        {
            get
            {
                Boxed boxed = _boxed as Boxed;
                if (boxed == null)
                    return default(T);

                return boxed.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance may be used concurrently from multiple threads.
        /// </summary>
        [PublicAPI]
        internal LazyThreadSafetyMode Mode
        {
            get
            {
                if (_threadSafeObj == null) return LazyThreadSafetyMode.None;
                if (_threadSafeObj == ResettableLazyHelpers.PublicationOnlySentinel)
                    return LazyThreadSafetyMode.PublicationOnly;
                return LazyThreadSafetyMode.ExecutionAndPublication;
            }
        }

        /// <summary>
        /// Gets whether the value creation is faulted or not
        /// </summary>
        [PublicAPI]
        internal bool IsValueFaulted
        {
            get { return _boxed is ExceptionDispatchInfo; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ResettableLazy{T}" /> has been initialized.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the <see cref="ResettableLazy{T}" /> instance has been initialized;
        /// otherwise, <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// The initialization of a 
        /// <see cref="ResettableLazy{T}" /> instance may result in either
        /// a value being produced or an exception being thrown.  If an exception goes unhandled during initialization,
        /// <see cref="IsValueCreated" /> will return <see langword="false"/>.
        /// </remarks>
        [PublicAPI]
        public bool IsValueCreated
        {
            get
            {
                return _boxed is Boxed;
            }
        }

        /// <summary>Gets the lazily initialized value of the current <see cref="ResettableLazy{T}"/>.</summary>
        /// <value>The lazily initialized value of the current <see cref="ResettableLazy{T}"/>.</value>
        /// <exception cref="T:System.MissingMemberException">
        /// The <see cref="ResettableLazy{T}"/> was initialized to use the default constructor 
        /// of the type being lazily initialized, and that type does not have a public, parameterless constructor.
        /// </exception>
        /// <exception cref="T:System.MemberAccessException">
        /// The <see cref="ResettableLazy{T}"/> was initialized to use the default constructor 
        /// of the type being lazily initialized, and permissions to access the constructor were missing.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// The <see cref="ResettableLazy{T}"/> was constructed with the <see cref="LazyThreadSafetyMode.ExecutionAndPublication"/> or
        /// <see cref="LazyThreadSafetyMode.None"/>  and the initialization function attempted to access <see cref="Value"/> on this instance.
        /// </exception>
        /// <remarks>
        /// If <see cref="IsValueCreated"/> is false, accessing <see cref="Value"/> will force initialization.
        /// Please <see cref="LazyThreadSafetyMode"/> for more information on how <see cref="ResettableLazy{T}"/> will behave if an exception is thrown
        /// from initialization delegate.
        /// </remarks>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [PublicAPI]
        public T Value
        {
            get
            {
                if (_boxed != null)
                {
                    // Do a quick check up front for the fast path.
                    Boxed boxed = _boxed as Boxed;
                    if (boxed != null)
                        return boxed.Value;

                    ExceptionDispatchInfo exc = _boxed as ExceptionDispatchInfo;
                    Contract.Assert(exc != null);
                    exc.Throw();
                }

                // Fall through to the slow path.
                return LazyInitValue();
            }
        }

        /// <summary>
        /// local helper method to initialize the value
        /// </summary>
        /// <returns>
        /// The initialized value
        /// </returns>
        private T LazyInitValue()
        {
            Boxed boxed;
            LazyThreadSafetyMode mode = Mode;
            if (mode == LazyThreadSafetyMode.None)
            {
                boxed = CreateValue();
                _boxed = boxed;
            }
            else if (mode == LazyThreadSafetyMode.PublicationOnly)
            {
                boxed = CreateValue();
                if (boxed == null ||
                    Interlocked.CompareExchange(ref _boxed, boxed, null) != null)
                {
                    // If CreateValue returns null, it means another thread successfully invoked the value factory
                    // and stored the result, so we should just take what was stored.  If CreateValue returns non-null
                    // but we lose the ---- to store the single value, again we should just take what was stored.
                    boxed = (Boxed)_boxed;
                }
                else
                {
                    _initialized = true;
                }
            }
            else
            {
                Contract.Assert(_threadSafeObj != null);
                lock (_threadSafeObj)
                {
                    if (_boxed == null)
                    {
                        boxed = CreateValue();
                        _boxed = boxed;
                    }
                    else
                    // got the lock but the value is not null anymore, check if it is created by another thread or faulted and throw if so
                    {
                        boxed = _boxed as Boxed;
                        if (boxed == null) // it is not Boxed, so it is a ExceptionDispatchInfo
                        {
                            ExceptionDispatchInfo exHolder = _boxed as ExceptionDispatchInfo;
                            Contract.Assert(exHolder != null);
                            exHolder.Throw();
                        }
                    }
                }
            }
            Contract.Assert(boxed != null);
            return boxed.Value;
        }

        /// <summary>
        /// Creates an instance of T using <see cref="_valueFactory" />.
        /// </summary>
        /// <returns>
        /// An instance of Boxed.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">ValueFactory attempted to access the Value property of this instance.</exception>
        [CanBeNull]
        private Boxed CreateValue()
        {
            Boxed boxed;
            LazyThreadSafetyMode mode = Mode;

            try
            {
                // check for recursion
                if (mode != LazyThreadSafetyMode.PublicationOnly && _initialized)
                {
                    // TODO Translate
                    throw new InvalidOperationException("ValueFactory attempted to access the Value property of this instance.");
                }

                Func<T> factory = _valueFactory;
                if (mode != LazyThreadSafetyMode.PublicationOnly)
                {
                    _initialized = true;
                }
                else if (_initialized)
                {
                    // Another thread beat us to successfully invoke the factory.
                    return null;
                }
                boxed = new Boxed(factory());
            }
            catch (Exception ex)
            {
                if (mode != LazyThreadSafetyMode.PublicationOnly)
                    // don't cache the exception for PublicationOnly mode
                    _boxed = ExceptionDispatchInfo.Capture(ex);
                throw;
            }

            return boxed;
        }

        /// <summary>
        /// Resets the value of this <see cref="ResettableLazy{T}"/> to not created.
        /// </summary>
        [PublicAPI]
        public void Reset()
        {
            LazyThreadSafetyMode mode = Mode;
            if (mode == LazyThreadSafetyMode.ExecutionAndPublication)
            {
                Contract.Assert(_threadSafeObj != null);
                lock (_threadSafeObj)
                {
                    _boxed = null;
                    _initialized = false;
                }
            }
            else
            {
                _boxed = null;
                _initialized = false;
            }
        }
    }
}