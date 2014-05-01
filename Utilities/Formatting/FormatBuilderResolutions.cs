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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    public sealed partial class FormatBuilder
    {
        /// <summary>
        /// Holds resolved values for a given resolver, in a stack.
        /// </summary>
        private class Resolutions : Resolvable
        {
            /// <summary>
            /// The parent resolutions.
            /// </summary>
            [CanBeNull]
            [PublicAPI]
            public readonly Resolutions Parent;

            /// <summary>
            /// The resolver.
            /// </summary>
            [NotNull]
            [PublicAPI]
            private readonly ResolveDelegate _resolver;

            /// <summary>
            /// Any resolved values.
            /// </summary>
            [NotNull]
            [PublicAPI]
            private readonly Dictionary<string, Optional<object>> _values;

            /// <summary>
            /// Initializes a new instance of the <see cref="Resolutions" /> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="resolver">The resolver.</param>
            /// <param name="isCaseSensitive">if set to <see langword="true" /> then tags are case sensitive.</param>
            /// <param name="resolveOuterTags">if set to <see langword="true" />  outer tags should be resolved automatically in formats.</param>
            public Resolutions(
                [CanBeNull] Resolutions parent,
                [NotNull] ResolveDelegate resolver,
                bool isCaseSensitive,
                bool resolveOuterTags)
                : base(isCaseSensitive, resolveOuterTags)
            {
                Contract.Requires(resolver != null);
                Parent = parent;
                _resolver = resolver;
                _values =
                    new Dictionary<string, Optional<object>>(
                        isCaseSensitive ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase);
            }

            // ReSharper disable once CodeAnnotationAnalyzer
            /// <summary>
            /// Resolves the specified chunk.
            /// </summary>
            /// <param name="writer">The writer.</param>
            /// <param name="chunk">The chunk.</param>
            /// <returns>An assigned<see cref="Optional{T}" /> if resolved; otherwise <see cref="Optional{T}.Unassigned" /></returns>
            public override Optional<object> Resolve(TextWriter writer, FormatChunk chunk)
            {
                Contract.Assert(chunk.Tag != null);

                Optional<object> value;
                if (_values.TryGetValue(chunk.Tag, out value))
                    return value;

                value = _resolver(writer, chunk);
                if (!value.IsAssigned &&
                    ResolveOuterTags && 
                    Parent != null)
                     value = Parent.Resolve(writer, chunk);

                _values[chunk.Tag] = value.Value;
                return value;
            }
        }
    }
}