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

using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;
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
        /// Holds all scheduled actions.
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<CombGuid, ScheduledAction> _actions =
            new ConcurrentDictionary<CombGuid, ScheduledAction>();

        /// <summary>
        /// Holds constructors for creating type specific scheduled function objects.
        /// </summary>
        [NotNull]
        private readonly
            ConcurrentDictionary
                <Type, Func<IScheduler, ISchedule, ISchedulableAction, SchedulableActionInfo, int, ScheduledAction>>
            _scheduledFunctionConstructors =
                new ConcurrentDictionary
                    <Type, Func<IScheduler, ISchedule, ISchedulableAction, SchedulableActionInfo, int, ScheduledAction>>
                    ();

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        /// <param name="defaultMaximumHistory">The default maximum history.</param>
        /// <remarks></remarks>
        public Scheduler(int defaultMaximumHistory = 100)
        {
            DefaultMaximumHistory = defaultMaximumHistory;
        }

        /// <inheritdoc/>
        public IScheduledAction Add(ISchedule schedule, ISchedulableAction action, int maximumHistory = -1)
        {
            // Get info.
            SchedulableActionInfo info = SchedulableActionInfo.Get(action);
            ScheduledAction scheduledAction;
            if (!info.IsFunction)
            {
                // Create action
                scheduledAction = new ScheduledAction(
                    this,
                    schedule,
                    action,
                    info,
                    maximumHistory < 1 ? DefaultMaximumHistory : maximumHistory);
            }
            else
            {
                // Get the function return type.
                Debug.Assert(info.FunctionReturnType != null);

                // Need to create relevant generic function type.
                Func<IScheduler, ISchedule, ISchedulableAction, SchedulableActionInfo, int, ScheduledAction> constructor
                    =
                    _scheduledFunctionConstructors.GetOrAdd(
                        info.FunctionReturnType,
                        ft =>
                        {
                            Type sfType = typeof(ScheduledFunction<>).MakeGenericType(ft);
                            return
                                sfType.ConstructorFunc
                                    <IScheduler, ISchedule, ISchedulableAction, SchedulableActionInfo, int,
                                        ScheduledAction>
                                    ();
                        });
                Debug.Assert(constructor != null);

                scheduledAction = constructor(
                    this,
                    schedule,
                    action,
                    info,
                    maximumHistory < 1 ? DefaultMaximumHistory : maximumHistory);
            }
            Debug.Assert(scheduledAction != null);

            // Update actions dictionary
            _actions.AddOrUpdate(scheduledAction.ID, scheduledAction, (i, a) => scheduledAction);

            return scheduledAction;
        }

        /// <inheritdoc/>
        public IScheduledFunction<T> Add<T>(
            ISchedule schedule,
            ISchedulableFunction<T> function,
            int maximumHistory = -1)
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
            return !Enabled ? TaskResult.False : scheduledAction.ExecuteAsync();
        }

        /// <inheritdoc/>
        public bool Enabled { get; set; }

        private int _defaultMaximumHistory;

        /// <inheritdoc/>
        public int DefaultMaximumHistory
        {
            get { return _defaultMaximumHistory; }
            set
            {
                if (value < 0)
                    throw new LoggingException(()=>Resource.Scheduler_DefaultMaximumHistory_Negative);
                _defaultMaximumHistory = value;
            }
        }
    }
}