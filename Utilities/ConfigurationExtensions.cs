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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Reflection;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Reflect;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Extension methods for <see cref="IConfigurationElement"/>.
    /// </summary>
    [PublicAPI]
    public static class ConfigurationExtensions
    {
        #region Get files from a configuration
        // ReSharper disable AssignNullToNotNullAttribute, PossibleNullReferenceException
        /// <summary>
        /// The <see cref="FieldInfo"/> corresponding to <c>System.Configuration.BaseConfigurationRecord._configRecord</c>.
        /// </summary>[NotNull]
        [NotNull]
        private static readonly FieldInfo _configRecordField =
            ExtendedType.Get(typeof(System.Configuration.Configuration))
                .GetField("_configRecord");

        [NotNull]
        private static readonly PropertyInfo _configStreamInfoProperty =
            ExtendedType.Get("System.Configuration.BaseConfigurationRecord, System.Configuration")
                .GetProperty("ConfigStreamInfo");

        [NotNull]
        private static readonly PropertyInfo _streamInfosProperty =
            ExtendedType.Get(
                "System.Configuration.BaseConfigurationRecord+ConfigRecordStreamInfo, System.Configuration")
                .GetProperty("StreamInfos");

        [NotNull]
        private static readonly PropertyInfo _dictionaryValues =
            ExtendedType.Get(typeof(HybridDictionary))
                .GetProperty("Values");

        [NotNull]
        private static readonly ExtendedType _streamInfoExtendedType =
            ExtendedType.Get("System.Configuration.StreamInfo, System.Configuration");

        [NotNull]
        private static readonly FieldInfo _streamNameField =
            _streamInfoExtendedType
                .GetField("_streamName");

        [NotNull]
        private static readonly MethodInfo _iCollectionCopyToMethod =
            ExtendedType.Get(typeof(ICollection))
            .GetMethod("CopyTo", typeof(Array), typeof(int), TypeSearch.Void);

        [NotNull]
        private static readonly PropertyInfo _iCollectionCountProperty =
            ExtendedType.Get(typeof(ICollection))
                .GetProperty("Count");

        /// <summary>
        /// Gets the associated configuration file paths.
        /// </summary>
        [NotNull]
        private static readonly Func<System.Configuration.Configuration, IReadOnlyCollection<string>>
            _getConfigFilePaths;

        /// <summary>
        /// Gets the configuration file paths for a given <see cref="System.Configuration.Configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The file paths that make up the configuration.</returns>
        public static IReadOnlyCollection<string> GetConfigFilePaths(
            [CanBeNull] this System.Configuration.Configuration configuration)
            // ReSharper disable once EventExceptionNotDocumented
            => configuration == null ? null : _getConfigFilePaths(configuration);
        #endregion
        
        /// <summary>
        /// Initializes static members of the <see cref="Utilities.Configuration.ConfigurationElement" /> class.
        /// </summary>
        static ConfigurationExtensions()
        {
            ParameterExpression configuration = Expression.Parameter(typeof(System.Configuration.Configuration), "configuration");
            ParameterExpression streamInfos = Expression.Parameter(typeof(ICollection), "streamInfos");
            ParameterExpression index = Expression.Parameter(typeof(int), "index");
            ParameterExpression resultArray = Expression.Parameter(typeof(string[]), "result");

            _getConfigFilePaths = Expression
                .Lambda<Func<System.Configuration.Configuration, IReadOnlyCollection<string>>>(
                    Expression.Convert(
                        Expression.Block(
                            typeof(string[]),
                            new[] { streamInfos, index, resultArray },
                            // Get the StreamInfos HybridDictionary's values
                            Expression.Assign(
                                streamInfos,
                                Expression.Property(
                                    Expression.Property(
                                        Expression.Property(
                                            Expression.Field(configuration, _configRecordField),
                                            _configStreamInfoProperty),
                                        _streamInfosProperty),
                                    _dictionaryValues)),

                            // Create string[] of same size as results
                            Expression.Assign(
                                resultArray,
                                Expression.NewArrayBounds(
                                    typeof(string),
                                    Expression.Property(
                                        streamInfos,
                                        _iCollectionCountProperty))),

                            // Initialize index
                            Expression.Assign(index, Expression.Constant(0)),

                            // Copy names into new array
                            Expression.Convert(streamInfos, typeof(IEnumerable))
                                .ForEach(
                                    item =>
                                        Expression.Block(
                                            Expression.Assign(
                                                Expression.ArrayAccess(resultArray, index),
                                                Expression.Field(
                                                    Expression.Convert(item, _streamInfoExtendedType.Type),
                                                    _streamNameField)),
                                            Expression.Assign(index, Expression.Increment(index)))),

                            // Return result array
                            resultArray),
                        typeof(IReadOnlyCollection<string>)),
                    configuration)
                .Compile();
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="configurationElementName">The property.</param>
        /// <returns>The full path.</returns>
        [NotNull]
        public static string GetFullPath([CanBeNull] this IConfigurationElement parent, [CanBeNull] string configurationElementName)
        {
            if (string.IsNullOrEmpty(configurationElementName))
                configurationElementName = "?";

            // Short cut
            if (parent == null) return configurationElementName;

            if (char.IsLetterOrDigit(configurationElementName[0]))
                configurationElementName = "." + configurationElementName;
            return parent.FullPath + configurationElementName;
        }

    }
}