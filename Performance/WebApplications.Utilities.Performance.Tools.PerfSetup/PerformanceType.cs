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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace WebApplications.Utilities.Performance.Tools.PerfSetup
{
    /// <summary>
    /// Holds information about performance counter types.
    /// </summary>
    internal class PerformanceType
    {
        /// <summary>
        /// The type cache.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, PerformanceType> _types =
            new ConcurrentDictionary<string, PerformanceType>();

        /// <summary>
        /// The performance counter type lookup
        /// </summary>
        [NotNull]
        private static readonly Dictionary<int, PerformanceCounterType> _performanceCounterTypes =
            Enum.GetValues(typeof(PerformanceCounterType))
                .Cast<PerformanceCounterType>()
                .ToDictionary(v => (int)v, v => v);

        /// <summary>
        /// The type reference.
        /// </summary>
        [NotNull]
        public readonly TypeReference TypeReference;

        /// <summary>
        /// The counter creation data for the type.
        /// </summary>
        [NotNull]
        public readonly CounterCreationData[] CounterCreationData;

        /// <summary>
        /// Whether the type is valid.
        /// </summary>
        public readonly bool IsValid;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceType" /> class.
        /// </summary>
        /// <param name="typeReference">The type reference.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        // ReSharper disable once NotNullMemberIsNotInitialized
        private PerformanceType([NotNull] TypeReference typeReference)
        {
            if (typeReference == null) throw new ArgumentNullException("typeReference");
            TypeReference = typeReference;
            try
            {
                TypeDefinition typeDefinition = typeReference.Resolve();
                Debug.Assert(typeDefinition != null);

                // Confirm we descend from PerfCategory
                TypeDefinition baseType = typeDefinition;
                do
                {
                    baseType = baseType.BaseType != null
                        ? baseType.BaseType.Resolve()
                        : null;

                    if (baseType == null)
                    {
                        Logger.Add(
                            Level.Error,
                            "The '{0}' type does not descend from 'WebApplications.Utilities.Performance.PerfCategory' and so cannot be used.",
                            typeDefinition.FullName);
                        IsValid = false;
                        return;
                    }

                    // Check for match
                    if ((baseType.FullName == "WebApplications.Utilities.Performance.PerfCategory") &&
                        // ReSharper disable PossibleNullReferenceException
                        (baseType.Module.Assembly.Name.Name == "WebApplications.Utilities.Performance"))
                        // ReSharper restore PossibleNullReferenceException
                        break;
                } while (true);

                // Find CounterCreationData[] field
                // ReSharper disable once AssignNullToNotNullAttribute
                FieldDefinition creationField = typeDefinition
                    .Fields
                    .SingleOrDefault(
                        fd =>
                            // ReSharper disable PossibleNullReferenceException
                            fd.IsStatic && fd.FieldType.FullName == "System.Diagnostics.CounterCreationData[]" &&
                            // ReSharper restore PossibleNullReferenceException
                            fd.IsInitOnly
                    );

                if (creationField == null)
                {
                    Logger.Add(
                        Level.Error,
                        "The '{0}' type does not contains a single readonly static field of type 'System.Diagnostics.CounterCreationData[]' so counters cannot be created.",
                        typeDefinition.FullName);
                    IsValid = false;
                    return;
                }

                // Find static constructor (initialised static readonly fields must be initialized from a static constructor).
                MethodDefinition staticConstructor =
                    // ReSharper disable once AssignNullToNotNullAttribute, PossibleNullReferenceException
                    typeDefinition.Methods.SingleOrDefault(m => m.IsConstructor && m.IsStatic);
                if ((staticConstructor == null) ||
                    !staticConstructor.HasBody)
                {
                    Logger.Add(
                        Level.Error,
                        "The '{0}' type does not contain a static constructor so the initial values for the '{1}' field cannot be parsed.",
                        typeDefinition.FullName,
                        creationField.FullName);
                    IsValid = false;
                    return;
                }

                // Parse static constructor for creation data.
                // TODO Genericising this and sharing with Scan would be very cool (but also quite complex).
                Queue<string> lastStrings = new Queue<string>(2);
                PerformanceCounterType lastCounterType = default(PerformanceCounterType);
                List<CounterCreationData> data = new List<CounterCreationData>();
                // ReSharper disable PossibleNullReferenceException
                foreach (Instruction instr in staticConstructor.Body.Instructions)
                    // ReSharper restore PossibleNullReferenceException
                {
                    Debug.Assert(instr != null);

                    // Detect string literals being loaded onto evaluation stack
                    if (instr.OpCode.Code == Code.Ldstr)
                    {
                        // We track last two load strings.
                        if (lastStrings.Count > 1)
                            lastStrings.Dequeue();

                        lastStrings.Enqueue(instr.Operand as string);
                        continue;
                    }

                    // Detect nulls being loaded onto evaluation stack
                    if (instr.OpCode.Code == Code.Ldnull)
                    {
                        // We track last two load strings.
                        if (lastStrings.Count > 1)
                            lastStrings.Dequeue();

                        lastStrings.Enqueue(null);
                        continue;
                    }

                    // Detect integers being loaded onto the evaluation stack.
                    if (instr.OpCode.Code == Code.Ldc_I4)
                    {
                        // If the int is not a valid performance counter type, clear strings (as this isn't the right data).
                        // ReSharper disable once PossibleNullReferenceException
                        if (!_performanceCounterTypes.TryGetValue((int)instr.Operand, out lastCounterType))
                            lastStrings.Clear();
                        continue;
                    }

                    if (instr.OpCode.Code != Code.Newobj)
                    {
                        // If we have any ops other than NewObj after our loads then the loads aren't for us.
                        lastStrings.Clear();
                        lastCounterType = default(PerformanceCounterType);
                        continue;
                    }

                    // Check we have a 3 parameter constructor with the right types
                    MethodReference methodReference = instr.Operand as MethodReference;
                    if ((methodReference == null) ||
                        // ReSharper disable PossibleNullReferenceException
                        (methodReference.Parameters.Count != 3) ||
                        (methodReference.Parameters[0].ParameterType.FullName != "System.String") ||
                        (methodReference.Parameters[1].ParameterType.FullName != "System.String") ||
                        (methodReference.Parameters[2].ParameterType.FullName !=
                         // ReSharper restore PossibleNullReferenceException
                         "System.Diagnostics.PerformanceCounterType"))
                        continue;

                    // Check this is a constructor for CounterCreationData.
                    TypeReference ctr = methodReference.DeclaringType;
                    if ((ctr == null) ||
                        (ctr.FullName !=
                         "System.Diagnostics.CounterCreationData"))
                        continue;

                    if (lastStrings.Count == 2)
                    {
                        // Add creation data.
                        string counterCategory = lastStrings.Dequeue();
                        string counterHelp = lastStrings.Dequeue();
                        data.Add(new CounterCreationData(counterCategory, counterHelp, lastCounterType));
                        Logger.Add(
                            Level.Low,
                            "The '{0}' type specifies CounterCreationData(\"{1}\", {2}, PerformanceCounterType.{3}).",
                            typeReference.FullName,
                            counterCategory,
                            counterHelp == null ? "null" : "\"" + counterHelp + "\"",
                            lastCounterType);
                    }
                    else
                    {
                        Logger.Add(
                            Level.Warning,
                            "Performance counter creation data construction was found for the '{0}' type, but the parameters were not literals and so could not be decoded.",
                            typeDefinition.FullName);
                        IsValid = false;
                        return;
                    }

                    lastStrings.Clear();
                }

                if (data.Count < 1)
                {
                    Logger.Add(
                        Level.Warning,
                        "The '{0}' type does not appear to contain any initialisation data for the '{1}' field.",
                        typeDefinition.FullName,
                        creationField.FullName);
                    IsValid = false;
                    return;
                }

                // We have valid creation data for the type.
                CounterCreationData = data.ToArray();
                Logger.Add(
                    Level.Normal,
                    "The '{0}' type defines '{1}' performance counters.",
                    typeDefinition.FullName,
                    CounterCreationData.Length);
                IsValid = true;
            }
            catch (Exception e)
            {
                Logger.Add(
                    Level.Error,
                    "Fatal error occurred trying to parse the '{0}' performance counter type. {1}",
                    typeReference.FullName,
                    e.Message);
                IsValid = false;
            }
        }

        /// <summary>
        /// Gets the specified type reference.
        /// </summary>
        /// <param name="typeReference">The type reference.</param>
        /// <returns>PerformanceType.</returns>
        [NotNull]
        public static PerformanceType Get([NotNull] TypeReference typeReference)
        {
            if (typeReference == null) throw new ArgumentNullException("typeReference");
            // ReSharper disable AssignNullToNotNullAttribute
            return _types.GetOrAdd(typeReference.FullName, t => new PerformanceType(typeReference));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return TypeReference + (IsValid ? string.Empty : " (INVALID)");
        }
    }
}