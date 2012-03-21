#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: PerformanceTimer.cs
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