using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// The scheduler, used to schedule events.
    /// </summary>
    public static class Scheduler
    {
        /// <summary>
        /// The scheduled functions.
        /// </summary>
        private static readonly ConcurrentDictionary<ScheduledFunction, Timer> _timers =
                new ConcurrentDictionary<ScheduledFunction, Timer>();

        /// <summary>
        /// Gets the scheduled functions.
        /// </summary>
        /// <value>The scheduled functions.</value>
        public static IEnumerable<ScheduledFunction> ScheduledFunctions { get { return _timers.Keys; } }

        /// <summary>
        /// Gets the enabled scheduled functions.
        /// </summary>
        /// <value>The scheduled functions.</value>
        public static IEnumerable<ScheduledFunction> EnabledScheduledFunctions { get { return _timers.Where(kvp => kvp.Value != null).Select(kvp => kvp.Key); } }

        /// <summary>
        /// Gets the enabled scheduled functions.
        /// </summary>
        /// <value>The scheduled functions.</value>
        public static IEnumerable<ScheduledFunction> DisabledScheduledFunctions { get { return _timers.Where(kvp => kvp.Value == null).Select(kvp => kvp.Key); } }

        /// <summary>
        /// Adds the specified schedule.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="schedule">The schedule.</param>
        /// <param name="function">The function.</param>
        /// <param name="startAfter">The date time to start after.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <param name="enabled">if set to <c>true</c> then the scheduled function is enabled.</param>
        /// <param name="executeImmediately">if set to <c>true</c> executes the function immediately.</param>
        /// <returns>The scheduled function.</returns>
        public static ScheduledFunction<TResult> Add<TResult>(Schedule schedule,
            Func<ScheduledFunction<TResult>, TResult> function, 
            DateTime? startAfter = null, int maximumHistory = ScheduledFunction.DefaultMaximumHistory,
            bool enabled = true, bool executeImmediately = false)
        {
            ScheduledFunction<TResult> scheduledFunction = new ScheduledFunction<TResult>(
                    schedule, function, startAfter ?? DateTime.Now, maximumHistory);

            // If we need to execute, do so now.
            if (executeImmediately)
                scheduledFunction.ExecuteNow();

            // Don't add a scheduled function that is never due
            DateTime nextDue = scheduledFunction.NextExecutionDue;

            // Check if we're ever due
            if (nextDue == DateTime.MaxValue)
                return scheduledFunction;

            Timer timer = enabled
                               ? new Timer(Tick, scheduledFunction, Timeout.Infinite, Timeout.Infinite)
                               : null;
            _timers.AddOrUpdate(scheduledFunction, timer, (sf, t) => timer);

            if (enabled)
                SetTimer(scheduledFunction);


            return scheduledFunction;
        }

        /// <summary>
        /// Sets the timer for a scheduled function.
        /// </summary>
        /// <param name="scheduledFunction">The scheduled function.</param>
        private static void SetTimer(ScheduledFunction scheduledFunction)
        {
            Timer timer;
            if ((!_timers.TryGetValue(scheduledFunction, out timer)) || (timer == null))
                return;
            DateTime nextDue = scheduledFunction.NextExecutionDue;
            if (nextDue == DateTime.MaxValue)
            {
                // If we're never due again, remove the scheduled function.
                if (_timers.TryRemove(scheduledFunction, out timer) && (timer != null))
                    timer.Dispose();
                return;
            }
            TimeSpan dueIn = nextDue - DateTime.Now;
            timer.Change(dueIn > TimeSpan.Zero ? dueIn : TimeSpan.FromTicks(1), TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Removes the specified scheduled function (if currently in the scheduler).
        /// </summary>
        /// <param name="scheduledFunction">The scheduled function.</param>
        /// <returns>The scheduled function passed in.</returns>
        public static ScheduledFunction Remove(ScheduledFunction scheduledFunction)
        {
            Timer timer;
            if (_timers.TryRemove(scheduledFunction, out timer) && (timer != null))
                timer.Dispose();
            return scheduledFunction;
        }

        /// <summary>
        /// Enables all scheduled functions.
        /// </summary>
        /// <returns></returns>
        public static void EnableAll()
        {
            foreach(ScheduledFunction scheduledFunction in _timers.Select(kvp => kvp.Key))
            {
                if (_timers.TryUpdate(scheduledFunction, new Timer(Tick, scheduledFunction, Timeout.Infinite, Timeout.Infinite), null))
                    SetTimer(scheduledFunction);
            }
        }

        /// <summary>
        /// Disables all scheduled functions.
        /// </summary>
        /// <returns></returns>
        public static void DisableAll()
        {
            foreach (KeyValuePair<ScheduledFunction, Timer> kvp in _timers)
                if (_timers.TryUpdate(kvp.Key, null, kvp.Value) && (kvp.Value != null))
                    kvp.Value.Dispose();
        }

        /// <summary>
        /// Enables the specified scheduled function.
        /// </summary>
        /// <param name="scheduledFunction">The scheduled function.</param>
        public static void Enable(ScheduledFunction scheduledFunction)
        {
            if (_timers.TryUpdate(scheduledFunction, new Timer(Tick, scheduledFunction, Timeout.Infinite, Timeout.Infinite), null))
                SetTimer(scheduledFunction);
        }

        /// <summary>
        /// Enables the specified scheduled function.
        /// </summary>
        /// <param name="scheduledFunction">The scheduled function.</param>
        public static void Disable(ScheduledFunction scheduledFunction)
        {
            Timer timer;
            if ((_timers.TryGetValue(scheduledFunction, out timer)) && (timer != null))
                if (_timers.TryUpdate(scheduledFunction, null, timer))
                    timer.Dispose();
        }

        /// <summary>
        /// Ticks every second.
        /// </summary>
        /// <param name="state">The state.</param>
        private static void Tick(object state)
        {
            ScheduledFunction scheduledFunction = state as ScheduledFunction;
            if (scheduledFunction == null)
                return;

            // Check we're still due.
            if (scheduledFunction.NextExecutionDue <= DateTime.Now)
                scheduledFunction.ExecuteNow();

            SetTimer(scheduledFunction);
        }
    }
}