#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: Range.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WebApplications.Utilities.Ranges
{
    /// <summary>
    ///   A range of values of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the values in the range.</typeparam>
    /// <typeparam name="TStep">The type of the step used to iterate through the collection.</typeparam>
    public class Range<TValue, TStep> : IEnumerable<TValue>
    {
        /// <summary>
        ///   Method for performing additions.
        /// </summary>
        public static readonly Func<TValue, TStep, TValue> Add;

        /// <summary>
        ///   Method for performing LessThan comparison.
        /// </summary>
        public static readonly Func<TValue, TValue, bool> LessThan;

        /// <summary>
        ///   The end of the range (inclusive).
        /// </summary>
        public readonly TValue End;

        /// <summary>
        ///   The start of the range (inclusive).
        /// </summary>
        public readonly TValue Start;

        /// <summary>
        ///   The step for enumeration
        /// </summary>
        public readonly TStep Step;

        /// <summary>
        ///   Initializes the <see cref="Range&lt;T, S&gt;"/> class.
        /// </summary>
        static Range()
        {
            // Create input parameter expressions
            ParameterExpression parameterAExpression = Expression.Parameter(typeof (TValue), "a");
            ParameterExpression parameterBExpression = Expression.Parameter(typeof (TStep), "b");

            // Create lambda for addition and compile
            Add =
                (Func<TValue, TStep, TValue>)
                Expression.Lambda(
                    Expression.Add(parameterAExpression, parameterBExpression),
                    parameterAExpression,
                    parameterBExpression).Compile();

            // Change type of parameter B to TValue (from TStep)
            parameterBExpression = Expression.Parameter(typeof(TValue), "b");

            // Create lambda for less than and compile
            LessThan =
                (Func<TValue, TValue, bool>)
                Expression.Lambda(
                    Expression.LessThan(parameterAExpression, parameterBExpression),
                    parameterAExpression,
                    parameterBExpression).Compile();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Range&lt;T,S&gt;"/> class.
        /// </summary>
        /// <param name="start">The start value (inclusive).</param>
        /// <param name="end">The end value (inclusive).</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   The <paramref name="start"/> value was greater than the <paramref name="end"/> value.
        /// </exception>
        public Range(TValue start, TValue end)
        {
            if (LessThan(end, start))
            {
                throw new ArgumentOutOfRangeException(
                    "start",
                    start,
                    string.Format(
                        Resources.Range_StartGreaterThanEnd,
                        start,
                        end,
                        typeof (TValue)));
            }
            Start = start;
            End = end;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Range&lt;T,S&gt;"/> class.
        /// </summary>
        /// <param name="start">The start value (inclusive).</param>
        /// <param name="end">The end value (inclusive).</param>
        /// <param name="step">The range step.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   The <paramref name="start"/> value was greater than the <paramref name="end"/> value.
        /// </exception>
        public Range(TValue start, TValue end, TStep step)
        {
            if (LessThan(end, start))
            {
                throw new ArgumentOutOfRangeException(
                    "start",
                    start,
                    string.Format(
                        Resources.Range_StartGreaterThanEnd,
                        start,
                        end,
                        typeof (TValue)));
            }
            Start = start;
            End = end;
            Step = step;
        }

        #region IEnumerable<TValue> Members
        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<TValue> GetEnumerator()
        {
                for (TValue loop = Start, next = Start; !LessThan(End, loop); loop = next)
                {
                    next = Add(loop, Step);
                    // Perform checks which are normally done behind the scenes to avoid infinite loops due to overflows
                    if(!LessThan(loop,next))
                        yield break;
                    yield return loop;
                }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        /// <summary>
        ///   Binds the specified value so that it cannot fall outside the values of the range.
        /// </summary>
        /// <param name="value">The value to bind.</param>
        /// <returns>The bound value.</returns>
        public TValue Bind(TValue value)
        {
            if (LessThan(value, Start))
                return Start;
            if (LessThan(End, value))
                return End;
            return value;
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection and allows a specific step size.
        /// </summary>
        /// <param name="step">The step to iterate by.</param>
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<TValue> GetEnumerator(TStep step)
        {
            for (TValue loop = Start; !LessThan(End, loop); loop = Add(loop, step))
                yield return loop;
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", Start, End);
        }
    }

    /// <summary>
    ///   A simple range where the step is the same type as the values.
    /// </summary>
    /// <typeparam name="T">The value and step type.</typeparam>
    public class Range<T> : Range<T, T>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="Range&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="start">The start value (inclusive).</param>
        /// <param name="end">The end value (inclusive).</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   The <paramref name="start"/> value was greater than the <paramref name="end"/> value.
        /// </exception>
        public Range(T start, T end)
            : base(start, end)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Range&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value (inclusive).</param>
        /// <param name="step">The range step (inclusive).</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   The <paramref name="start"/> value was greater than the <paramref name="end"/> value.
        /// </exception>
        public Range(T start, T end, T step)
            : base(start, end, step)
        {
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", Start, End);
        }
    }
}