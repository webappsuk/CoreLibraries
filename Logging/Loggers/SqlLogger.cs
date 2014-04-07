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

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    /// Allows logging to a database.
    /// TODO This still needs to be implemented!
    /// </summary>
#if false
    [PublicAPI]
    public class SqlLogger : LoggerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLogger" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="validLevels">The valid levels.</param>
        public SqlLogger(
            [NotNull]string name,
            [NotNull]string connectionString,
            LoggingLevels validLevels = LoggingLevels.All)
            : base(name, false, true, validLevels)
        {
            Contract.Requires(name != null);
            Contract.Requires(connectionString != null);
        }

        /// <summary>
        /// Adds the specified logs to storage in batches.
        /// </summary>
        /// <param name="logs">The logs to add to storage.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Task Add(IEnumerable<Log> logs, CancellationToken token = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
#endif
}