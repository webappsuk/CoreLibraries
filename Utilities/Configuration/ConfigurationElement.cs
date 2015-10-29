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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Reflect;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    ///   Represents a configuration element in a configuration file.
    /// </summary>
    [PublicAPI]
    public abstract partial class ConfigurationElement : System.Configuration.ConfigurationElement, IInternalConfigurationElement
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
        private static ExtendedType _streamInfoExtendedType =
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
        internal static readonly Func<System.Configuration.Configuration, IReadOnlyCollection<string>>
            GetConfigFilePaths;
        #endregion

        #region Create Elements
        // ReSharper disable AssignNullToNotNullAttribute, PossibleNullReferenceException
        /// <summary>
        /// The <see cref="MethodInfo"/> corresponding to <see cref="IInternalConfigurationElement.Initialize"/>.
        /// </summary>
        [NotNull]
        private static readonly MethodInfo _initializeMethod =
            ExtendedType.Get(typeof(IInternalConfigurationElement))
                .GetMethod("Initialize", TypeSearch.Void);

        /// <summary>
        /// The function to efficiently get the values collection from a <see cref="System.Configuration.ConfigurationElement"/>.
        /// </summary>
        [NotNull]
        internal static readonly Func<System.Configuration.ConfigurationElement, NameObjectCollectionBase> GetValues =
            ExtendedType.Get(typeof(System.Configuration.ConfigurationElement))
                .GetField("_values")
                .Getter<System.Configuration.ConfigurationElement, NameObjectCollectionBase>();
        // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException

        /// <summary>
        /// Holds code for creating collection types.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<Type, Func<IInternalConfigurationElement>> _createElements
            = new ConcurrentDictionary<Type, Func<IInternalConfigurationElement>>();

        /// <summary>
        /// Creates the element, if the supplied <paramref name="type"/> implements
        /// <see cref="IInternalConfigurationElement"/>.
        /// </summary>
        /// <param name="type">Type of the element.</param>
        /// <returns>WebApplications.Utilities.Configuration.IInternalConfigurationElement.</returns>
            // ReSharper disable ExceptionNotDocumented
        internal static IInternalConfigurationElement Create([NotNull] Type type) =>
                    _createElements.GetOrAdd(
                        type,
                        t =>
                        {
                            Debug.Assert(t != null);
                            if (!t.ImplementsInterface(typeof(IInternalConfigurationElement))) return null;

                            ParameterExpression result = Expression.Parameter(typeof(IInternalConfigurationElement), "result");
                            return (Func<IInternalConfigurationElement>)Expression.Lambda(
                                Expression.Block(
                                    new[] { result },
                                    Expression.Assign(
                                        result,
                                        Expression.Convert(Expression.New(type), typeof(IInternalConfigurationElement))),
                                    Expression.Call(result, _initializeMethod),
                                    result)).Compile();
                        })?.Invoke();
        // ReSharper restore ExceptionNotDocumented
        #endregion

        /// <summary>
        /// Initializes static members of the <see cref="ConfigurationElement" /> class.
        /// </summary>
        static ConfigurationElement()
        {
            ParameterExpression configuration = Expression.Parameter(typeof(System.Configuration.Configuration), "configuration");
            ParameterExpression streamInfos = Expression.Parameter(typeof(ICollection), "streamInfos");
            ParameterExpression index = Expression.Parameter(typeof(int), "index");
            ParameterExpression resultArray = Expression.Parameter(typeof(string[]), "result");

            GetConfigFilePaths = Expression
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
        /// Creates an element of the specified type.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <returns>An element of type <typeparamref name="T"/>.</returns>
        /// <seealso cref="IConfigurationElement" />
        public static T Create<T>()
            where T : IConfigurationElement, new()
            => (T)Create(typeof(T));

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="property">The property.</param>
        /// <returns>The full path.</returns>
        [NotNull]
        internal static string GetFullPath([CanBeNull] IConfigurationElement parent, [CanBeNull] string property)
        {
            if (string.IsNullOrWhiteSpace(property))
                property = ".*";

            if (parent == null) return property;
            if (char.IsLetterOrDigit(property[0])) property = $".{property}";

            return $"{parent.FullPath}{property}";
        }

        /// <inheritdoc />
        IInternalConfigurationSection IInternalConfigurationElement.Section => _parent?.Section;

        /// <inheritdoc />
        public bool IsDisposed => Section?.IsDisposed ?? false;
    }
}