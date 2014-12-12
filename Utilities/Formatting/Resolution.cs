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
using System.Text;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// A resolution, is used to resolve tags in a <see cref="FormatBuilder"/>.
    /// </summary>
    public struct Resolution : IFormattable
    {
        /// <summary>
        /// The tag is unknown (cached).
        /// </summary>
        [PublicAPI]
        public static readonly object Unknown = default(Resolution);

        /// <summary>
        /// The tag is not yet known (not cached).
        /// </summary>
        [PublicAPI]
        public static readonly object UnknownYet = new Resolution(false, true);

        /// <summary>
        /// The tag's value is Null.
        /// </summary>
        [PublicAPI]
        public static readonly object Null = new Resolution(null);

        /// <summary>
        /// The tag's value is currently null, but may change.
        /// </summary>
        [PublicAPI]
        public static readonly object CurrentlyNull = new Resolution(null, true);

        /// <summary>
        /// The tag's value is <see cref="string.Empty"/>.
        /// </summary>
        [PublicAPI]
        public static readonly object Empty = new Resolution(string.Empty);

        /// <summary>
        /// The tag's value is currently <see cref="string.Empty"/>, but may change.
        /// </summary>
        [PublicAPI]
        public static readonly object CurrentlyEmpty = new Resolution(string.Empty, true);

        /// <summary>
        /// Whether this resolution should be cached.
        /// </summary>
        [PublicAPI]
        public readonly bool NoCache;

        /// <summary>
        /// <see langword="true"/> is the value represents a resolution; otherwise <see langword="false"/> to indicate the original tag was unknown.
        /// </summary>
        [PublicAPI]
        public readonly bool IsResolved;

        /// <summary>
        /// The resolved value if <see cref="IsResolved"/> is <see langword="true"/>.
        /// </summary>
        [PublicAPI]
        [CanBeNull]
        public readonly object Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resolution" /> struct.
        /// </summary>
        /// <param name="isResolved">if set to <see langword="true" /> [is resolved].</param>
        /// <param name="noCache">if set to <see langword="true" /> then this resolution is not cached.</param>
        private Resolution(bool isResolved, bool noCache)
            : this()
        {
            IsResolved = isResolved;
            NoCache = noCache;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resolution" /> struct.
        /// </summary>
        /// <param name="value">The resolved value.</param>
        /// <param name="noCache">if set to <see langword="true" /> then this resolution is not cached.</param>
        public Resolution([CanBeNull] object value, bool noCache = false)
            : this()
        {
            NoCache = noCache;
            IsResolved = true;
            Value = value;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format to use.-or- A null reference (Nothing in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable" /> implementation.</param>
        /// <param name="formatProvider">The provider to use to format the value.-or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string ToString([CanBeNull]string format, [CanBeNull]IFormatProvider formatProvider = null)
        {
            StringBuilder builder = new StringBuilder();
            if (!IsResolved)
                builder.Append("<unresolved>");
            else if (Value == null)
                builder.Append("<null>");
            else
            {
                IFormattable f = Value as IFormattable;
                if (f != null)
                    builder.Append(f.ToString(format, formatProvider));
                else
                    builder.Append(Value);
            }

            if (NoCache)
                builder.Append(" [DON'T CACHE]");
            return builder.ToString();
        }
    }
}