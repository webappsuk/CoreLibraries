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
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Can be used to convert <see cref="FormatBuilder"/> to/from <see cref="string"/> in configurations.
    /// </summary>
    [PublicAPI]
    public sealed class FormatBuilderConverter : ConfigurationConverterBase
    {
        /// <summary>
        /// Converts to.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="ci">The ci.</param>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public override object ConvertTo(
            [CanBeNull] ITypeDescriptorContext ctx,
            [CanBeNull] CultureInfo ci,
            [CanBeNull] object value,
            [CanBeNull] Type type)
        {
            return value != null ? value.ToString() : null;
        }

        /// <summary>
        /// Converts from.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="ci">The ci.</param>
        /// <param name="data">The data.</param>
        /// <returns>System.Object.</returns>
        public override object ConvertFrom(
            [CanBeNull] ITypeDescriptorContext ctx,
            [CanBeNull] CultureInfo ci,
            object data)
        {
            string format = data as string;
            return format == null
                ? null
                : new FormatBuilder(format);
        }
    }
}