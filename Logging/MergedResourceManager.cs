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
using System.Collections;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Resources;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    /// Supports resources that have been merged into a new assembly.
    /// </summary>
    /// <remarks>See http://stackoverflow.com/questions/1952638/single-assembly-multi-language-windows-forms-deployment-ilmerge-and-satellite-a
    /// </remarks>
    internal class MergedResourceManager : ResourceManager
    {
        private readonly Type _contextTypeInfo;
        private CultureInfo _neutralResourcesCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedResourceManager"/> class.
        /// </summary>
        /// <param name="t">The t.</param>
        public MergedResourceManager([NotNull] Type t)
            : base(t)
        {
            _contextTypeInfo = t;
        }

        /// <summary>
        /// Provides the implementation for finding a resource set, however this overload looks in the current assembly before attempting to find satelite assemblies, which supports
        /// merged assemblies.
        /// </summary>
        /// <param name="culture">The culture object to look for.</param>
        /// <param name="createIfNotExists">true to load the resource set, if it has not been loaded yet; otherwise, false.</param>
        /// <param name="tryParents">true to check parent <see cref="T:System.Globalization.CultureInfo" /> objects if the resource set cannot be loaded; otherwise, false.</param>
        /// <returns>
        /// The specified resource set.
        /// </returns>
        protected override ResourceSet InternalGetResourceSet(
            [NotNull] CultureInfo culture,
            bool createIfNotExists,
            bool tryParents)
        {
            Contract.Assert(ResourceSets != null);
            Contract.Assert(MainAssembly != null);
            ResourceSet rs = (ResourceSet) ResourceSets[culture];
            if (rs != null) return rs;

            //lazy-load default language (without caring about duplicate assignment in race conditions, no harm done);
            if (_neutralResourcesCulture == null)
                _neutralResourcesCulture = GetNeutralResourcesLanguage(MainAssembly);

            // if we're asking for the default language, then ask for the
            // invariant (non-specific) resources.
            if (_neutralResourcesCulture.Equals(culture))
                culture = CultureInfo.InvariantCulture;
            string resourceFileName = GetResourceFileName(culture);

            Stream store = MainAssembly.GetManifestResourceStream(_contextTypeInfo, resourceFileName);

            //If we found the appropriate resources in the local assembly
            if (store != null)
            {
                rs = new ResourceSet(store);
                //save for later.
                AddResourceSet(ResourceSets, culture, ref rs);
            }
            else
                rs = base.InternalGetResourceSet(culture, createIfNotExists, tryParents);
            return rs;
        }

        //private method in framework, had to be re-specified here.
        private static void AddResourceSet(
            [NotNull] Hashtable localResourceSets,
            [NotNull] CultureInfo culture,
            [NotNull] ref ResourceSet rs)
        {
            lock (localResourceSets)
            {
                ResourceSet objA = (ResourceSet) localResourceSets[culture];
                if (objA != null)
                {
                    if (Equals(objA, rs)) return;
                    rs.Dispose();
                    rs = objA;
                }
                else
                    localResourceSets.Add(culture, rs);
            }
        }
    }
}