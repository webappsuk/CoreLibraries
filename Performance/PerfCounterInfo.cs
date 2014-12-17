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
using System.Diagnostics.Contracts;
using System.IO;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Performance
{
    /// <summary>
    /// Provides read-only access to a <see cref="PerfCategory">PerfCategory's</see> information.
    /// </summary>
    public abstract class PerfCounterInfo : ResolvableWriteable
    {
        /// <summary>
        /// The value type
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly Type ValueType;

        /// <summary>
        /// The name
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string Name;

        /// <summary>
        /// The help
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string Help;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        [CanBeNull]
        [PublicAPI]
        public abstract object Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerfCounterInfo" /> class.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="name">The name.</param>
        /// <param name="help">The help.</param>
        protected PerfCounterInfo([NotNull] Type valueType, [NotNull] string name, [NotNull] string help)
        {
            Contract.Requires(valueType != null);
            Contract.Requires(name != null);
            Contract.Requires(help != null);
            ValueType = valueType;
            Name = name;
            Help = help;
        }

        /// <summary>
        /// The default builder for writing out information.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatBuilder VerboseFormat =
            new FormatBuilder("{Name}\t: {Value}", true);

        /// <summary>
        /// Gets the default format.
        /// </summary>
        /// <value>The default format.</value>
        public override FormatBuilder DefaultFormat
        {
            get { return VerboseFormat; }
        }

        /// <summary>
        /// Resolves the specified tag.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="chunk">The chunk.</param>
        /// <returns>An object that will be cached unless it is a <see cref="T:WebApplications.Utilities.Formatting.Resolution" />.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public override object Resolve(FormatWriteContext context, FormatChunk chunk)
        {
            // ReSharper disable once PossibleNullReferenceException
            switch (chunk.Tag.ToLowerInvariant())
            {
                case "name":
                    return Name;
                case "value":
                    return Value;
                default:
                    return Resolution.Unknown;
            }
        }
    }

    /// <summary>
    /// Provides read-only access to a <see cref="PerfCategory">PerfCategory's</see> information.
    /// </summary>
    [PublicAPI]
    public class PerfCounterInfo<T> : PerfCounterInfo
    {
        [NotNull]
        private readonly Func<T> _getLatestValueFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerfCounterInfo{T}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="help">The help.</param>
        /// <param name="getLatestValueFunc">The function to get the latest value.</param>
        public PerfCounterInfo([NotNull] string name, [NotNull] string help, [NotNull] Func<T> getLatestValueFunc)
            : base(typeof(T), name, help)
        {
            Contract.Requires(name != null);
            Contract.Requires(help != null);
            Contract.Requires(getLatestValueFunc != null);
            _getLatestValueFunc = getLatestValueFunc;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public override object Value
        {
            get { return _getLatestValueFunc(); }
        }
    }
}