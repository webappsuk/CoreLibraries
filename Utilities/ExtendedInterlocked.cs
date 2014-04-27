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

using System.Threading;
using JetBrains.Annotations;
using WebApplications.Utilities.Ranges;

namespace WebApplications.Utilities
{
    /// <summary>
    /// An extension to the interlocked funcionality in .NET for ranges
    /// </summary>
    public static class ExtendedInterlocked
    {
        /// <summary>
        /// Increments a value whilst keeping it inside a range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="range">The range.</param>
        /// <returns>The incremented result.</returns>
        public static int Increment(ref int value, [NotNull] IntRange range)
        {
            return Increment(ref value, range.Start, range.End);
        }

        /// <summary>
        /// Increments a value whilst keeping it inside a range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="start">The start of the range (inclusive).</param>
        /// <param name="end">The end of the range (inclusive).</param>
        /// <returns>The incremented result.</returns>
        public static int Increment(ref int value, int start, int end)
        {
            SpinWait spinWait = new SpinWait();
            do
            {
                int v = value;
                if (Interlocked.CompareExchange(ref value, v >= end ? start : v + 1, v) == v)
                    return v;

                spinWait.SpinOnce();
            } while (true);
        }

        /// <summary>
        /// Decrement a value whilst keeping it inside a range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="range">The range.</param>
        /// <returns>The decremented result.</returns>
        public static int Decrement(ref int value, [NotNull] IntRange range)
        {
            return Decrement(ref value, range.Start, range.End);
        }

        /// <summary>
        /// Decrement a value whilst keeping it inside a range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="start">The start of the range (inclusive).</param>
        /// <param name="end">The end of the range (inclusive).</param>
        /// <returns>The decremented result.</returns>
        public static int Decrement(ref int value, int start, int end)
        {
            SpinWait spinWait = new SpinWait();
            do
            {
                int v = value;
                if (Interlocked.CompareExchange(ref value, v - 1 <= start ? end : v - 1, v) == v)
                    return v;

                spinWait.SpinOnce();
            } while (true);
        }
    }
}