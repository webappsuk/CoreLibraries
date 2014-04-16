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
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Lays out text written to the underlying text writer.
    /// </summary>
    public class LayoutWriter : FormatWriter
    {
        /// <summary>
        /// The default layout.
        /// </summary>
        [NotNull]
        public static readonly Layout DefaultLayout = new Layout();

        /// <summary>
        /// The current layout
        /// </summary>
        [NotNull]
        private Layout _defaultLayout;

        /// <summary>
        /// The current layout
        /// </summary>
        [NotNull]
        private Layout _layout;

        /// <summary>
        /// The current position.
        /// </summary>
        private int _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutWriter" /> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="layout">The layout.</param>
        /// <param name="startPosition">The start position, if the writer is currently not at the start of a line.</param>
        public LayoutWriter([NotNull] TextWriter writer, [CanBeNull] Layout layout = null, int startPosition = 0, IFormatProvider formatProvider = null)
            : base(writer, formatProvider)
        {
            Contract.Requires(writer != null);
            Contract.Requires(layout != null);
            _defaultLayout = _layout = layout ?? DefaultLayout;
            _position = startPosition;
        }

        /// <summary>
        /// Gets the current horizontal position.
        /// </summary>
        /// <value>The position.</value>
        [PublicAPI]
        public int Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        /// <value>The layout.</value>
        [PublicAPI]
        [NotNull]
        public Layout Layout
        {
            get { return _layout; }
            set
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse, HeuristicUnreachableCode
                if (value == null) value = _defaultLayout;
                if (_layout == value) return;
                using (Lock.LockAsync().Result)
                    _layout = value;
            }
        }

        /// <summary>
        /// Gets a string represent of each chunk to write.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="chunk">The chunk.</param>
        /// <returns>A string representation of the chunk; otherwise <see langword="null"/> to skip.</returns>
        [CanBeNull]
        protected override string DoWriteChunk(IFormatProvider formatProvider, FormatChunk chunk)
        {
            // TODO Layout chunks...
            return base.DoWriteChunk(formatProvider, chunk);
        }
    }
}