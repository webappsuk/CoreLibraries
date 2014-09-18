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
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

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
        [NotNull]
        private static readonly Func<TValue, TStep, TValue> _add;

        /// <summary>
        ///   Method for performing LessThan comparison.
        /// </summary>
        [NotNull]
        private static readonly Func<TValue, TValue, bool> _lessThan;

        /// <summary>
        ///   Method for performing LessThanOrEqual comparison.
        /// </summary>
        [NotNull]
        private static readonly Func<TValue, TValue, bool> _lessThanOrEqual;

        /// <summary>
        ///   Method for performing GreaterThanOrEqual comparison.
        /// </summary>
        [NotNull]
        private static readonly Func<TValue, TValue, bool> _greaterThanOrEqual;

        /// <summary>
        ///   The end of the range (inclusive).
        /// </summary>
        private readonly TValue _end;

        /// <summary>
        ///   The start of the range (inclusive).
        /// </summary>
        private readonly TValue _start;

        /// <summary>
        ///   The step for enumeration
        /// </summary>
        private readonly TStep _step;

        /// <summary>
        ///   Initializes the <see cref="Range&lt;T, S&gt;"/> class.
        /// </summary>
        static Range()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            _add = Reflection.AddFunc<TValue, TStep, TValue>();
            _lessThan = Reflection.LessThanFunc<TValue>();
            _lessThanOrEqual = Reflection.LessThanOrEqualFunc<TValue>();
            _greaterThanOrEqual = Reflection.GreaterThanOrEqualFunc<TValue>();
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Range&lt;T,S&gt;"/> class.
        /// </summary>
        /// <param name="start">The start value (inclusive).</param>
        /// <param name="end">The end value (inclusive).</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   The <paramref name="start"/> value was greater than the <paramref name="end"/> value.
        /// </exception>
        public Range([NotNull] TValue start, [NotNull] TValue end)
        {
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(start, null));
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(end, null));
            if (_lessThan(end, start))
                throw new ArgumentOutOfRangeException(
                    "start",
                    start,
                    string.Format(
                    // ReSharper disable once AssignNullToNotNullAttribute
                        Resources.Range_StartGreaterThanEnd,
                        start,
                        end,
                        typeof(TValue)));
            _start = start;
            _end = end;
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
        public Range([NotNull] TValue start, [NotNull] TValue end, [NotNull]  TStep step)
        {
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(start, null));
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(end, null));
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(step, null));
            if (_lessThan(end, start))
                throw new ArgumentOutOfRangeException(
                    "start",
                    start,
                    string.Format(
                    // ReSharper disable once AssignNullToNotNullAttribute
                        Resources.Range_StartGreaterThanEnd,
                        start,
                        end,
                        typeof(TValue)));
            _start = start;
            _end = end;
            _step = step;
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
            for (TValue loop = Start, next = Start; !_lessThan(End, loop) && nextIsInRange; loop = next)
            {
                try
                {
                    next = _add(loop, Step);
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
                if (!_lessThan(loop, next) && nextIsInRange)
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
        ///   The start of the range (inclusive).
        /// </summary>
        public TValue Start
        {
            get { return _start; }
        }

        /// <summary>
        ///   The end of the range (inclusive).
        /// </summary>
        public TValue End
        {
            get { return _end; }
        }

        /// <summary>
        ///   The step for enumeration
        /// </summary>
        public TStep Step
        {
            get { return _step; }
        }

        /// <summary>
        ///   Binds the specified value so that it cannot fall outside the values of the range.
        /// </summary>
        /// <param name="value">The value to bind.</param>
        /// <returns>The bound value.</returns>
        [PublicAPI]
        public TValue Bind([NotNull] TValue value)
        {
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(value, null));
            if (_lessThan(value, _start))
                return _start;
            if (_lessThan(_end, value))
                return _end;
            return value;
        }

        /// <summary>
        /// Determines whether the <paramref name="value"/> given is within this range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains([NotNull] TValue value)
        {
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(value, null));
            return _lessThanOrEqual(_start, value) && _lessThanOrEqual(value, _end);
        }

        /// <summary>
        /// Checks if this range intersects with another range.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><see langword="true"/> if the range intersects with this range; otherwise <see langword="false"/>.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects([NotNull] Range<TValue, TStep> other)
        {
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(other, null));
            return _greaterThanOrEqual(other._end, _start) && _greaterThanOrEqual(_end, other._start);
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection and allows a specific step size.
        /// </summary>
        /// <param name="step">The step to iterate by.</param>
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        [NotNull,PublicAPI]
        public IEnumerator<TValue> GetEnumerator(TStep step)
        {
            for (TValue loop = _start, next; !_lessThan(_end, loop); loop = next)
            {
                try
                {
                    next = _add(loop, step);
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
                if (!_lessThan(loop, next))
                    yield break;
                yield return loop;
            }
        }

        /// <inheritDoc />
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            Range<TValue, TStep> range = obj as Range<TValue, TStep>;
            if (ReferenceEquals(null, range)) return false;
            return EqualityComparer<TValue>.Default.Equals(_end, range._end) &&
                   EqualityComparer<TValue>.Default.Equals(_start, range._start) &&
                   EqualityComparer<TStep>.Default.Equals(_step, range._step);
        }

        /// <inheritDoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = EqualityComparer<TValue>.Default.GetHashCode(_end);
                hashCode = (hashCode * 397) ^ EqualityComparer<TValue>.Default.GetHashCode(_start);
                hashCode = (hashCode * 397) ^ EqualityComparer<TStep>.Default.GetHashCode(_step);
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
            return string.Format("{0} - {1}", _start, _end);
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
        public Range([NotNull] T start, [NotNull] T end)
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
        public Range([NotNull] T start, [NotNull] T end, [NotNull] T step)
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