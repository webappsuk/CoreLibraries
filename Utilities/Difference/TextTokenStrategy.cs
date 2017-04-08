#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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

namespace WebApplications.Utilities.Difference
{
    /// <summary>
    /// Describes the tokenization strategy for strings used by the difference engine.
    /// </summary>
    public enum TextTokenStrategy
    {
        /// <summary>
        /// Differences are calculated per character.
        /// </summary>
        Character,

        /// <summary>
        /// Differences are calculated per word (based on word boundaries).
        /// </summary>
        /// <remarks><para>This follows the Unicode Standard, as shown in the Word Break Chart (see ftp://www.unicode.org/Public/UNIDATA/auxiliary/WordBreakTest.html ). </para></remarks>
        Word,

        /// <summary>
        /// Differences are calculated per line.
        /// </summary>
        /// <remarks><para>This follows the Unicode Standard, as shown in the Line Break Chart (see ftp://www.unicode.org/Public/UNIDATA/auxiliary/LineBreakTest.html ). </para></remarks>
        Line,

        /// <summary>
        /// Differences are calculated per full sentence (ignoring line breaks).
        /// </summary>
        /// <remarks><para>This follows the Unicode Standard, as shown in the Sentence Break Chart (see ftp://www.unicode.org/Public/7.0.0/ucd/auxiliary/SentenceBreakTest.html ),
        /// except it ignores line breaks (i.e. breaks caused by CR, LR or SEP). </para></remarks>
        FullSentence,

        /// <summary>
        /// Differences are calculated per sentence.
        /// </summary>
        /// <remarks><para>This follows the Unicode Standard, as shown in the Sentence Break Chart (see ftp://www.unicode.org/Public/7.0.0/ucd/auxiliary/SentenceBreakTest.html ). </para></remarks>
        Sentence
    }
}