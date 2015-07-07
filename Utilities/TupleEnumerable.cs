








 
 
#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using System.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for querying objects that implement <see cref="IEnumerable{T}"/> of <see cref="Tuple">tuples</see>.
    /// </summary>
    [PublicAPI]
    public static class TupleEnumerable
    {


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2> Empty<T1, T2>()
        {
            return Enumerable<T1, T2>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2> AsTupleEnumerable<T1, T2>([CanBeNull] this IEnumerable<Tuple<T1, T2>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2> tuples = enumerable as IEnumerable<T1, T2>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2>>()))
                return Enumerable<T1, T2>.Empty;
            return new Enumerable<T1, T2>(enumerable);
        }

        private class Enumerable<T1, T2> : IEnumerable<T1, T2>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2> Empty = 
                new Enumerable<T1, T2>(Enumerable.Empty<Tuple<T1, T2>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3> Empty<T1, T2, T3>()
        {
            return Enumerable<T1, T2, T3>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3> AsTupleEnumerable<T1, T2, T3>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3> tuples = enumerable as IEnumerable<T1, T2, T3>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3>>()))
                return Enumerable<T1, T2, T3>.Empty;
            return new Enumerable<T1, T2, T3>(enumerable);
        }

        private class Enumerable<T1, T2, T3> : IEnumerable<T1, T2, T3>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3> Empty = 
                new Enumerable<T1, T2, T3>(Enumerable.Empty<Tuple<T1, T2, T3>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4> Empty<T1, T2, T3, T4>()
        {
            return Enumerable<T1, T2, T3, T4>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4> AsTupleEnumerable<T1, T2, T3, T4>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4> tuples = enumerable as IEnumerable<T1, T2, T3, T4>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4>>()))
                return Enumerable<T1, T2, T3, T4>.Empty;
            return new Enumerable<T1, T2, T3, T4>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4> : IEnumerable<T1, T2, T3, T4>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4> Empty = 
                new Enumerable<T1, T2, T3, T4>(Enumerable.Empty<Tuple<T1, T2, T3, T4>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5> Empty<T1, T2, T3, T4, T5>()
        {
            return Enumerable<T1, T2, T3, T4, T5>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5> AsTupleEnumerable<T1, T2, T3, T4, T5>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5>>()))
                return Enumerable<T1, T2, T3, T4, T5>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5> : IEnumerable<T1, T2, T3, T4, T5>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5> Empty = 
                new Enumerable<T1, T2, T3, T4, T5>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6> Empty<T1, T2, T3, T4, T5, T6>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6> AsTupleEnumerable<T1, T2, T3, T4, T5, T6>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6> : IEnumerable<T1, T2, T3, T4, T5, T6>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7> Empty<T1, T2, T3, T4, T5, T6, T7>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7> : IEnumerable<T1, T2, T3, T4, T5, T6, T7>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8> Empty<T1, T2, T3, T4, T5, T6, T7, T8>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        /// <typeparam name="T21">The type of item 21.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        /// <typeparam name="T21">The type of item 21.</typeparam>
        /// <typeparam name="T22">The type of item 22.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        /// <typeparam name="T21">The type of item 21.</typeparam>
        /// <typeparam name="T22">The type of item 22.</typeparam>
        /// <typeparam name="T23">The type of item 23.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        /// <typeparam name="T21">The type of item 21.</typeparam>
        /// <typeparam name="T22">The type of item 22.</typeparam>
        /// <typeparam name="T23">The type of item 23.</typeparam>
        /// <typeparam name="T24">The type of item 24.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        /// <typeparam name="T21">The type of item 21.</typeparam>
        /// <typeparam name="T22">The type of item 22.</typeparam>
        /// <typeparam name="T23">The type of item 23.</typeparam>
        /// <typeparam name="T24">The type of item 24.</typeparam>
        /// <typeparam name="T25">The type of item 25.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        /// <typeparam name="T21">The type of item 21.</typeparam>
        /// <typeparam name="T22">The type of item 22.</typeparam>
        /// <typeparam name="T23">The type of item 23.</typeparam>
        /// <typeparam name="T24">The type of item 24.</typeparam>
        /// <typeparam name="T25">The type of item 25.</typeparam>
        /// <typeparam name="T26">The type of item 26.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        /// <typeparam name="T21">The type of item 21.</typeparam>
        /// <typeparam name="T22">The type of item 22.</typeparam>
        /// <typeparam name="T23">The type of item 23.</typeparam>
        /// <typeparam name="T24">The type of item 24.</typeparam>
        /// <typeparam name="T25">The type of item 25.</typeparam>
        /// <typeparam name="T26">The type of item 26.</typeparam>
        /// <typeparam name="T27">The type of item 27.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        /// <typeparam name="T21">The type of item 21.</typeparam>
        /// <typeparam name="T22">The type of item 22.</typeparam>
        /// <typeparam name="T23">The type of item 23.</typeparam>
        /// <typeparam name="T24">The type of item 24.</typeparam>
        /// <typeparam name="T25">The type of item 25.</typeparam>
        /// <typeparam name="T26">The type of item 26.</typeparam>
        /// <typeparam name="T27">The type of item 27.</typeparam>
        /// <typeparam name="T28">The type of item 28.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        /// <typeparam name="T21">The type of item 21.</typeparam>
        /// <typeparam name="T22">The type of item 22.</typeparam>
        /// <typeparam name="T23">The type of item 23.</typeparam>
        /// <typeparam name="T24">The type of item 24.</typeparam>
        /// <typeparam name="T25">The type of item 25.</typeparam>
        /// <typeparam name="T26">The type of item 26.</typeparam>
        /// <typeparam name="T27">The type of item 27.</typeparam>
        /// <typeparam name="T28">The type of item 28.</typeparam>
        /// <typeparam name="T29">The type of item 29.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        /// <typeparam name="T21">The type of item 21.</typeparam>
        /// <typeparam name="T22">The type of item 22.</typeparam>
        /// <typeparam name="T23">The type of item 23.</typeparam>
        /// <typeparam name="T24">The type of item 24.</typeparam>
        /// <typeparam name="T25">The type of item 25.</typeparam>
        /// <typeparam name="T26">The type of item 26.</typeparam>
        /// <typeparam name="T27">The type of item 27.</typeparam>
        /// <typeparam name="T28">The type of item 28.</typeparam>
        /// <typeparam name="T29">The type of item 29.</typeparam>
        /// <typeparam name="T30">The type of item 30.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        /// <typeparam name="T21">The type of item 21.</typeparam>
        /// <typeparam name="T22">The type of item 22.</typeparam>
        /// <typeparam name="T23">The type of item 23.</typeparam>
        /// <typeparam name="T24">The type of item 24.</typeparam>
        /// <typeparam name="T25">The type of item 25.</typeparam>
        /// <typeparam name="T26">The type of item 26.</typeparam>
        /// <typeparam name="T27">The type of item 27.</typeparam>
        /// <typeparam name="T28">The type of item 28.</typeparam>
        /// <typeparam name="T29">The type of item 29.</typeparam>
        /// <typeparam name="T30">The type of item 30.</typeparam>
        /// <typeparam name="T31">The type of item 31.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        /// <typeparam name="T21">The type of item 21.</typeparam>
        /// <typeparam name="T22">The type of item 22.</typeparam>
        /// <typeparam name="T23">The type of item 23.</typeparam>
        /// <typeparam name="T24">The type of item 24.</typeparam>
        /// <typeparam name="T25">The type of item 25.</typeparam>
        /// <typeparam name="T26">The type of item 26.</typeparam>
        /// <typeparam name="T27">The type of item 27.</typeparam>
        /// <typeparam name="T28">The type of item 28.</typeparam>
        /// <typeparam name="T29">The type of item 29.</typeparam>
        /// <typeparam name="T30">The type of item 30.</typeparam>
        /// <typeparam name="T31">The type of item 31.</typeparam>
        /// <typeparam name="T32">The type of item 32.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        /// <typeparam name="T21">The type of item 21.</typeparam>
        /// <typeparam name="T22">The type of item 22.</typeparam>
        /// <typeparam name="T23">The type of item 23.</typeparam>
        /// <typeparam name="T24">The type of item 24.</typeparam>
        /// <typeparam name="T25">The type of item 25.</typeparam>
        /// <typeparam name="T26">The type of item 26.</typeparam>
        /// <typeparam name="T27">The type of item 27.</typeparam>
        /// <typeparam name="T28">The type of item 28.</typeparam>
        /// <typeparam name="T29">The type of item 29.</typeparam>
        /// <typeparam name="T30">The type of item 30.</typeparam>
        /// <typeparam name="T31">The type of item 31.</typeparam>
        /// <typeparam name="T32">The type of item 32.</typeparam>
        /// <typeparam name="T33">The type of item 33.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        /// <typeparam name="T21">The type of item 21.</typeparam>
        /// <typeparam name="T22">The type of item 22.</typeparam>
        /// <typeparam name="T23">The type of item 23.</typeparam>
        /// <typeparam name="T24">The type of item 24.</typeparam>
        /// <typeparam name="T25">The type of item 25.</typeparam>
        /// <typeparam name="T26">The type of item 26.</typeparam>
        /// <typeparam name="T27">The type of item 27.</typeparam>
        /// <typeparam name="T28">The type of item 28.</typeparam>
        /// <typeparam name="T29">The type of item 29.</typeparam>
        /// <typeparam name="T30">The type of item 30.</typeparam>
        /// <typeparam name="T31">The type of item 31.</typeparam>
        /// <typeparam name="T32">The type of item 32.</typeparam>
        /// <typeparam name="T33">The type of item 33.</typeparam>
        /// <typeparam name="T34">The type of item 34.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }


        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35}"/> that has the specified type arguments.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <typeparam name="T10">The type of item 10.</typeparam>
        /// <typeparam name="T11">The type of item 11.</typeparam>
        /// <typeparam name="T12">The type of item 12.</typeparam>
        /// <typeparam name="T13">The type of item 13.</typeparam>
        /// <typeparam name="T14">The type of item 14.</typeparam>
        /// <typeparam name="T15">The type of item 15.</typeparam>
        /// <typeparam name="T16">The type of item 16.</typeparam>
        /// <typeparam name="T17">The type of item 17.</typeparam>
        /// <typeparam name="T18">The type of item 18.</typeparam>
        /// <typeparam name="T19">The type of item 19.</typeparam>
        /// <typeparam name="T20">The type of item 20.</typeparam>
        /// <typeparam name="T21">The type of item 21.</typeparam>
        /// <typeparam name="T22">The type of item 22.</typeparam>
        /// <typeparam name="T23">The type of item 23.</typeparam>
        /// <typeparam name="T24">The type of item 24.</typeparam>
        /// <typeparam name="T25">The type of item 25.</typeparam>
        /// <typeparam name="T26">The type of item 26.</typeparam>
        /// <typeparam name="T27">The type of item 27.</typeparam>
        /// <typeparam name="T28">The type of item 28.</typeparam>
        /// <typeparam name="T29">The type of item 29.</typeparam>
        /// <typeparam name="T30">The type of item 30.</typeparam>
        /// <typeparam name="T31">The type of item 31.</typeparam>
        /// <typeparam name="T32">The type of item 32.</typeparam>
        /// <typeparam name="T33">The type of item 33.</typeparam>
        /// <typeparam name="T34">The type of item 34.</typeparam>
        /// <typeparam name="T35">The type of item 35.</typeparam>
        [NotNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> Empty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>()
        {
            return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>.Empty;
        }

        /// <summary>
        /// Returns the input typed as IEnumerable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35}.
        /// </summary>
        [CanBeNull]
        public static IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> AsTupleEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>([CanBeNull] this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>> enumerable)
        {
            if (enumerable == null)
                return null;
            IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> tuples = enumerable as IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>;
            if (tuples != null)
                return tuples;
            if (ReferenceEquals(enumerable, Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>>()))
                return Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>.Empty;
            return new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(enumerable);
        }

        private class Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> : IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>
        {
            [NotNull]
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> Empty = 
                new Enumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(Enumerable.Empty<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>>());

            [NotNull]
            private readonly IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>> _enum;

            public Enumerable([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>> enumerable) {_enum = enumerable;}
            public IEnumerator<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>> GetEnumerator() { return _enum.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() {  return _enum.GetEnumerator(); }
        }

    }
}
 
