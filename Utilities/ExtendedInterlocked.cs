#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: ExtendedInterlocked.cs
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
// © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
#endregion

using System.Threading;
using JetBrains.Annotations;
using WebApplications.Utilities.Ranges;

namespace WebApplications.Utilities
{
    public static class ExtendedInterlocked
    {
        /// <summary>
        /// Increments a value whilst keeping it inside a range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int Increment(ref int value, [NotNull]IntRange range)
        {
            return Increment(ref value, range.Start, range.End);
        }

        /// <summary>
        /// Increments a value whilst keeping it inside a range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="start">The start of the range (inclusive).</param>
        /// <param name="end">The end of the range (inclusive).</param>
        /// <returns></returns>
        /// <remarks></remarks>
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
        /// <returns></returns>
        /// <remarks></remarks>
        public static int Decrement(ref int value, [NotNull]IntRange range)
        {
            return Decrement(ref value, range.Start, range.End);
        }

        /// <summary>
        /// Decrement a value whilst keeping it inside a range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="start">The start of the range (inclusive).</param>
        /// <param name="end">The end of the range (inclusive).</param>
        /// <returns></returns>
        /// <remarks></remarks>
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