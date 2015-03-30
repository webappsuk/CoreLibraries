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
using System.Threading;
using NodaTime;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Performance.Configuration;

namespace WebApplications.Utilities.Performance
{
    /// <summary>
    /// Called when a <see cref="RegionTimer"/> is <see cref="IDisposable.Dispose">disposed</see> (i.e. stopped).
    /// </summary>
    /// <param name="timer">The timer that has been disposed.</param>
    public delegate void RegionTimerDisposedDelegate([NotNull] RegionTimer timer);

    /// <summary>
    /// Used to allow the timing of a region of code for a <see cref="PerfTimer"/>.
    /// </summary>
    public class RegionTimer : IDisposable
    {
        /// <summary>
        /// Delegate to call on disposal.
        /// </summary>
        [CanBeNull]
        private RegionTimerDisposedDelegate _onDisposed;

        /// <summary>
        /// The duration after which the timer has passed a warning level, or <see langword="null"/>.
        /// </summary>
        [PublicAPI]
        public readonly Duration WarningDuration;

        /// <summary>
        /// The duration after which the timer has passed a critical level, or <see langword="null"/>.
        /// </summary>
        [PublicAPI]
        public readonly Duration CriticalDuration;

        /// <summary>
        /// When the timer started.
        /// </summary>
        [PublicAPI]
        public readonly Instant Started;

        private Instant? _stopped;

        /// <summary>
        /// When the timer stopped, if stopped; otherwise <see langword="nulL" />.
        /// </summary>
        /// <value>When the timer stopped.</value>
        [PublicAPI]
        public Instant? Stopped
        {
            get { return _stopped; }
        }

        /// <summary>
        /// How long the timer was running for, or how long since it started if <see cref="IsRunning">the timer is
        /// still running</see>.
        /// </summary>
        /// <value>The elapsed time.</value>
        [PublicAPI]
        public Duration Elapsed
        {
            get
            {
                Instant? stop = _stopped;
                if (!stop.HasValue) stop = HighPrecisionClock.Instance.Now;
                return stop.Value - Started;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value><see langword="true" /> if this instance is running; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool IsRunning
        {
            get { return !_stopped.HasValue; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="RegionTimer" /> has exceeded the <see cref="WarningDuration"/>.
        /// </summary>
        /// <value><see langword="true" /> if warning; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool Warning
        {
            get { return Elapsed > WarningDuration; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="RegionTimer" /> has exceeded the <see cref="CriticalDuration"/>.
        /// </summary>
        /// <value><see langword="true" /> if critical; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool Critical
        {
            get { return Elapsed > CriticalDuration; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionTimer"/> class.
        /// </summary>
        /// <param name="onDisposed">The disposed delegate is called once on disposal.</param>
        /// <param name="warningDuration">Duration of the warning.</param>
        /// <param name="criticalDuration">Duration of the critical.</param>
        public RegionTimer(
            [CanBeNull] RegionTimerDisposedDelegate onDisposed = null,
            Duration? warningDuration = null,
            Duration? criticalDuration = null)
        {
            Started = HighPrecisionClock.Instance.Now;
            _onDisposed = onDisposed;
            WarningDuration = warningDuration ?? PerformanceConfiguration.DefaultWarningDuration;
            CriticalDuration = criticalDuration ?? PerformanceConfiguration.DefaultCriticalDuration;
        }

        /// <summary>
        /// Disposes the timer updating the performance counters the first time this is called.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Following disposal, the public properties of this type remain safely accessible.</para>
        /// </remarks>
        public void Dispose()
        {
            Instant stop = HighPrecisionClock.Instance.Now;
            var onDisposed = Interlocked.Exchange(ref _onDisposed, null);
            if (ReferenceEquals(onDisposed, null)) return;

            _stopped = stop;
            onDisposed(this);
        }
    }
}