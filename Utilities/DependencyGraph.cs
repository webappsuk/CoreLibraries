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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Represents graph of dependencies between objects.
    /// </summary>
    /// <typeparam name="T">The type of the object in the graph.</typeparam>
    /// <threadsafety static="true" instance="false" />
    [PublicAPI]
    public class DependencyGraph<T>
    {
        /// <summary>
        /// Contains objects and the objects that depend on them.
        /// </summary>
        [NotNull]
        private readonly Dictionary<T, HashSet<T>> _dependencies;

        /// <summary>
        /// Contains objects and the objects they depend on.
        /// </summary>
        [NotNull]
        private readonly Dictionary<T, HashSet<T>> _dependsOn;

        /// <summary>
        /// Contains all the objects in the graph.
        /// </summary>
        [NotNull]
        [ContractPublicPropertyName("All")]
        private readonly HashSet<T> _all;

        /// <summary>
        /// The comparer to use when comparing objects in the graph.
        /// </summary>
        private readonly IEqualityComparer<T> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyGraph{T}"/> class, using the default <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        public DependencyGraph()
            : this(null)
        {
        }

        /// <summary>
        /// Gets all the objects in the graph, unordered.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<T> All
        {
            get { return _all; }
        }

        /// <summary>
        /// Gets all the objects in the graph, top down.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<T> AllTopDown
        {
            get { return GetAllIterator(_dependencies, TopLeaves); }
        }

        /// <summary>
        /// Gets all the objects in the graph, bottom up.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<T> AllBottomUp
        {
            get { return GetAllIterator(_dependsOn, BottomLeaves); }
        }

        /// <summary>
        /// Gets the number of objects in the graph.
        /// </summary>
        /// <value>
        /// The number of objects in the graph.
        /// </value>
        [PublicAPI]
        public int Count
        {
            get { return _all.Count; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyGraph{T}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use when comparing objects in the graph.</param>
        public DependencyGraph([CanBeNull] IEqualityComparer<T> comparer)
        {
            _comparer = comparer ?? EqualityComparer<T>.Default;
            _dependencies = new Dictionary<T, HashSet<T>>(_comparer);
            _dependsOn = new Dictionary<T, HashSet<T>>(_comparer);
            _all = new HashSet<T>(_comparer);
        }

        /// <summary>
        /// Gets the leaf vertices that do not depend on anything.
        /// </summary>
        /// <value>
        /// The leaf vertices that do not depend on anything.
        /// </value>
        [NotNull]
        [PublicAPI]
        public IEnumerable<T> TopLeaves
        {
            // ReSharper disable once PossibleNullReferenceException
            get { return _all.Except(_dependsOn.Where(kvp => kvp.Value.Count > 0).Select(kvp => kvp.Key)); }
        }

        /// <summary>
        /// Gets the leaf vertices that do not have any dependencies.
        /// </summary>
        /// <value>
        /// The leaf vertices that do not have any dependencies.
        /// </value>
        [NotNull]
        [PublicAPI]
        public IEnumerable<T> BottomLeaves
        {
            // ReSharper disable once PossibleNullReferenceException
            get { return _all.Except(_dependencies.Where(kvp => kvp.Value.Count > 0).Select(kvp => kvp.Key)); }
        }

        /// <summary>
        /// Adds an object with no dependencies to the dependency graph.
        /// </summary>
        /// <param name="obj">The object to add.</param>
        /// <returns><see langword="true"/> if the object was added; <see langword="false"/> if the graph already contained the object.</returns>
        [PublicAPI]
        public bool Add([NotNull] T obj)
        {
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(obj, null));

            if (!_all.Add(obj)) return false;

            _dependencies.Add(obj, new HashSet<T>(_comparer));
            _dependsOn.Add(obj, new HashSet<T>(_comparer));
            
            return true;
        }

        /// <summary>
        /// Adds a dependency from <paramref name="a" /> to <paramref name="b" />.
        /// </summary>
        /// <param name="a">The object that has a dependency.</param>
        /// <param name="b">The object that <paramref name="a" /> depends on.</param>
        /// <exception cref="System.InvalidOperationException">Cannot add the dependency as this would cause a cycle</exception>
        [PublicAPI]
        public void Add([NotNull] T a, [NotNull] T b)
        {
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(a, null));
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(b, null));

            _all.Add(a);
            _all.Add(b);

            if (GetAllDependencies(a).Contains(b))
                throw new InvalidOperationException("Cannot add the dependency as this would cause a cycle");

            HashSet<T> depends;
            if (!_dependsOn.TryGetValue(a, out depends))
                _dependsOn[a] = depends = new HashSet<T>(_comparer);
            Contract.Assert(depends != null);
            depends.Add(b);

            if (!_dependencies.TryGetValue(b, out depends))
                _dependencies[b] = depends = new HashSet<T>(_comparer);
            Contract.Assert(depends != null);
            depends.Add(a);
        }

        /// <summary>
        /// Determines whether the graph contains the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><see langword="true"/> if the graph contains the specified object; otherwise <see langword="false"/>.</returns>
        [PublicAPI]
        public bool Contains([NotNull] T obj)
        {
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(obj, null));

            return _all.Contains(obj);
        }

        /// <summary>
        /// Gets the immediate dependencies of the object given.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">value;The value given is not in the graph</exception>
        [NotNull]
        [PublicAPI]
        public IEnumerable<T> GetDependencies([NotNull] T obj)
        {
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(obj, null));
            Contract.Requires<ArgumentOutOfRangeException>(_all.Contains(obj), "The value given is not in the graph");

            HashSet<T> depends;
            // ReSharper disable AssignNullToNotNullAttribute
            return _dependencies.TryGetValue(obj, out depends) ? depends : Enumerable.Empty<T>();
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Gets all the dependencies of the object given, 'recursively'.
        /// </summary>
        /// <param name="obj">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">value;The value given is not in the graph</exception>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetAllDependencies([NotNull] T obj)
        {
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(obj, null));
            Contract.Requires<ArgumentOutOfRangeException>(_all.Contains(obj), "The value given is not in the graph");

            return GetAllDependenciesIterator(obj);
        }

        [NotNull]
        private IEnumerable<T> GetAllDependenciesIterator([NotNull] T obj)
        {
            HashSet<T> seen = new HashSet<T>(_comparer);
            Queue<T> queue = new Queue<T>();
            queue.Enqueue(obj);

            while (queue.Count > 0)
            {
                T current = queue.Dequeue();
                Contract.Assert(!ReferenceEquals(current, null));

                HashSet<T> depends;
                if (_dependencies.TryGetValue(current, out depends))
                {
                    Contract.Assert(depends != null);
                    foreach (T val in depends)
                    {
                        if (seen.Add(val))
                            yield return val;
                        queue.Enqueue(val);
                    }
                }
            }
        }

        [NotNull]
        private IEnumerable<T> GetAllIterator([NotNull] Dictionary<T, HashSet<T>> dict, [NotNull] IEnumerable<T> objs)
        {
            HashSet<T> seen = new HashSet<T>(_comparer);
            Queue<T> queue = new Queue<T>();

            foreach (T current in objs)
            {
                Contract.Assert(!ReferenceEquals(current, null));
                yield return current;

                HashSet<T> depends;
                if (dict.TryGetValue(current, out depends))
                {
                    Contract.Assert(depends != null);
                    foreach (T val in depends)
                        queue.Enqueue(val);
                }
            }

            while (queue.Count > 0)
            {
                T current = queue.Dequeue();
                Contract.Assert(!ReferenceEquals(current, null));
                if (seen.Add(current))
                    yield return current;

                HashSet<T> depends;
                if (dict.TryGetValue(current, out depends))
                {
                    Contract.Assert(depends != null);
                    foreach (T val in depends)
                        queue.Enqueue(val);
                }
            }
        }

        // Starts with the things that do not depend on anything
        /// <summary>
        /// Traverses the graph from the top down, calling an <paramref name="action"/> on each object in the graph.
        /// </summary>
        /// <param name="action">The action that is called for each object. The action is passed the current object and all the objects that it depends on.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public IEnumerable<T> TraverseTopDown([NotNull] [InstantHandle] Action<T, IEnumerable<T>> action)
        {
            Contract.Requires<ArgumentNullException>(action != null);
            return TraverseInternal(action, TopLeaves, BottomLeaves, _dependsOn, _dependencies);
        }

        /// <summary>
        /// Traverses the graph from the bottom up, calling an <paramref name="action"/> on each object in the graph.
        /// </summary>
        /// <param name="action">The action that is called for each object. The action is passed the current object and all the objects that depend on it.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public IEnumerable<T> TraverseBottomUp([NotNull] [InstantHandle] Action<T, IEnumerable<T>> action)
        {
            Contract.Requires<ArgumentNullException>(action != null);
            return TraverseInternal(action, BottomLeaves, TopLeaves, _dependencies, _dependsOn);
        }

        /// <summary>
        /// Traverses the graph from <paramref name="top"/> to <paramref name="bottom"/>.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="top">The top leaves.</param>
        /// <param name="bottom">The bottom leaves.</param>
        /// <param name="up">The next values up the graph (towards <paramref name="top"/>) from each value.</param>
        /// <param name="down">The next values down the graph (towards <paramref name="bottom"/>) from each value.</param>
        /// <returns><paramref name="bottom"/></returns>
        [NotNull]
        [PublicAPI]
        private IEnumerable<T> TraverseInternal(
            [NotNull] [InstantHandle] Action<T, IEnumerable<T>> action,
            [NotNull] [InstantHandle] IEnumerable<T> top,
            [NotNull] IEnumerable<T> bottom,
            [NotNull] Dictionary<T, HashSet<T>> up,
            [NotNull] Dictionary<T, HashSet<T>> down)
        {
            Queue<T> queue = new Queue<T>(top);
            HashSet<T> seen = new HashSet<T>(_comparer);

            while (queue.Count > 0)
            {
                T current = queue.Dequeue();
                Contract.Assert(!ReferenceEquals(current, null));

                HashSet<T> depends;
                action(
                    current,
                    up.TryGetValue(current, out depends)
                        ? depends
                        : Enumerable.Empty<T>());

                if (down.TryGetValue(current, out depends) &&
                    // ReSharper disable once PossibleNullReferenceException
                    depends.Count > 0)
                {
                    Contract.Assert(depends != null);
                    foreach (T val in depends)
                        if (seen.Add(val))
                            queue.Enqueue(val);
                }
            }

            return bottom;
        }
    }
}