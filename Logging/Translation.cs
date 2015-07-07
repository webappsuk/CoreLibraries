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
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    /// Handles resource translation for logs.
    /// </summary>
    [PublicAPI]
    public class Translation
    {
        /// <summary>
        /// Holds all resource types seen by the logger.
        /// </summary>
        [NotNull]
        [NonSerialized]
        private static readonly ConcurrentDictionary<string, ResourceManager> _resourceManagers =
            new ConcurrentDictionary<string, ResourceManager>();

        /// <summary>
        /// Gets or sets the default culture information.
        /// </summary>
        /// <value>
        /// The default culture information.
        /// </value>
        /// <remarks>
        /// <para>
        /// This is the culture that logs will use when added to the system.
        /// </para></remarks>
        [NotNull]
        public static CultureInfo DefaultCulture
        {
            get { return _defaultCulture; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _defaultCulture = value;
            }
        }

        /// <summary>
        /// The default culture information.
        /// </summary>
        [NotNull]
        [NonSerialized]
        private static CultureInfo _defaultCulture = CultureInfo.CurrentCulture;

        /// <summary>
        /// Registers the type for resource retrieval, and returns the <see cref="ResourceManager"/>.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <returns></returns>
        [CanBeNull]
        public static ResourceManager GetResourceManager<TResource>()
            where TResource : class
        {
            return GetResourceManager(typeof(TResource));
        }

        /// <summary>
        /// Registers the type for resource retrieval, and returns the <see cref="ResourceManager"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResourceManager GetResourceManager([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            Debug.Assert(type.FullName != null);

            return _resourceManagers.GetOrAdd(
                type.FullName,
                t =>
                {
                    try
                    {
                        PropertyInfo resourceInfo = type.GetProperty(
                            "ResourceManager",
                            BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public |
                            BindingFlags.Static);

                        return resourceInfo == null
                            ? null
                            : resourceInfo.GetValue(null) as ResourceManager;
                    }
                    catch (Exception e)
                    {
                        Log.Add(e, LoggingLevel.Error, () => Resources.Log_GetResourceManager_FatalError, t);
                        return null;
                    }
                });
        }

        /// <summary>
        /// Gets the resource manager, by the type's full name, assuming it has been seen previously (or registered using one of the typed overloads).
        /// </summary>
        /// <param name="typeFullName">Full name of the type.</param>
        /// <returns>A resource manager, if the type has already been registered.</returns>
        /// <remarks>This will only retrieve <see cref="ResourceManager">resource managers</see> for previously registered types.</remarks>
        [CanBeNull]
        public static ResourceManager GetResourceManager([NotNull] string typeFullName)
        {
            if (typeFullName == null) throw new ArgumentNullException("typeFullName");

            ResourceManager resourceManager;
            return _resourceManagers.TryGetValue(typeFullName, out resourceManager) ? resourceManager : null;
        }

        /// <summary>
        /// Gets the resource's value, in the <see cref="DefaultCulture">default culture</see>.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns>
        /// The resource's value in the specified <see paramref="culture" />; otherwise <see langword="null" />.
        /// </returns>
        [CanBeNull]
        public static string GetResource<TResource>([NotNull] string property)
            where TResource : class
        {
            string translation;
            return TryGetResource(typeof(TResource), property, DefaultCulture, out translation) ? translation : null;
        }

        /// <summary>
        /// Gets the resource's value, in the <see cref="DefaultCulture">default culture</see>.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="property">The property.</param>
        /// <returns>
        /// The resource's value in the specified <see paramref="culture" />; otherwise <see langword="null" />.
        /// </returns>
        [CanBeNull]
        public static string GetResource([NotNull] Type resourceType, [NotNull] string property)
        {
            string translation;
            return TryGetResource(resourceType, property, DefaultCulture, out translation) ? translation : null;
        }

        /// <summary>
        /// Gets the resource's value, in the <see paramref="culture">specified culture</see>.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// The resource's value in the specified <see paramref="culture" />; otherwise <see langword="null" />.
        /// </returns>
        [CanBeNull]
        public static string GetResource<TResource>([NotNull] string property, [CanBeNull] CultureInfo culture)
            where TResource : class
        {
            string translation;
            return TryGetResource(typeof(TResource), property, culture, out translation) ? translation : null;
        }

        /// <summary>
        /// Gets the resource's value, in the <see paramref="culture">specified culture</see>.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="property">The property.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The resource's value in the specified <see paramref="culture"/>; otherwise <see langword="null"/>.</returns>
        [CanBeNull]
        public static string GetResource(
            [NotNull] Type resourceType,
            [NotNull] string property,
            [CanBeNull] CultureInfo culture)
        {
            string translation;
            return TryGetResource(resourceType, property, culture, out translation) ? translation : null;
        }

        /// <summary>
        /// Tries to get the resource value, in the <see cref="DefaultCulture">default culture</see>, for the given property name.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="translation">The translation.</param>
        /// <returns>
        ///   <see langword="true" /> if the resource was found; otherwise <see langword="false" />.
        /// </returns>
        [ContractAnnotation("=>true,translation:notnull;=>false,translation:null")]
        public static bool TryGetResource<TResource>([NotNull] string property, out string translation)
            where TResource : class
        {
            return TryGetResource(typeof(TResource), property, DefaultCulture, out translation);
        }

        /// <summary>
        /// Tries to get the resource value, in the <see cref="DefaultCulture">default culture</see>, for the given property name.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="property">The property.</param>
        /// <param name="translation">The translation.</param>
        /// <returns>
        ///   <see langword="true" /> if the resource was found; otherwise <see langword="false" />.
        /// </returns>
        [ContractAnnotation("=>true,translation:notnull;=>false,translation:null")]
        public static bool TryGetResource(
            [NotNull] Type resourceType,
            [NotNull] string property,
            out string translation)
        {
            return TryGetResource(resourceType, property, DefaultCulture, out translation);
        }

        /// <summary>
        /// Tries to get the resource value, in the <see paramref="culture">specified culture</see>, for the given property name.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="translation">The translation.</param>
        /// <returns>
        ///   <see langword="true" /> if the resource was found; otherwise <see langword="false" />.
        /// </returns>
        [ContractAnnotation("=>true,translation:notnull;=>false,translation:null")]
        public static bool TryGetResource<TResource>(
            [NotNull] string property,
            [CanBeNull] CultureInfo culture,
            out string translation)
            where TResource : class
        {
            return TryGetResource(typeof(TResource), property, culture, out translation);
        }

        /// <summary>
        /// Tries to get the resource value, in the <see paramref="culture">specified culture</see>, for the given property name.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="property">The property.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="translation">The translation.</param>
        /// <returns><see langword="true"/> if the resource was found; otherwise <see langword="false"/>.</returns>
        [ContractAnnotation("=>true,translation:notnull;=>false,translation:null")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetResource(
            [NotNull] Type resourceType,
            [NotNull] string property,
            [CanBeNull] CultureInfo culture,
            out string translation)
        {
            if (resourceType == null) throw new ArgumentNullException("resourceType");
            if (property == null) throw new ArgumentNullException("property");

            ResourceManager resourceManager = GetResourceManager(resourceType);
            if (resourceManager == null)
            {
                translation = null;
                return false;
            }

            try
            {
                translation = resourceManager.GetString(property, culture ?? DefaultCulture);
                return true;
            }
            catch
            {
                translation = null;
                return false;
            }
        }

        /// <summary>
        /// Gets the resource's value, in the <see cref="DefaultCulture">default culture</see>.
        /// </summary>
        /// <param name="typeFullName">Full name of the type.</param>
        /// <param name="property">The property.</param>
        /// <returns>
        /// The resource's value in the specified <see paramref="culture" />; otherwise <see langword="null" />.
        /// </returns>
        /// <remarks>This will only retrieve resource values for previously registered types.</remarks>
        [CanBeNull]
        public static string GetResource([NotNull] string typeFullName, [NotNull] string property)
        {
            string translation;
            return TryGetResource(typeFullName, property, DefaultCulture, out translation) ? translation : null;
        }

        /// <summary>
        /// Gets the resource's value, in the <see paramref="culture">specified culture</see>.
        /// </summary>
        /// <param name="typeFullName">Full name of the type.</param>
        /// <param name="property">The property.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The resource's value in the specified <see paramref="culture"/>; otherwise <see langword="null"/>.</returns>
        /// <remarks>This will only retrieve resource values for previously registered types.</remarks>
        [CanBeNull]
        public static string GetResource(
            [NotNull] string typeFullName,
            [NotNull] string property,
            [CanBeNull] CultureInfo culture)
        {
            string translation;
            return TryGetResource(typeFullName, property, culture, out translation) ? translation : null;
        }

        /// <summary>
        /// Gets a translation for a resource property.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        [CanBeNull]
        public static string GetResource(
            [NotNull] Expression<Func<string>> resource,
            [CanBeNull] CultureInfo culture = null)
        {
            Translation value;
            return TryGet(resource, out value, culture)
                ? value.Message
                : null;
        }

        /// <summary>
        /// Tries to get the resource value, in the <see cref="DefaultCulture">default culture</see>, for the given property name.
        /// </summary>
        /// <param name="typeFullName">Full name of the type.</param>
        /// <param name="property">The property.</param>
        /// <param name="translation">The translation.</param>
        /// <returns>
        ///   <see langword="true" /> if the resource was found; otherwise <see langword="false" />.
        /// </returns>
        /// <remarks>This will only retrieve resource values for previously registered types.</remarks>
        [ContractAnnotation("=>true,translation:notnull;=>false,translation:null")]
        public static bool TryGetResource(
            [NotNull] string typeFullName,
            [NotNull] string property,
            out string translation)
        {
            return TryGetResource(typeFullName, property, DefaultCulture, out translation);
        }

        /// <summary>
        /// Tries to get the resource value, in the <see paramref="culture">specified culture</see>, for the given property name.
        /// </summary>
        /// <param name="typeFullName">Full name of the type.</param>
        /// <param name="property">The property.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="translation">The translation.</param>
        /// <returns><see langword="true"/> if the resource was found; otherwise <see langword="false"/>.</returns>
        /// <remarks>This will only retrieve resource values for previously registered types.</remarks>
        [ContractAnnotation("=>true,translation:notnull;=>false,translation:null")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetResource(
            [NotNull] string typeFullName,
            [NotNull] string property,
            [CanBeNull] CultureInfo culture,
            out string translation)
        {
            if (typeFullName == null) throw new ArgumentNullException("typeFullName");
            if (property == null) throw new ArgumentNullException("property");

            ResourceManager resourceManager = GetResourceManager(typeFullName);
            if (resourceManager == null)
            {
                translation = null;
                return false;
            }

            try
            {
                translation = resourceManager.GetString(property, culture ?? DefaultCulture);
                return true;
            }
            catch
            {
                translation = null;
                return false;
            }
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        [CanBeNull]
        public static Expression<Func<string>> GetExpression<TResource>([NotNull] string property)
            where TResource : class
        {
            return GetExpression(typeof(TResource), property);
        }

        /// <summary>
        /// Gets a resource expression from the type and property name.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        [CanBeNull]
        public static Expression<Func<string>> GetExpression([NotNull] Type resourceType, [NotNull] string property)
        {
            try
            {
                return GetResourceManager(resourceType) == null
                    ? null
                    : Expression.Lambda<Func<string>>(Expression.Property(null, resourceType, property));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a resource expression from the type name and property name.
        /// </summary>
        /// <param name="typeFullName">Full name of the type.</param>
        /// <param name="property">The property.</param>
        /// <returns>An <see cref="Expression{T}"/> that represents the resource property; otherwise <see langword="null"/>.</returns>
        [CanBeNull]
        public static Expression<Func<string>> GetExpression([NotNull] string typeFullName, [NotNull] string property)
        {
            try
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                return Expression.Lambda<Func<string>>(Expression.Property(null, Type.GetType(typeFullName), property));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Tries to get a translation for a resource property.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        [ContractAnnotation("=>true,value:notnull;=>false,value:null")]
        public static bool TryGet<TResource>(
            [NotNull] string property,
            out Translation value,
            [CanBeNull] CultureInfo culture = null)
            where TResource : class
        {
            return TryGet(typeof(TResource), property, out value, culture);
        }

        /// <summary>
        /// Tries to get a translation for a resource property.
        /// </summary>
        /// <param name="resourceType">Type of the resource class.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        [ContractAnnotation("=>true,value:notnull;=>false,value:null")]
        public static bool TryGet(
            [NotNull] Type resourceType,
            [NotNull] string property,
            out Translation value,
            [CanBeNull] CultureInfo culture = null)
        {
            if (resourceType == null) throw new ArgumentNullException("resourceType");
            if (property == null) throw new ArgumentNullException("property");

            ResourceManager resourceManager = GetResourceManager(resourceType);
            if (resourceManager == null)
            {
                value = null;
                return false;
            }

            try
            {
                if (culture == null)
                    culture = DefaultCulture;

                string str = resourceManager.GetString(property, culture);
                // ReSharper disable once AssignNullToNotNullAttribute
                value = new Translation(resourceType.FullName, property, str ?? string.Empty, culture, resourceManager);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }


        /// <summary>
        /// Tries to get a translation for a resource property.
        /// </summary>
        /// <param name="typeFullName">Full name of the type.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The translation value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        [ContractAnnotation("=>true,value:notnull;=>false,value:null")]
        public static bool TryGet(
            [NotNull] string typeFullName,
            [NotNull] string property,
            out Translation value,
            [CanBeNull] CultureInfo culture = null)
        {
            if (typeFullName == null) throw new ArgumentNullException("typeFullName");
            if (property == null) throw new ArgumentNullException("property");

            ResourceManager resourceManager = GetResourceManager(typeFullName);
            if (resourceManager == null)
            {
                value = null;
                return false;
            }

            try
            {
                if (culture == null)
                    culture = DefaultCulture;

                string str = resourceManager.GetString(property, culture);
                value = new Translation(typeFullName, property, str ?? string.Empty, culture, resourceManager);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to get a translation for a resource property.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="value">The value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        [ContractAnnotation("=>true,value:notnull;=>false,value:null")]
        public static bool TryGet(
            [NotNull] Expression<Func<string>> resource,
            out Translation value,
            [CanBeNull] CultureInfo culture = null)
        {
            if (resource == null) throw new ArgumentNullException("resource");

            try
            {
                // See if we can get the resource property.
                MemberExpression me = resource.Body as MemberExpression;
                if (me != null)
                {
                    PropertyInfo pi = me.Member as PropertyInfo;
                    if (pi != null)
                    {
                        MethodInfo mi = pi.GetMethod;
                        if (mi != null)
                        {
                            Type dt = mi.DeclaringType;
                            Debug.Assert(dt != null);

                            return TryGet(dt, pi.Name, out value, culture);
                        }
                    }
                }
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Gets the translation for a resource property.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        [CanBeNull]
        public static Translation Get<TResource>(
            [NotNull] string property,
            [CanBeNull] CultureInfo culture = null)
            where TResource : class
        {
            Translation value;
            return TryGet(typeof(TResource), property, out value, culture)
                ? value
                : null;
        }

        /// <summary>
        /// Gets the translation for a resource property.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="property">The property.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        [CanBeNull]
        public static Translation Get(
            [NotNull] Type resourceType,
            [NotNull] string property,
            [CanBeNull] CultureInfo culture = null)
        {
            Translation value;
            return TryGet(resourceType, property, out value, culture)
                ? value
                : null;
        }


        /// <summary>
        /// Gets a translation for a resource property.
        /// </summary>
        /// <param name="typeFullName">Full name of the type.</param>
        /// <param name="property">The property.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        [CanBeNull]
        public static Translation Get(
            [NotNull] string typeFullName,
            [NotNull] string property,
            [CanBeNull] CultureInfo culture = null)
        {
            Translation value;
            return TryGet(typeFullName, property, out value, culture)
                ? value
                : null;
        }

        /// <summary>
        /// Gets a translation for a resource property.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        [CanBeNull]
        public static Translation Get(
            [NotNull] Expression<Func<string>> resource,
            [CanBeNull] CultureInfo culture = null)
        {
            Translation value;
            return TryGet(resource, out value, culture)
                ? value
                : null;
        }

        /// <summary>
        /// The resource manager
        /// </summary>
        [NotNull]
        public readonly ResourceManager ResourceManager;

        /// <summary>
        /// The resource type full name
        /// </summary>
        [NotNull]
        public readonly string ResourceTypeFullName;

        /// <summary>
        /// The resource tag
        /// </summary>
        [NotNull]
        public readonly string ResourceTag;

        /// <summary>
        /// The message format
        /// </summary>
        [NotNull]
        public readonly string Message;

        /// <summary>
        /// The culture
        /// </summary>
        [NotNull]
        public readonly CultureInfo Culture;

        /// <summary>
        /// Gets the resource property.
        /// </summary>
        /// <value>
        /// The resource property.
        /// </value>
        [NotNull]
        public string ResourceProperty
        {
            get { return string.Join(".", ResourceTypeFullName, ResourceTag); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Translation" /> class.
        /// </summary>
        /// <param name="resourceTypeFullName">Full name of the resource type.</param>
        /// <param name="resourceTag">The resource tag.</param>
        /// <param name="message">The message format.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="resourceManager">The resource manager.</param>
        private Translation(
            [NotNull] string resourceTypeFullName,
            [NotNull] string resourceTag,
            [NotNull] string message,
            [NotNull] CultureInfo culture,
            [NotNull] ResourceManager resourceManager)
        {
            if (resourceTypeFullName == null) throw new ArgumentNullException("resourceTypeFullName");
            if (resourceTag == null) throw new ArgumentNullException("resourceTag");
            if (message == null) throw new ArgumentNullException("message");
            if (culture == null) throw new ArgumentNullException("culture");
            if (resourceManager == null) throw new ArgumentNullException("resourceManager");

            ResourceTypeFullName = resourceTypeFullName;
            ResourceTag = resourceTag;
            Message = message;
            Culture = culture;
            ResourceManager = resourceManager;
        }
    }
}