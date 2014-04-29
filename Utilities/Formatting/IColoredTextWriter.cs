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

using System.Drawing;
using System.IO;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Interface that indicates a <see cref="TextWriter"/> supports color.
    /// </summary>
    public interface IColoredTextWriter : ITextWriter
    {
        /// <summary>
        /// Resets the foreground and background colors of the writer.
        /// </summary>
        [PublicAPI]
        void ResetColors();

        /// <summary>
        /// Resets the foreground color of the writer.
        /// </summary>
        [PublicAPI]
        void ResetForegroundColor();

        /// <summary>
        /// Sets the foreground color of the writer.
        /// </summary>
        /// <param name="color">The color.</param>
        [PublicAPI]
        void SetForegroundColor(Color color);

        /// <summary>
        /// Sets the background color of the writer.
        /// </summary>
        [PublicAPI]
        void ResetBackgroundColor();

        /// <summary>
        /// Sets the background color of the writer.
        /// </summary>
        /// <param name="color">The color.</param>
        [PublicAPI]
        void SetBackgroundColor(Color color);
    }
}