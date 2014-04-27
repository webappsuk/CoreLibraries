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
using System.ComponentModel;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Describes the <see cref="Layout" /> line wrapping modes.
    /// </summary>
    [PublicAPI]
    public enum LayoutWrapMode : byte
    {
        /// <summary>
        /// Always adds a new line when wrapping lines.
        /// </summary>
        [PublicAPI]
        [Description("Always adds a new line when wrapping lines.")]
        NewLine,

        /// <summary>
        /// Only adds a new line if the line length is less than <see cref="Layout.Width"/>, 
        /// otherwise it assumes that the writer/display wraps if you reach the width.
        /// </summary>
        [PublicAPI]
        [Description(
            "Only adds a new line if the line length is less than Layout width, otherwise it assumes that the display wraps if you reach the width."
            )]
        NewLineOnShort,

        /// <summary>
        /// Adds indent characters to the end of each line till <see cref="Layout.Width"/> is reached and lets the display handle wrapping.
        /// </summary>
        /// <remarks>This should be used when writing to the <see cref="Console"/> as this wraps when if a line length is equal to the width.
        /// This would cause extra blank lines when using a <see cref="Alignment.Justify">justified</see> <see cref="Layout"/> with no right margin.</remarks>
        [PublicAPI]
        [Description(
            "Adds indent characters to the end of each line till Layout width is reached and lets the display handle wrapping."
            )]
        PadToWrap
    }
}