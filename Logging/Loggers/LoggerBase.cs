#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging.Interfaces;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    ///   A logger base class that implements log storage and retrieval.
    /// </summary>
    public abstract class LoggerBase : ILogger
    {
        private LoggingLevels _validLevels;

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoggerBase"/> class.
        /// </summary>
        /// <param name="name">The logger name.</param>
        /// <param name="canRetrieve">Whether the logger can <see cref="LoggerBase.CanRetrieve">retrieve</see> historic logs.</param>
        /// <param name="validLevels">The valid log levels.</param>
        protected LoggerBase([NotNull] string name, bool canRetrieve, LoggingLevels validLevels)
        {
            CanRetrieve = canRetrieve;
            Name = name;
            ValidLevels = validLevels;
        }

        #region ILogger Members
        /// <summary>
        ///   A <see cref="bool"/> value indicating whether the logger can retrieve historic logs.
        /// </summary>
        /// <value>
        ///   Returns <see langword="true"/> if the logger can retrieve historic logs; otherwise returns <see langword="false"/>.
        /// </value>
        public virtual bool CanRetrieve { get; private set; }

        /// <summary>
        ///   The valid <see cref="LoggingLevels"/> for this logger
        /// </summary>
        /// <value>The valid log levels.</value>
        public LoggingLevels ValidLevels
        {
            get { return _validLevels; }
            set
            {
                if (_validLevels == value)
                    return;
                _validLevels = value;
                Log.RecalculateLoggedLevels();
            }
        }

        /// <summary>
        ///   Gets the logger name.
        /// </summary>
        /// <value>The logger name.</value>
        [NotNull]
        public string Name { get; private set; }

        /// <summary>
        ///   Adds the specified log to storage in time order.
        /// </summary>
        /// <param name="log">The log to add to storage.</param>
        [NotNull]
        public abstract void Add([NotNull] Log log);

        /// <summary>
        ///   Adds the specified logs to storage in time order.
        ///   This can be overridden in the default class to bulk store items in one call (e.g. to a database),
        ///   the inbuilt logger will always use this method where possible for efficiency.
        ///   By default it calls the standard Add method repeatedly.
        /// </summary>
        /// <param name="logs">The logs to add to storage.</param>
        [NotNull]
        public virtual void Add([NotNull] IEnumerable<Log> logs)
        {
            foreach (Log l in logs)
                Add(l);
        }

        /// <summary>
        ///   Gets all of the logs from the end date backwards up to the specified limit.
        /// </summary>
        /// <param name="endDate">The last date to get logs up to (exclusive).</param>
        /// <param name="limit">The maximum number of logs to retrieve.</param>
        /// <returns>
        ///   The retrieved logs in reverse date order, the first being the newest log before the <paramref name="endDate"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   <para>The logger does not implement Get.</para>
        ///   <para>-or-</para>
        ///   <para>The logger does not support <see cref="LoggerBase.CanRetrieve">log retrieval</see>.</para>
        /// </exception>
        [NotNull]
        public virtual IEnumerable<Log> Get(DateTime endDate, int limit)
        {
            if (CanRetrieve)
            {
                throw new LoggingException(Resources.LoggerBase_GetEndDateLimit_NotImplemented, LoggingLevel.Critical, Name);
            }
            throw new LoggingException(Resources.LoggerBase_DoesNotSupportRetrieval, LoggingLevel.Critical, Name);
        }

        /// <summary>
        ///   Gets all of the logs from the end date backwards up to the start date.
        /// </summary>
        /// <param name="endDate">The last date to get logs from (exclusive).</param>
        /// <param name="startDate">The start date to get logs up to (inclusive).</param>
        /// <returns>
        ///   The retrieved logs in reverse date order, the first being the newest log before the <paramref name="endDate"/>
        ///   and the last being the first log after the <paramref name="startDate"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   <para>The logger does not implement Get.</para>
        ///   <para>-or-</para>
        ///   <para>The logger does not support <see cref="LoggerBase.CanRetrieve">log retrieval</see>.</para>
        /// </exception>
        [NotNull]
        public virtual IEnumerable<Log> Get(DateTime endDate, DateTime startDate)
        {
            if (CanRetrieve)
            {
                throw new LoggingException(Resources.LoggerBase_GetEndDateStartDate_NotImplemented,
                    LoggingLevel.Critical, Name);
            }
            throw new LoggingException(Resources.LoggerBase_DoesNotSupportRetrieval, LoggingLevel.Critical, Name);
        }

        /// <summary>
        ///   Gets all of the logs from the start date forwards up to the specified limit.
        /// </summary>
        /// <param name="startDate">The start date to get logs from (inclusive).</param>
        /// <param name="limit">The maximum number of logs to retrieve.</param>
        /// <returns>
        ///   The retrieved logs starting from the <paramref name="startDate"/> up to the specified <paramref name="limit"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   <para>The logger does not implement Get.</para>
        ///   <para>-or-</para>
        ///   <para>The logger does not support <see cref="LoggerBase.CanRetrieve">log retrieval</see>.</para>
        /// </exception>
        [NotNull]
        public virtual IEnumerable<Log> GetForward(DateTime startDate, int limit)
        {
            if (CanRetrieve)
            {
                throw new LoggingException(Resources.LoggerBase_GetForwardStartDateLimit_NotImplemented,
                    LoggingLevel.Critical, Name);
            }
            throw new LoggingException(Resources.LoggerBase_DoesNotSupportRetrieval, LoggingLevel.Critical, Name);
        }

        /// <summary>
        ///   Gets all of the logs from the start date forwards up to the end date.
        ///   If not overridden, and <see cref="LoggerBase.CanRetrieve">retrieval</see> is supported,
        ///   this will reverse the results of the normal get method.
        /// </summary>
        /// <param name="startDate">The start date to get logs from (inclusive).</param>
        /// <param name="endDate">The last date to get logs to (exclusive).</param>
        /// <returns>
        ///   All of the retrieved logs from the <paramref name="startDate"/> to the <paramref name="endDate"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   <para>The logger does not implement Get.</para>
        ///   <para>-or-</para>
        ///   <para>The logger does not support <see cref="LoggerBase.CanRetrieve">log retrieval</see>.</para>
        /// </exception>
        [NotNull]
        public virtual IEnumerable<Log> GetForward(DateTime startDate, DateTime endDate)
        {
            if (!CanRetrieve)
                throw new LoggingException(Resources.LoggerBase_DoesNotSupportRetrieval,
                    LoggingLevel.Critical, Name);

            return Get(endDate, startDate).Reverse();
        }

        /// <summary>
        ///   Flushes the logger, this should be overridden in classes that require Flush logic.
        /// </summary>
        public virtual void Flush()
        {
            // Default action is to do nothing.
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            // Default action is to do nothing.
        }
        #endregion
    }
}