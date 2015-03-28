using System;
using System.Diagnostics;
using System.Threading;
using WebApplications.Utilities.Annotations;

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
        /// The <see cref="DateTime"/> ticks per millisecond.
        /// </summary>
        [PublicAPI]
        public const long TicksPerMillisecond = 10000;

        /// <summary>
        /// The <see cref="DateTime"/> ticks per second.
        /// </summary>
        [PublicAPI]
        public const long TicksPerSecond = TicksPerMillisecond * 1000;

        /// <summary>
        /// Delegate to call on disposal.
        /// </summary>
        [CanBeNull]
        private RegionTimerDisposedDelegate _onDisposed;

        /// <summary>
        /// The duration after which the timer has passed a warning level.
        /// </summary>
        [PublicAPI]
        public readonly TimeSpan WarningDuration;

        /// <summary>
        /// The duration after which the timer has passed a critical level.
        /// </summary>
        [PublicAPI]
        public readonly TimeSpan CriticalDuration;

        private readonly long _start;
        private long _stop;

        private static readonly double _tickFrequency;

        /// <summary>
        /// Initializes static members of the <see cref="RegionTimer"/> class.
        /// </summary>
        static RegionTimer()
        {
            _tickFrequency = !Stopwatch.IsHighResolution ? 1.0 : ((double)TicksPerSecond) / Stopwatch.Frequency;
        }

        /// <summary>
        /// Gets a <see cref="TimeSpan"/> from ticks.
        /// </summary>
        /// <returns>System.Int64.</returns>
        private static long GetDateTimeTicks(long ticks)
        {
            if (!Stopwatch.IsHighResolution)
                return ticks;

            // convert high resolution perf counter to DateTime ticks
            double dticks = ticks;
            dticks *= _tickFrequency;
            return unchecked((long)dticks);
        }

        /// <summary>
        /// Gets the raw elapsed CPU ticks.
        /// </summary>
        /// <value>The raw elapsed CPU ticks.</value>
        /// <remarks>This will be different from <see cref="TimeSpan.Ticks">Elapsed.Ticks</see>.</remarks>
        [PublicAPI]
        public long ElapsedTicks
        {
            get
            {
                long stop = Interlocked.Read(ref _stop);
                if (stop == 0) stop = Stopwatch.GetTimestamp();
                return stop - _start;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value><see langword="true" /> if this instance is running; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool IsRunning
        {
            get
            {
                long stop = Interlocked.Read(ref _stop);
                return stop == 0;
            }
        }

        /// <summary>
        /// Gets the elapsed <see cref="TimeSpan"/>.
        /// </summary>
        /// <value>The elapsed <see cref="TimeSpan"/>.</value>
        [PublicAPI]
        public TimeSpan Elapsed
        {
            get { return new TimeSpan(GetDateTimeTicks(ElapsedTicks)); }
        }

        /// <summary>
        /// Gets the elapsed milliseconds.
        /// </summary>
        /// <value>The elapsed milliseconds.</value>
        [PublicAPI]
        public long ElapsedMilliseconds
        {
            get { return GetDateTimeTicks(ElapsedTicks) / TicksPerMillisecond; }
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
            TimeSpan warningDuration = default(TimeSpan),
            TimeSpan criticalDuration = default(TimeSpan))
        {
            _start = Stopwatch.GetTimestamp();
            _onDisposed = onDisposed;

            if (warningDuration == default(TimeSpan))
                warningDuration = TimeSpan.MaxValue;
            if (criticalDuration == default(TimeSpan))
                criticalDuration = TimeSpan.MaxValue;

            if (warningDuration > criticalDuration)
                criticalDuration = warningDuration;

            WarningDuration = warningDuration;
            CriticalDuration = criticalDuration;
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
            Interlocked.CompareExchange(ref _stop, Stopwatch.GetTimestamp(), 0);

            var onDisposed = Interlocked.Exchange(ref _onDisposed, null);
            if (!ReferenceEquals(onDisposed, null))
                onDisposed(this);
        }
    }
}