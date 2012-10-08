#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Linq.Expressions;

namespace WebApplications.Utilities.Ranges
{
    /// <summary>
    ///   A range of values of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the values in the range.</typeparam>
    /// <typeparam name="TStep">The type of the step used to iterate through the collection.</typeparam>
    public class Range<TValue, TStep> : IEnumerable<TValue>, IEquatable<Range<TValue, TStep>>
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

            // Workaround for the fact that byte types have no addition operator.
            bool castTValueToShort = typeof (TValue) == typeof (byte) ||
                              typeof (TValue) == typeof (sbyte);
            bool castTStepToShort = typeof (TStep) == typeof (byte) ||
                              typeof (TStep) == typeof (sbyte);

            Expression lhs = castTValueToShort
                                 ? Expression.Convert(parameterAExpression, typeof (short))
                                 : (Expression)parameterAExpression;
            Expression rhs = castTStepToShort
                                 ? Expression.Convert(parameterBExpression, typeof(short))
                                 : (Expression)parameterBExpression;

            // Create lambda for addition and compile
            Expression add = Expression.Add(lhs, rhs);
            if (add.Type != typeof(TValue))
                add = Expression.Convert(add, typeof (TValue));

            Add =
                (Func<TValue, TStep, TValue>)
                Expression.Lambda(
                    add,
                    parameterAExpression,
                    parameterBExpression).Compile();

            // Create new parameter for TValue 
            parameterBExpression = Expression.Parameter(typeof (TValue), "b");
            rhs = castTValueToShort
                      ? Expression.Convert(parameterBExpression, typeof (short))
                      : (Expression) parameterBExpression;

            // Create lambda for less than and compile
            LessThan =
                (Func<TValue, TValue, bool>)
                Expression.Lambda(
                    Expression.LessThan(lhs, rhs),
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
            bool nextIsInRange = true;
            for (TValue loop = Start, next = Start; !LessThan(End, loop) && nextIsInRange; loop = next)
            {
                try
                {
                    next = Add(loop, Step);
                }
                catch (ArgumentOutOfRangeException) // For dates
                {
                    nextIsInRange = false;
                }
                catch (OverflowException) // For decimals
                {
                    nextIsInRange = false;
                }
                // Perform checks which are normally done behind the scenes to avoid infinite loops due to interger overflows
                if (!LessThan(loop, next) && nextIsInRange)
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

        #region IEquatable<Range<TValue,TStep>> Members
        /// <inheritDoc />
        public bool Equals(Range<TValue, TStep> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TValue>.Default.Equals(End, other.End) &&
                   EqualityComparer<TValue>.Default.Equals(Start, other.Start) &&
                   EqualityComparer<TStep>.Default.Equals(Step, other.Step);
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
            for (TValue loop = Start, next = Start; !LessThan(End, loop); loop = next)
            {
                try
                {
                    next = Add(loop, step);
                }
                catch (ArgumentOutOfRangeException)
                {
                    yield break;
                }
                catch (OverflowException)
                {
                    yield break;
                }
                // Perform checks avoid infinite loops due to integer overflows
                if (!LessThan(loop, next))
                    yield break;
                yield return loop;
            }
        }

        /// <inheritDoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            Range<TValue, TStep> range = obj as Range<TValue, TStep>;
            if (ReferenceEquals(null, range)) return false;
            return EqualityComparer<TValue>.Default.Equals(End, range.End) &&
                   EqualityComparer<TValue>.Default.Equals(Start, range.Start) &&
                   EqualityComparer<TStep>.Default.Equals(Step, range.Step);
        }

        /// <inheritDoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = EqualityComparer<TValue>.Default.GetHashCode(End);
                hashCode = (hashCode*397) ^ EqualityComparer<TValue>.Default.GetHashCode(Start);
                hashCode = (hashCode*397) ^ EqualityComparer<TStep>.Default.GetHashCode(Step);
                return hashCode;
            }
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