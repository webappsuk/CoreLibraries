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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Holds all commands tha can be run by the <see cref="ServiceRunner"/>, by <see cref="InstanceType"/>.
    /// </summary>
    public class ServiceRunnerCommands
    {
        /// <summary>
        /// The commands by instance type.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<Type, ServiceRunnerCommands> _commandTypes =
            new ConcurrentDictionary<Type, ServiceRunnerCommands>();

        /// <summary>
        /// The commands implemneted by the <see cref="InstanceType"/>
        /// </summary>
        [NotNull]
        private readonly IReadOnlyDictionary<string, ServiceRunnerCommand> _commands;

        /// <summary>
        /// The instance type that the commands accept.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly Type InstanceType;

        /// <summary>
        /// Gets all commands names (including aliases).
        /// </summary>
        /// <value>All commands.</value>
        [NotNull]
        [PublicAPI]
        public IEnumerable<string> AllNames
        {
            get
            {
                Contract.Assert(_commands.Keys != null);
                return _commands.Keys;
            }
        }

        /// <summary>
        /// Gets all commands implemented by the type.
        /// </summary>
        /// <value>All commands.</value>
        [NotNull]
        [PublicAPI]
        public IEnumerable<ServiceRunnerCommand> AllCommands
        {
            get
            {
                Contract.Assert(_commands.Values != null);
                return _commands.Values.Distinct();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRunnerCommands"/> class.
        /// </summary>
        /// <param name="instanceType">Type of the instance.</param>
        /// <param name="commands">The commands.</param>
        private ServiceRunnerCommands(
            [NotNull] Type instanceType,
            [NotNull] IReadOnlyDictionary<string, ServiceRunnerCommand> commands)
        {
            Contract.Requires<RequiredContractException>(instanceType != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(commands != null, "Parameter_Null");
            InstanceType = instanceType;
            _commands = commands;
        }

        /// <summary>
        /// Runs the command on the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="command">The command.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns><see langword="true" /> if succeeded, <see langword="false" /> otherwise.</returns>
        public bool Run([NotNull] string command, [NotNull] object instance, [NotNull] string arguments)
        {
            Contract.Requires<RequiredContractException>(instance != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(command != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(arguments != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(InstanceType.IsInstanceOfType(instance), "Bad_Instance");

            ServiceRunnerCommand src;
            if (!_commands.TryGetValue(command, out src))
            {
                Log.Add(LoggingLevel.Error, ServiceResources.Err_Command_Not_On_Instance, command, InstanceType);
                return false;
            }

            return src.Run(instance, arguments);
        }

        /// <summary>
        /// Gets the <see cref="ServiceRunnerCommands"/> by <see cref="InstanceType"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="InstanceType"/>.</typeparam>
        /// <returns>ServiceRunnerCommands.</returns>
        [NotNull]
        [PublicAPI]
        public static ServiceRunnerCommands Get<T>()
        {
            return Get(typeof (T));
        }

        /// <summary>
        /// Gets the <see cref="ServiceRunnerCommands"/> by <see paramref="instanceType"/>.
        /// </summary>
        /// <returns>ServiceRunnerCommands.</returns>
        [NotNull]
        [PublicAPI]
        public static ServiceRunnerCommands Get([NotNull] Type instanceType)
        {
            Contract.Requires<RequiredContractException>(instanceType != null, "Parameter_Null");
            // ReSharper disable once AssignNullToNotNullAttribute
            return _commandTypes.GetOrAdd(
                instanceType,
                t =>
                {
                    Contract.Assert(t != null);
                    MethodInfo[] allMethods = t
                        .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .ToArray();
                    Dictionary<string, ServiceRunnerCommand> commands =
                        new Dictionary<string, ServiceRunnerCommand>(
                            allMethods.Length * 3,
                            StringComparer.CurrentCultureIgnoreCase);
                    foreach (MethodInfo method in allMethods)
                    {
                        Contract.Assert(method != null);
                        ServiceRunnerCommand src;
                        try
                        {
                            ServiceRunnerCommandAttribute attribute = method
                                .GetCustomAttributes(typeof (ServiceRunnerCommandAttribute), false)
                                .OfType<ServiceRunnerCommandAttribute>()
                                .FirstOrDefault();
                            if (attribute == null) continue;
                            if (method.IsGenericMethod)
                            {
                                Log.Add(
                                    LoggingLevel.Warning,
                                    () => ServiceResources.Wrn_Command_Invalid_Generic,
                                    method);
                                continue;
                            }

                            src = new ServiceRunnerCommand(method, attribute);
                        }
                        catch (Exception e)
                        {
                            Log.Add(
                                e,
                                LoggingLevel.Warning,
                                () => ServiceResources.Wrn_ServiceCommand_Creation_Failed,
                                method);
                            continue;
                        }

                        // Add command aliases to dictionary
                        foreach (string name in src.AllNames)
                        {
                            Contract.Assert(name != null);
                            ServiceRunnerCommand existing;
                            if (commands.TryGetValue(name, out existing))
                            {
                                Contract.Assert(existing != null);
                                Log.Add(
                                    LoggingLevel.Warning,
                                    () => ServiceResources.Wrn_Command_Alias_Already_Used_By_Other_Command,
                                    name,
                                    src.Name,
                                    existing.Name);
                            }
                            commands[name] = src;
                        }
                    }

                    return new ServiceRunnerCommands(instanceType, commands);
                });
        }
    }
}