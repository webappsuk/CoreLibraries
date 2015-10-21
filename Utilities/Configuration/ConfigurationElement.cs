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

        /// <summary>
        /// Creates an element of the specified type.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <returns>An element of type <typeparamref name="T"/>.</returns>
        /// <seealso cref="IConfigurationElement" />
        public static T Create<T>()
            where T : IConfigurationElement, new()
            => (T)Create(typeof(T));

        /// <inheritdoc />
        public IConfigurationElement Section => Parent?.Section;
    }
}