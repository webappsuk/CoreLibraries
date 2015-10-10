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

using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebApplications.Utilities.Annotations;

namespace ILMerge.Build.Task
{
    /// <summary>
    /// Extensions for <see cref="IBuildEngine"/>.
    /// </summary>
    public static class BuildEngineExtensions
    {
        private const BindingFlags BindingFlags =
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy |
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public;

        /// <summary>
        /// Gets the environment variable.
        /// </summary>
        /// <param name="buildEngine">The build engine.</param>
        /// <param name="key">The key.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> [throw if not found].</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        [NotNull]
        public static IEnumerable<string> GetEnvironmentVariable(
            this IBuildEngine buildEngine,
            string key,
            // ReSharper disable once UnusedParameter.Global
            bool throwIfNotFound = false)
        {
            ProjectInstance projectInstance = GetProjectInstance(buildEngine);

            List<ProjectItemInstance> items = projectInstance.Items
                .Where(x => string.Equals(x.ItemType, key, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (items.Count > 0)
                return items.Select(x => x.EvaluatedInclude);


            List<ProjectPropertyInstance> properties = projectInstance.Properties
                .Where(x => string.Equals(x.Name, key, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (properties.Count > 0)
                return properties.Select(x => x.EvaluatedValue);

            if (throwIfNotFound)
                throw new Exception($"Could not extract from '{key}' environmental variables.");

            return Enumerable.Empty<string>();
        }

        private static ProjectInstance GetProjectInstance(IBuildEngine buildEngine)
        {
            Type buildEngineType = buildEngine.GetType();
            FieldInfo targetBuilderCallbackField = buildEngineType.GetField("targetBuilderCallback", BindingFlags);
            if (targetBuilderCallbackField == null)
                throw new Exception("Could not extract targetBuilderCallback from " + buildEngineType.FullName);

            object targetBuilderCallback = targetBuilderCallbackField.GetValue(buildEngine);
            Type targetCallbackType = targetBuilderCallback.GetType();
            FieldInfo projectInstanceField = targetCallbackType.GetField("projectInstance", BindingFlags);
            if (projectInstanceField == null)
                throw new Exception("Could not extract projectInstance from " + targetCallbackType.FullName);

            return (ProjectInstance) projectInstanceField.GetValue(targetBuilderCallback);
        }
    }
}