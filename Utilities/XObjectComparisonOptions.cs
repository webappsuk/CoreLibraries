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

using System;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Comparison options for <see cref="XNodeExtensions.DeepEquals" />.
    /// </summary>
    [Flags]
    [PublicAPI]
    public enum XObjectComparisonOptions
    {
        /// <summary>
        /// Everything is considered significant.
        /// </summary>
        None = 0,

        /// <summary>
        /// The attribute order is not considered significant.
        /// </summary>
        IgnoreAttributeOrder = 1 << 0,
        /// <summary>
        /// Attributes that occur in the second node but don't occur in the first node are ignored (Also sets <see cref="IgnoreAttributeOrder"/>).
        /// </summary>
        IgnoreAdditionalAttributes = IgnoreAttributeOrder | 1 << 1,
        /// <summary>
        /// All attributes are ignored.
        /// </summary>
        IgnoreAttributes = 1 << 2,

        /// <summary>
        /// The child element order is not considered significant.
        /// </summary>
        IgnoreElementOrder = 1 << 3,
        /// <summary>
        /// Child elements that occur in the second node but don't occur in the first node are ignored (Also sets <see cref="IgnoreElementOrder"/>).
        /// </summary>
        IgnoreAdditionalElements = IgnoreElementOrder | 1 << 4,
        /// <summary>
        /// All child elements are not considered.
        /// </summary>
        IgnoreElements = 1 << 5,

        /// <summary>
        /// The comment order is not considered significant.
        /// </summary>
        IgnoreCommentOrder = 1 << 6,
        /// <summary>
        /// Comments that occur in the second node but don't occur in the first node are ignored (Also sets <see cref="IgnoreCommentOrder"/>).
        /// </summary>
        IgnoreAdditionalComments = IgnoreCommentOrder | 1 << 7,
        /// <summary>
        /// All comments are ignored
        /// </summary>
        IgnoreComments = 1 << 8,

        /// <summary>
        /// The text/CDATA order is not considered significant.
        /// </summary>
        IgnoreTextOrder = 1 << 9,
        /// <summary>
        /// Text/CDATA that occurs in the second node but doesn't occur in the first node are ignored (Also sets <see cref="IgnoreTextOrder"/>).
        /// </summary>
        IgnoreAdditionalText = IgnoreTextOrder | 1 << 10,
        /// <summary>
        /// All text/CDATA that occurs outside of child elements is ignored, but text in nodes without child elements is considered.
        /// </summary>
        IgnoreTextOutsideOfChildren = 1 << 11,
        /// <summary>
        /// All text/CDATA is ignored
        /// </summary>
        IgnoreText = 1 << 12,

        /// <summary>
        /// The order of processing instructions is ignored.
        /// </summary>
        IgnoreProcessingInstructionOrder = 1 << 13,
        /// <summary>
        /// Processing instructions that occur in the second node but don't occur in the first node are ignored (Also sets <see cref="IgnoreProcessingInstructionOrder"/>).
        /// </summary>
        IgnoreAdditionalProcessingInstructions = IgnoreProcessingInstructionOrder | 1 << 14,
        /// <summary>
        /// All processing instructions are ignored.
        /// </summary>
        IgnoreProcessingInstructions = 1 << 15,
        
        /// <summary>
        /// Document types that occur in the second node but don't occur in the first node are ignored .
        /// </summary>
        IgnoreAdditionalDocumentTypes = 1 << 16,
        /// <summary>
        /// All document types  are ignored.
        /// </summary>
        IgnoreDocumentTypes = 1 << 17,

        /// <summary>
        /// The default options.
        /// </summary>
        Default = IgnoreComments | IgnoreProcessingInstructions | IgnoreDocumentTypes | IgnoreTextOutsideOfChildren | IgnoreAttributeOrder,

        /// <summary>
        /// The order of elements is ignored.
        /// </summary>
        Normalised = IgnoreAttributeOrder | IgnoreElementOrder | IgnoreCommentOrder | IgnoreTextOrder | IgnoreProcessingInstructionOrder,

        /// <summary>
        /// Order does not matter, text outside of child nodes, comments, processing instructions and document types are all ignored.
        /// </summary>
        Loose = Normalised | Default,

        /// <summary>
        /// The order of elements is ignored, and additional nodes are ignored.
        /// </summary>
        IgnoreAdditional = IgnoreAdditionalAttributes | IgnoreAdditionalElements | IgnoreAdditionalComments | IgnoreAdditionalText | IgnoreAdditionalProcessingInstructions | IgnoreAdditionalDocumentTypes,

        /// <summary>
        /// The legacy option is most similar to <see cref="XNode.DeepEquals"/>.
        /// </summary>
        Legacy = IgnoreComments | IgnoreProcessingInstructions | IgnoreDocumentTypes,

        /// <summary>
        /// The semantic mode ensures that the second node is effectively compatible with the first node, i.e. it 
        /// contains all the nodes in the first, but not necessarily in the same order, additonal nodes are allowed,
        /// and insignificant nodes are ignored.
        /// </summary>
        Semantic = IgnoreAdditional | Default,

        /// <summary>
        /// Everything is considered significant.
        /// </summary>
        Exact = None
    }
}