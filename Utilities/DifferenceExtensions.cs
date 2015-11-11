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

using System.Collections.Generic;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Difference;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Extension methods for the difference engine.
    /// </summary>
    /// <seealso cref="Differences{T}"/>
    [PublicAPI]
    public static class DifferenceExtensions
    {
        /// <summary>
        /// Find the differences between two collections.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="a">The first collection.</param>
        /// <param name="b">The second collection</param>
        /// <param name="comparer">The comparer to compare items in each collection.</param>
        /// <returns>Returns a read only list of <see cref="Chunk{T}">chunks</see>.</returns>
        /// <remarks>
        /// <para> 
        /// Based on "An O(ND) Difference Algorithm and its Variations" by Eugene Myers
        /// (http://www.xmailserver.org/diff2.pdf) Algorithmica Vol. 1 No. 2, 1986, p 251.  
        /// </para>
        /// </remarks>
        [NotNull]
        [ItemNotNull]
        public static Differences<T> Diff<T>(
            [NotNull] this IReadOnlyList<T> a,
            [NotNull] IReadOnlyList<T> b,
            IEqualityComparer<T> comparer = null)
            => new Differences<T>(a, 0, a.Count, b, 0, b.Count, comparer);

        /// <summary>
        /// Find the differences between two collections.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="a">The first collection.</param>
        /// <param name="offsetA">The offset to the start of a window in the first collection.</param>
        /// <param name="lengthA">The length of the window in the first collection.</param>
        /// <param name="b">The second collection</param>
        /// <param name="offsetB">The offset to the start of a window in the second collection.</param>
        /// <param name="lengthB">The length of the window in the second collection.</param>
        /// <param name="comparer">The comparer to compare items in each collection.</param>
        /// <returns>Returns a read only list of <see cref="Chunk{T}">chunks</see>.</returns>
        /// <remarks>
        /// <para> 
        /// Based on "An O(ND) Difference Algorithm and its Variations" by Eugene Myers
        /// (http://www.xmailserver.org/diff2.pdf) Algorithmica Vol. 1 No. 2, 1986, p 251.  
        /// </para>
        /// </remarks>
        [NotNull]
        [ItemNotNull]
        public static Differences<T> Diff<T>(
            [NotNull] this IReadOnlyList<T> a,
            int offsetA,
            int lengthA,
            [NotNull] IReadOnlyList<T> b,
            int offsetB,
            int lengthB,
            IEqualityComparer<T> comparer = null)
            => new Differences<T>(a, offsetA, lengthA, b, offsetB, lengthB, comparer);
    }
}