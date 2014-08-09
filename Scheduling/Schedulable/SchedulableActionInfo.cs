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
using System.Collections.Generic;
using System.Diagnostics;
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
                new SchedulableActionInfo(typeof(ISchedulableFunctionCancellableAsync<>), true, true, true),
                new SchedulableActionInfo(typeof(ISchedulableActionCancellableAsync), false, true, true),
                new SchedulableActionInfo(typeof(ISchedulableFunctionAsync<>), true, true, false),
                new SchedulableActionInfo(typeof(ISchedulableActionAsync), false, true, false),
                new SchedulableActionInfo(typeof(ISchedulableFunction<>), true, false, false),
                new SchedulableActionInfo(typeof(ISchedulableAction), false, false, false),
                new SchedulableActionInfo(typeof(ISchedulableFunctionCancellableAsync<>), true, true, true),
                new SchedulableActionInfo(typeof(ISchedulableActionCancellableAsync), false, true, true),
                new SchedulableActionInfo(typeof(ISchedulableFunctionAsync<>), true, true, false),
                new SchedulableActionInfo(typeof(ISchedulableActionAsync), false, true, false),
                new SchedulableActionInfo(typeof(ISchedulableFunction<>), true, false, false),
                new SchedulableActionInfo(typeof(ISchedulableAction), false, false, false),
            };

        /// <summary>
        /// Caches lookups for performance.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<Type, SchedulableActionInfo> _cache =
            new ConcurrentDictionary<Type, SchedulableActionInfo>();

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
        /// The return type of any function (or null if action).
        /// </summary>
        [CanBeNull]
        public readonly Type FunctionReturnType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulableActionInfo"/> struct.
        /// </summary>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="isFunction">if set to <see langword="true"/> [is function].</param>
        /// <param name="isAsynchronous">if set to <see langword="true"/> [is asynchronous].</param>
        /// <param name="isCancellable">if set to <see langword="true"/> [is cancellable].</param>
        /// <remarks></remarks>
        private SchedulableActionInfo(
            [NotNull] Type interfaceType,
            bool isFunction,
            bool isAsynchronous,
            bool isCancellable,
            Type functionReturnType = null)
        {
            InterfaceType = interfaceType;
            IsFunction = isFunction;
            IsAsynchronous = isAsynchronous;
            IsCancellable = isCancellable;
            FunctionReturnType = functionReturnType;
        }

        /// <summary>
        /// Gets information about the specified schedulable action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        public static SchedulableActionInfo Get([NotNull] ISchedulableAction action)
        {
            SchedulableActionInfo info = Get(action.GetType());
            Debug.Assert(info != null);
            return info;
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
            SchedulableActionInfo info = Get(typeof(T));
            Debug.Assert(info != null);
            return info;
        }

        /// <summary>
        /// Gets information about the specified schedulable action type.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [CanBeNull]
        public static SchedulableActionInfo Get([NotNull] Type actionType)
        {
            // Find the first info that the action is assignable to.
            return _cache.GetOrAdd(
                actionType,
                at =>
                {
                    foreach (SchedulableActionInfo info in _infos)
                    {
                        Debug.Assert(info != null);
                        if (!info.IsFunction)
                        {
                            if (info.InterfaceType.IsAssignableFrom(actionType))
                                return info;
                            continue;
                        }

                        Type t = actionType;
                        while (t != null)
                        {
                            Type interfaceType = t.GetInterfaces().Where(it => it.IsGenericType).
                                FirstOrDefault(
                                    it => info.InterfaceType.IsAssignableFrom(it.GetGenericTypeDefinition()));
                            if (interfaceType != null)
                            {
                                return new SchedulableActionInfo(
                                    interfaceType,
                                    true,
                                    info.IsAsynchronous,
                                    info.IsCancellable,
                                    interfaceType.GetGenericArguments()[0]);
                            }

                            t = t.BaseType;
                        }
                    }
                    return null;
                });
        }
    }
}