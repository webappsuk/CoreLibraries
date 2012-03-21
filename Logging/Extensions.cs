#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: Extensions.cs
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

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   Extension methods for logging.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///   Returns a <see cref="bool"/> value indicating whether the specified
        ///   <see cref="WebApplications.Utilities.Logging.LogLevel">log level</see> is within the valid
        ///   <see cref="LogLevels">levels</see>.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="validLevels">The valid levels.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified <paramref name="level"/> is within the <paramref name="validLevels"/>;
        ///   provided; otherwise returns <see langword="false"/>.
        /// </returns>
        public static bool IsValid(this LogLevel level, LogLevels validLevels)
        {
            LogLevels l = (LogLevels) level;
            return l == (l & validLevels);
        }
    }
}