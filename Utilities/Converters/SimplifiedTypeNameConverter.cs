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
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Security.Permissions;
using WebApplications.Utilities.Reflect;

namespace WebApplications.Utilities.Converters
{
    /// <summary>
    /// A <see cref="ConfigurationConverterBase"/> that provides simplified type names that are more robust for round-tripping.
    /// </summary>
    public sealed class SimplifiedTypeNameConverter : ConfigurationConverterBase
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentException"><paramref name="value"/> must be a <see cref="Type"/>.</exception>
        public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
        {
            if (value == null) return null;
            ExtendedType t = value as Type;
            if (t == null)
                throw new ArgumentException(Resources.SimplifiedTypeNameConverter_ConvertTo_Invalid_Type, nameof(value));

            return t.SimpleFullName;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException"><paramref name="data"/> must be the name of a valid <see cref="Type"/>.</exception>
        [ReflectionPermission(SecurityAction.Assert, Flags = ReflectionPermissionFlag.NoFlags)]
        public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
        {
            if (data == null) return null;
            string typeName = (string)data;
            // ReSharper disable ExceptionNotDocumented
            ExtendedType result = ExtendedType.FindType(typeName, false, true);
            // ReSharper restore ExceptionNotDocumented

            if (result == null)
                throw new ArgumentException(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    string.Format(Resources.SimplifiedTypeNameConverter_ConvertFrom_Unknown_Type, typeName),
                    nameof(data));

            return result;
        }
    }
}