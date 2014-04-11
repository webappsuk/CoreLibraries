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
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    /// Handles resource translation for logs.
    /// </summary>
    public static class Translation
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
        [PublicAPI]
        public static CultureInfo DefaultCulture
        {
            get { return _defaultCulture; }
            set
            {
                Contract.Requires(value != null);
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
        [PublicAPI]
        [CanBeNull]
        public static ResourceManager GetResourceManager<TResource>()
        {
            return GetResourceManager(typeof (TResource));
        }

        /// <summary>
        /// Registers the type for resource retrieval, and returns the <see cref="ResourceManager"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        [PublicAPI]
        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResourceManager GetResourceManager([NotNull] Type type)
        {
            Contract.Requires(type != null);
            Contract.Requires(type.FullName != null);
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
        [PublicAPI]
        [CanBeNull]
        public static ResourceManager GetResourceManager([NotNull] string typeFullName)
        {
            Contract.Requires(typeFullName != null);
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
        [PublicAPI]
        [CanBeNull]
        public static string GetResource<TResource>([NotNull] string property)
        {
            string translation;
            return TryGetResource(typeof (TResource), property, DefaultCulture, out translation) ? translation : null;
        }

        /// <summary>
        /// Gets the resource's value, in the <see cref="DefaultCulture">default culture</see>.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="property">The property.</param>
        /// <returns>
        /// The resource's value in the specified <see paramref="culture" />; otherwise <see langword="null" />.
        /// </returns>
        [PublicAPI]
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
        [PublicAPI]
        [CanBeNull]
        public static string GetResource<TResource>([NotNull] string property, [CanBeNull] CultureInfo culture)
        {
            string translation;
            return TryGetResource(typeof (TResource), property, culture, out translation) ? translation : null;
        }

        /// <summary>
        /// Gets the resource's value, in the <see paramref="culture">specified culture</see>.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="property">The property.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The resource's value in the specified <see paramref="culture"/>; otherwise <see langword="null"/>.</returns>
        [PublicAPI]
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
        [PublicAPI]
        public static bool TryGetResource<TResource>([NotNull] string property, out string translation)
        {
            return TryGetResource(typeof (TResource), property, DefaultCulture, out translation);
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
        [PublicAPI]
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
        [PublicAPI]
        public static bool TryGetResource<TResource>(
            [NotNull] string property,
            [CanBeNull] CultureInfo culture,
            out string translation)
        {
            return TryGetResource(typeof (TResource), property, culture, out translation);
        }

        /// <summary>
        /// Tries to get the resource value, in the <see paramref="culture">specified culture</see>, for the given property name.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="property">The property.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="translation">The translation.</param>
        /// <returns><see langword="true"/> if the resource was found; otherwise <see langword="false"/>.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetResource(
            [NotNull] Type resourceType,
            [NotNull] string property,
            [CanBeNull] CultureInfo culture,
            out string translation)
        {
            Contract.Requires(resourceType != null);
            Contract.Requires(property != null);
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
        [PublicAPI]
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
        [PublicAPI]
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
        /// Tries to get the resource value, in the <see cref="DefaultCulture">default culture</see>, for the given property name.
        /// </summary>
        /// <param name="typeFullName">Full name of the type.</param>
        /// <param name="property">The property.</param>
        /// <param name="translation">The translation.</param>
        /// <returns>
        ///   <see langword="true" /> if the resource was found; otherwise <see langword="false" />.
        /// </returns>
        /// <remarks>This will only retrieve resource values for previously registered types.</remarks>
        [PublicAPI]
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
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetResource(
            [NotNull] string typeFullName,
            [NotNull] string property,
            [CanBeNull] CultureInfo culture,
            out string translation)
        {
            Contract.Requires(typeFullName != null);
            Contract.Requires(property != null);
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
    }
}