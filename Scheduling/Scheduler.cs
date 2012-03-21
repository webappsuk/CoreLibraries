using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Scheduling.Schedulable;
using WebApplications.Utilities.Scheduling.Scheduled;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Implements a scheduler
    /// </summary>
    /// <remarks></remarks>
    [Export(typeof(IScheduler))]
    public class Scheduler : IScheduler
    {
        /// <summary>
        /// False result.
        /// </summary>
        [NotNull]
        internal static readonly Task<bool> FalseResult = TaskEx.FromResult(false);

        /// <summary>
        /// Holds all scheduled actions.
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<CombGuid, ScheduledAction> _actions = new ConcurrentDictionary<CombGuid, ScheduledAction>();

        /// <summary>
        /// Holds constructors for creating type specific scheduled function objects.
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<Type, Func<IScheduler, ISchedule, ISchedulableAction, SchedulableActionInfo, int, ScheduledAction>> _scheduledFunctionConstructors = new ConcurrentDictionary<Type, Func<IScheduler, ISchedule, ISchedulableAction, SchedulableActionInfo, int, ScheduledAction>>();

        /// <inheritdoc/>
        public IScheduledAction Add(ISchedule schedule, ISchedulableAction action, int maximumHistory = -1)
        {
            // Get info.
            SchedulableActionInfo info = SchedulableActionInfo.Get(action);
            ScheduledAction scheduledAction;
            if (!info.IsFunction)
            {
                // Create action
                scheduledAction = new ScheduledAction(this, schedule, action, info, maximumHistory < 1 ? DefaultMaximumHistory : maximumHistory);
            }
            else
            {
                Type t = action.GetType();
                while (t != null)
                {
                    if (t.IsGenericType &&
                        t.GetGenericTypeDefinition() == typeof(ISchedulableFunction<>))
                        break;
                    t = t.BaseType;
                }

                // Sanity check - should never happen.
                if (t == null)
                    throw new InvalidOperationException("The action of type '{0}' does not implement the ISchedulableFunction<> interface.");

                // Get the function return type.
                Type functionType = t.GetGenericArguments()[0];
                Debug.Assert(functionType != null);

                // Need to create relevant generic function type.
                Func<IScheduler, ISchedule, ISchedulableAction, SchedulableActionInfo, int, ScheduledAction> constructor =
                    _scheduledFunctionConstructors.GetOrAdd(
                        functionType,
                        ft =>
                            {
                                Type sfType = typeof (ScheduledFunction<>).MakeGenericType(ft);
                                return
                                    sfType.ConstructorFunc
                                        <IScheduler, ISchedule, ISchedulableAction, SchedulableActionInfo, int,
                                            ScheduledAction>
                                        ();
                            });
                Debug.Assert(constructor != null);

                scheduledAction = constructor(this, schedule, action, info, maximumHistory < 1 ? DefaultMaximumHistory : maximumHistory);
            }
            Debug.Assert(scheduledAction != null);

            // Update actions dictionary
            _actions.AddOrUpdate(scheduledAction.ID, scheduledAction, (i, a) => scheduledAction);

            return scheduledAction;
        }

        /// <inheritdoc/>
        public IScheduledFunction<T> Add<T>(ISchedule schedule, ISchedulableFunction<T> function, int maximumHistory = -1)
        {
            return (IScheduledFunction<T>)Add(schedule, (ISchedulableAction)function, maximumHistory);
        }

        /// <inheritdoc/>
        public bool Remove(IScheduledAction scheduledAction)
        {
            ScheduledAction action = scheduledAction as ScheduledAction;
            if (action == null)
                return false;
            ScheduledAction a;
            bool result = _actions.TryRemove(action.ID, out a);
            Debug.Assert(!result || a.ID == action.ID);
            return result;
        }

        /// <inheritdoc/>
        public bool Execute(IScheduledAction scheduledAction)
        {
            return Enabled && scheduledAction.Execute();
        }

        /// <inheritdoc/>
        public Task<bool> ExecuteAsync(IScheduledAction scheduledAction)
        {
            return !Enabled ? FalseResult : scheduledAction.ExecuteAsync();
        }

        /// <inheritdoc/>
        public bool Enabled { get; set; }

        /// <inheritdoc/>
        public int DefaultMaximumHistory { get; set; }
    }
}