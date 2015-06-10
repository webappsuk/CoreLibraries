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
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using NodaTime;
using NodaTime.TimeZones;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Helper methods for working with NodaTime objects.
    /// </summary>
    [PublicAPI]
    public static class TimeHelpers
    {
        /// <summary>
        /// The current date time zone provider.
        /// </summary>
        [NotNull]
        private static IDateTimeZoneProvider _dateTimeZoneProvider;

        /// <summary>
        /// Whether <see cref="_dateTimeZoneProvider"/> was loaded from the config.
        /// </summary>
        private static bool _isFromConfig;

        [NotNull]
        private static IClock _clock;

        /// <summary>
        /// A constant used to specify an infinite waiting period, for methods that accept a <see cref="Duration"/> parameter.
        /// </summary>
        [PublicAPI]
        public static readonly Duration InfiniteDuration = Duration.FromMilliseconds(Timeout.Infinite);

        /// <summary>
        /// The one tick <see cref="Duration"/>.
        /// </summary>
        [PublicAPI]
        public static readonly Duration OneTick = Duration.FromTicks(1);

        /// <summary>
        /// The one millisecond <see cref="Duration"/>.
        /// </summary>
        [PublicAPI]
        public static readonly Duration OneMillisecond = Duration.FromMilliseconds(1);

        /// <summary>
        /// The one second <see cref="Duration"/>.
        /// </summary>
        [PublicAPI]
        public static readonly Duration OneSecond = Duration.FromSeconds(1);

        /// <summary>
        /// The one minute <see cref="Duration"/>.
        /// </summary>
        [PublicAPI]
        public static readonly Duration OneMinute = Duration.FromMinutes(1);

        /// <summary>
        /// The one hour <see cref="Duration"/>.
        /// </summary>
        [PublicAPI]
        public static readonly Duration OneHour = Duration.FromHours(1);

        /// <summary>
        /// The one standard day <see cref="Duration"/>.
        /// </summary>
        [PublicAPI]
        public static readonly Duration OneStandardDay = Duration.FromStandardDays(1);

        /// <summary>
        /// The one standard week <see cref="Duration"/>.
        /// </summary>
        [PublicAPI]
        public static readonly Duration OneStandardWeek = Duration.FromStandardWeeks(1);

        /// <summary>
        /// The file time epoch, 12:00 A.M. January 1, 1601.
        /// </summary>
        public static readonly Instant FileTimeEpoch = Instant.FromUtc(1601, 1, 1, 0, 0);

        /// <summary>
        /// Initializes the <see cref="TimeHelpers"/> class.
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized - _dateTimeZoneProvider is set by SetDateTimeZoneProvider
        static TimeHelpers()
        {
            UtilityConfiguration.Changed += OnUtilityConfigurationChanged;

            SetDateTimeZoneProvider();

            // ReSharper disable once AssignNullToNotNullAttribute
            _clock = SystemClock.Instance;
        }

        /// <summary>
        /// Called when the utility configuration changes. If the <see cref="UtilityConfiguration.TimeZoneDB"/> property changes, the database will be reloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="UtilityConfiguration.ConfigurationChangedEventArgs"/> instance containing the event data.</param>
        private static void OnUtilityConfigurationChanged(
            [NotNull] object sender,
            [NotNull] ConfigurationSection<UtilityConfiguration>.ConfigurationChangedEventArgs e)
        {
            if (_isFromConfig &&
                !string.Equals(
                    e.NewConfiguration.TimeZoneDB,
                    e.OldConfiguration.TimeZoneDB,
                    StringComparison.InvariantCulture))
                SetDateTimeZoneProvider();
        }

        /// <summary>
        /// Gets an <see cref="Instant"/> from file time ticks.
        /// </summary>
        /// <param name="fileTimeTicks">The number of 100-nanosecond intervals that have elapsed since <see cref="FileTimeEpoch"/>.</param>
        /// <returns></returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Instant InstantFromFileTimeUtc(long fileTimeTicks)
        {
            return FileTimeEpoch.PlusTicks(fileTimeTicks);
        }

        /// <summary>
        /// Gets or sets the date time zone provider.
        /// </summary>
        /// <value>The date time zone provider.</value>
        [NotNull]
        [PublicAPI]
        public static IDateTimeZoneProvider DateTimeZoneProvider
        {
            get
            {
                Contract.Ensures(Contract.Result<IDateTimeZoneProvider>() != null);
                return _dateTimeZoneProvider;
            }
            set
            {
                Contract.Requires(value != null);
                _dateTimeZoneProvider = value;
                _isFromConfig = false;
            }
        }

        /// <summary>
        /// Gets or sets the clock.
        /// </summary>
        /// <value>The clock.</value>
        [NotNull]
        [PublicAPI]
        public static IClock Clock
        {
            get
            {
                Contract.Ensures(Contract.Result<IClock>() != null);
                return _clock;
            }
            set
            {
                Contract.Requires(value != null);
                _clock = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="DateTimeZoneProvider"/> to the time zone database given.
        /// </summary>
        /// <param name="path">The path of the database file to load, or <see langword="null"/> to use the path in the configuration.
        /// If no path is given in the config, the default NodaTime <see cref="DateTimeZoneProviders.Tzdb"/> will be used.</param>
        /// <returns></returns>
        [PublicAPI]
        public static void SetDateTimeZoneProvider(string path = null)
        {
            _dateTimeZoneProvider = LoadTzdb(path);
            _isFromConfig = path == null;
        }

        /// <summary>
        /// Loads a time zone database.
        /// </summary>
        /// <param name="path">The path of the database file to load, or <see langword="null"/> to use the path in the configuration.
        /// If no path is given in the config, the default NodaTime <see cref="DateTimeZoneProviders.Tzdb"/> will be used.</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static IDateTimeZoneProvider LoadTzdb(string path = null)
        {
            IDateTimeZoneProvider provider;

            // Load the time zone database from a file, if specified.
            path = path ?? UtilityConfiguration.Active.TimeZoneDB;
            if (!string.IsNullOrWhiteSpace(path))
            {
                Uri uri = new Uri(path, UriKind.RelativeOrAbsolute);
                if (!uri.IsAbsoluteUri)
                    uri = new Uri(UtilityExtensions.AppDomainBaseUri, uri);

                // If the URI is a file, load it from the file system, otherwise download it
                if (uri.IsFile)
                {
                    path = uri.LocalPath;
                    if (!File.Exists(path))
                        throw new FileNotFoundException(
                            // ReSharper disable once AssignNullToNotNullAttribute
                            string.Format(Resources.TimeHelper_TimeHelper_TimeZoneDB_Not_Found, path));

                    try
                    {
                        using (FileStream stream = File.OpenRead(path))
                            // ReSharper disable once AssignNullToNotNullAttribute
                            provider = new DateTimeZoneCache(TzdbDateTimeZoneSource.FromStream(stream));
                    }
                    catch (Exception e)
                    {
                        // ReSharper disable once AssignNullToNotNullAttribute
                        throw new FileLoadException(
                            string.Format(Resources.TimeHelper_TimeHelper_TimeZoneDB_Failed, path),
                            e);
                    }
                }
                else
                {
                    path = uri.AbsoluteUri;
                    try
                    {
                        // ReSharper disable AssignNullToNotNullAttribute
                        WebRequest request = WebRequest.Create(uri);
                        using (WebResponse response = request.GetResponse())
                        using (Stream stream = response.GetResponseStream())
                            provider = new DateTimeZoneCache(TzdbDateTimeZoneSource.FromStream(stream));
                        // ReSharper restore AssignNullToNotNullAttribute
                    }
                    catch (Exception e)
                    {
                        // ReSharper disable once AssignNullToNotNullAttribute
                        throw new FileLoadException(
                            string.Format(Resources.TimeHelper_TimeHelper_TimeZoneDB_Failed, path),
                            e);
                    }
                }
            }
            // ReSharper disable once AssignNullToNotNullAttribute
            else
                // ReSharper disable once AssignNullToNotNullAttribute
                provider = DateTimeZoneProviders.Tzdb;

            Contract.Assert(provider != null);

            return provider;
        }

        #region Duration
        /// <summary>
        /// Gets the value of the <see cref="Duration"/> expressed in whole and fractional milliseconds.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static double TotalMilliseconds(this Duration duration)
        {
            return (double)duration.Ticks / NodaConstants.TicksPerMillisecond;
        }

        /// <summary>
        /// Gets the milliseconds component of the <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static int Milliseconds(this Duration duration)
        {
            return (int)(duration.Ticks / NodaConstants.TicksPerMillisecond) % 1000;
        }

        /// <summary>
        /// Gets the value of the <see cref="Duration"/> expressed in whole and fractional seconds.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static double TotalSeconds(this Duration duration)
        {
            return (double)duration.Ticks / NodaConstants.TicksPerSecond;
        }

        /// <summary>
        /// Gets the seconds component of the <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static int Seconds(this Duration duration)
        {
            return (int)(duration.Ticks / NodaConstants.TicksPerSecond) % 60;
        }

        /// <summary>
        /// Gets the value of the <see cref="Duration"/> expressed in whole and fractional minutes.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static double TotalMinutes(this Duration duration)
        {
            return (double)duration.Ticks / NodaConstants.TicksPerMinute;
        }

        /// <summary>
        /// Gets the minutes component of the <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static int Minutes(this Duration duration)
        {
            return (int)(duration.Ticks / NodaConstants.TicksPerMinute) % 60;
        }

        /// <summary>
        /// Gets the value of the <see cref="Duration"/> expressed in whole and fractional hours.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static double TotalHours(this Duration duration)
        {
            return (double)duration.Ticks / NodaConstants.TicksPerHour;
        }

        /// <summary>
        /// Gets the hours component of the <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static int Hours(this Duration duration)
        {
            return (int)(duration.Ticks / NodaConstants.TicksPerHour) % 24;
        }

        /// <summary>
        /// Gets the value of the <see cref="Duration"/> expressed in whole and fractional standard days.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static double TotalStandardDays(this Duration duration)
        {
            return (double)duration.Ticks / NodaConstants.TicksPerStandardDay;
        }

        /// <summary>
        /// Gets the standard days component of the <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static int StandardDays(this Duration duration)
        {
            return (int)(duration.Ticks / NodaConstants.TicksPerStandardDay);
        }

        /// <summary>
        /// Gets the value of the <see cref="Duration"/> expressed in whole and fractional standard days.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static double TotalStandardWeeks(this Duration duration)
        {
            return (double)duration.Ticks / NodaConstants.TicksPerStandardWeek;
        }

        /// <summary>
        /// Gets the standard weeks component of the <see cref="Duration"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>System.Double.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static int StandardWeeks(this Duration duration)
        {
            return (int)(duration.Ticks / NodaConstants.TicksPerStandardWeek);
        }
        #endregion

        #region Floor/Ceiling
        /// <summary>
        /// Floors the specified instant to the second.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <returns>Instant.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static Instant FloorSecond(this Instant instant)
        {
            return new Instant((instant.Ticks / NodaConstants.TicksPerSecond) * NodaConstants.TicksPerSecond);
        }

        /// <summary>
        /// Ceilings the specified instant to the second.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <returns>Instant.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static Instant CeilingSecond(this Instant instant)
        {
            return new Instant(
                ((instant.Ticks + NodaConstants.TicksPerSecond - 1) / NodaConstants.TicksPerSecond) *
                NodaConstants.TicksPerSecond);
        }

        /// <summary>
        /// Floors the <see cref="Instant"/> to the nearest minute.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <returns>Instant.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static Instant FloorMinute(this Instant instant)
        {
            return new Instant((instant.Ticks / NodaConstants.TicksPerMinute) * NodaConstants.TicksPerMinute);
        }

        /// <summary>
        /// Ceilings the <see cref="Instant"/> to the nearest minute.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <returns>Instant.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static Instant CeilingMinute(this Instant instant)
        {
            return new Instant(
                ((instant.Ticks * NodaConstants.TicksPerMinute - 1) / NodaConstants.TicksPerMinute) *
                NodaConstants.TicksPerMinute);
        }

        /// <summary>
        /// Floors the <see cref="Instant"/> to the nearest hour.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <returns>Instant.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static Instant FloorHour(this Instant instant)
        {
            return new Instant((instant.Ticks / NodaConstants.TicksPerHour) * NodaConstants.TicksPerHour);
        }

        /// <summary>
        /// Ceilings the <see cref="Instant"/> to the nearest hour.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <returns>Instant.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static Instant CeilingHour(this Instant instant)
        {
            return new Instant(
                ((instant.Ticks * NodaConstants.TicksPerHour - 1) / NodaConstants.TicksPerHour) *
                NodaConstants.TicksPerHour);
        }
        #endregion

        #region Periods
        /// <summary>
        /// Determines whether the specified period is zero.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <returns></returns>
        [PublicAPI]
        public static bool IsZero([NotNull] this Period period)
        {
            Contract.Requires(period != null);

            return period.Ticks == 0 &&
                   period.Milliseconds == 0 &&
                   period.Seconds == 0 &&
                   period.Minutes == 0 &&
                   period.Hours == 0 &&
                   period.Days == 0 &&
                   period.Weeks == 0 &&
                   period.Months == 0 &&
                   period.Years == 0;
        }

        /// <summary>
        /// Determines whether the specified period is positive, relative to a <see cref="LocalDateTime" />.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="local">The local.</param>
        /// <returns></returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static bool IsPositive([NotNull] this Period period, LocalDateTime local)
        {
            Contract.Requires(period != null);

            return (local + period) > local;
        }

        /// <summary>
        /// Determines whether the specified period is negative, relative to a <see cref="LocalDateTime" />.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="local">The local.</param>
        /// <returns></returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static bool IsNegative([NotNull] this Period period, LocalDateTime local)
        {
            Contract.Requires(period != null);

            return (local + period) < local;
        }

        /// <summary>
        /// Determines whether the specified period is positive, relative to a <see cref="LocalDateTime" />.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="local">The local.</param>
        /// <returns></returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static bool IsPositive([NotNull] this Period period, LocalDate local)
        {
            Contract.Requires(period != null);

            return (local + period) > local;
        }

        /// <summary>
        /// Determines whether the specified period is negative, relative to a <see cref="LocalDateTime" />.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="local">The local.</param>
        /// <returns></returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        public static bool IsNegative([NotNull] this Period period, LocalDate local)
        {
            Contract.Requires(period != null);

            return (local + period) < local;
        }
        #endregion
    }
}