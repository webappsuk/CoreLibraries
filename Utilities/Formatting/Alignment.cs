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

using System.ComponentModel;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Describes <see cref="Layout"/> alignments.
    /// </summary>
    [PublicAPI]
    public enum Alignment
    {
        /// <summary>
        /// The none alignment preserves spaces at the start of a line.
        /// </summary>
        [Description("The none alignment preserves spaces at the start of a line.")]
        [PublicAPI]
        None,

        /// <summary>
        /// The left alignment removes spaces at the start of a line.
        /// </summary>
        [Description("The left alignment removes spaces at the start of a line.")]
        [PublicAPI]
        Left,

        /// <summary>
        /// The centre alignment centres the text on each line.
        /// </summary>
        [Description("The centre alignment centres the text on each line.")]
        [PublicAPI]
        Centre,

        /// <summary>
        /// The right alignment removes spaces at the end of a line.
        /// </summary>
        [Description("The right alignment removes spaces at the end of a line.")]
        [PublicAPI]
        Right,

        /// <summary>
        /// The justify alignment removes spaces at the start and end of each line, and expands space inside the line.
        /// </summary>
        [Description(
            "The justify alignment removes spaces at the start and end of each line, and expands space inside the line."
            )]
        [PublicAPI]
        Justify
    }
}