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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;

namespace WebApplications.Utilities.Logging.Performance
{
    /// <summary>
    ///   Times an operation using a performance timer.
    /// </summary>
    public class PerformanceTimer : IDisposable
    {
#if Performance
        private readonly PerformanceTimerHelper _helper;
#endif
        private readonly string _categoryName;
        private readonly Stopwatch _stopwatch;
        private readonly TimeSpan _warningDuration;
        private readonly TimeSpan _criticalDuration;
        private int _stopped;
        private TimeSpan _duration;

        /// <summary>
        ///   Initializes a new instance of the <see cref="PerformanceTimer"/> class.
        /// </summary>
        /// <param name="categoryName">The name of the category.</param>
        public PerformanceTimer(string categoryName)
            : this(categoryName, TimeSpan.MaxValue, TimeSpan.MaxValue)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PerformanceTimer"/> class.
        /// </summary>
        /// <param name="categoryName">The name of the category.</param>
        /// <param name="warningDuration">
        ///   The duration of time that the operation should take before logging a warning.
        /// </param>
        public PerformanceTimer(string categoryName, TimeSpan warningDuration)
            : this(categoryName, warningDuration, TimeSpan.MaxValue)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PerformanceTimer"/> class.
        /// </summary>
        /// <param name="categoryName">The name of the category.</param>
        /// <param name="warningDuration">
        ///   The duration of time that the operation should take before logging a warning.
        /// </param>
        /// <param name="criticalDuration">
        ///   The duration of time that the operation should take before logging an error.
        /// </param>
        public PerformanceTimer(string categoryName, TimeSpan warningDuration, TimeSpan criticalDuration)
        {
            Contract.Requires(warningDuration <= criticalDuration);
            if (warningDuration > criticalDuration)
                criticalDuration = warningDuration;
#if Performance
            _helper = PerformanceTimerHelper.Get(categoryName, warningDuration, criticalDuration);
#else
            if ((warningDuration == TimeSpan.MaxValue) &&
                (criticalDuration == TimeSpan.MaxValue))
                return;
#endif
            _categoryName = categoryName;
            _warningDuration = warningDuration;
            _criticalDuration = criticalDuration;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        /// <summary>
        ///   Stops the <see cref="PerformanceTimer"/>.
        /// </summary>
        public TimeSpan Stop()
        {
            // If the timer was already stopped return the duration.
            if (Interlocked.Exchange(ref _stopped, 1) == 1)
                return _duration;
#if !Performance
            if (this._stopwatch == null)
                return TimeSpan.Zero;
#endif
            _stopwatch.Stop();
            _duration = _stopwatch.Elapsed;

            // Check performance levels
            if (_duration >=
                _warningDuration)
            {
                if (_duration >=
                    _criticalDuration)
                {
                    Log.Add(
                        Resources.PerformanceTimer_Stop_ExceededCriticalLevel,
                        LogLevel.Error,
                        _categoryName,
                        _criticalDuration.TotalMilliseconds,
                        _stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    Log.Add(
                        Resources.PerformanceTimer_Stop_ExceededWarningLevel,
                        LogLevel.Warning,
                        _categoryName,
                        _warningDuration.TotalMilliseconds,
                        _stopwatch.ElapsedMilliseconds);
                }
            }
#if Performance
            _helper.IncrementCounters(_duration);
#endif
            return _duration;
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }
    }
}