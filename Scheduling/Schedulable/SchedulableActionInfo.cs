using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling.Schedulable
{
    /// <summary>
    /// Holds information about schedulable action types.
    /// </summary>
    public class SchedulableActionInfo
    {
        /// <summary>
        /// Holds info objects in resolution order.
        /// </summary>
        [NotNull]
        private static readonly IEnumerable<SchedulableActionInfo> _infos =
            new List<SchedulableActionInfo>
                {
                    new SchedulableActionInfo(typeof (ISchedulableFunctionCancellableAsync<>), true, true, true),
                    new SchedulableActionInfo(typeof (ISchedulableActionCancellableAsync), false, true, true),
                    new SchedulableActionInfo(typeof (ISchedulableFunctionAsync<>), true, true, false),
                    new SchedulableActionInfo(typeof (ISchedulableActionAsync), false, true, false),
                    new SchedulableActionInfo(typeof (ISchedulableFunction<>), true, false, false),
                    new SchedulableActionInfo(typeof (ISchedulableAction), false, false, false),
                };

        /// <summary>
        /// The interface type.
        /// </summary>
        [NotNull]
        public readonly Type InterfaceType;

        /// <summary>
        /// Whether the action is actually a function.
        /// </summary>
        public readonly bool IsFunction;

        /// <summary>
        /// Whether the action is asynchronous.
        /// </summary>
        public readonly bool IsAsynchronous;

        /// <summary>
        /// Whether the action is cancellable.
        /// </summary>
        public readonly bool IsCancellable;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulableActionInfo"/> struct.
        /// </summary>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="isFunction">if set to <see langword="true"/> [is function].</param>
        /// <param name="isAsynchronous">if set to <see langword="true"/> [is asynchronous].</param>
        /// <param name="isCancellable">if set to <see langword="true"/> [is cancellable].</param>
        /// <remarks></remarks>
        private SchedulableActionInfo([NotNull]Type interfaceType, bool isFunction, bool isAsynchronous, bool isCancellable)
        {
            InterfaceType = interfaceType;
            IsFunction = isFunction;
            IsAsynchronous = isAsynchronous;
            IsCancellable = isCancellable;
        }

        /// <summary>
        /// Gets information about the specified schedulable action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        public static SchedulableActionInfo Get([NotNull]ISchedulableAction action)
        {
            Type actionType = action.GetType();
            // Find the first info that the action is assignable to.
            return _infos.First(info => info.InterfaceType.IsAssignableFrom(actionType));
        }

        /// <summary>
        /// Gets information about the specified schedulable action type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        public static SchedulableActionInfo Get<T>() where T : ISchedulableAction
        {
            Type actionType = typeof(T);
            // Find the first info that the action is assignable to.
            return _infos.First(info => info.InterfaceType.IsAssignableFrom(actionType));
        }
    }
}