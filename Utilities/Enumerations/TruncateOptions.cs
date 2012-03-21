#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: TruncateOptions.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;

namespace WebApplications.Utilities.Enumerations
{
    /// <summary>
    ///   These flags are used to control how a string is truncated in the
    ///   <see cref="WebApplications.Utilities.Extensions.Truncate">Truncate</see> extension method.
    /// </summary>
    [Flags]
    public enum TruncateOptions
    {
        /// <summary>
        ///   Use the default functionality.
        /// </summary>
        None = 0x0,

        /// <summary>
        ///   Do not allow words to be cut off midway through.
        /// </summary>
        FinishWord = 0x1,

        /// <summary>
        ///   Allow the last word to go over the specified maximum length.
        /// </summary>
        AllowLastWordToGoOverMaxLength = 0x2,

        /// <summary>
        ///   Include an ellipsis character at the end of the result.
        /// </summary>
        IncludeEllipsis = 0x4
    }
}