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

using NodaTime;
using System.ComponentModel;
using System.Net.Mail;
using System.Security;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Converters;

namespace WebApplications.Utilities
{
    /// <summary>
    ///   This type cannot be private, as it must be callable from the actual Module Initializer.
    /// </summary>
    [PublicAPI]
    internal static class ModuleInitializer
    {
        /// <summary>
        ///   This method must not be private and must be <see langword="static"/>. 
        ///   Any return value is ignored.
        /// </summary>
        /// <remarks>
        ///   Include initialization code here that will run when the library is first loaded,
        ///   and before any element of the library is used.
        /// </remarks>
        internal static void Initialize()
        {
            // Add type converters to the NodaTime types
            TypeDescriptor.AddAttributes(typeof(Instant), new TypeConverterAttribute(typeof(InstantConverter)));
            TypeDescriptor.AddAttributes(typeof(ZonedDateTime), new TypeConverterAttribute(typeof(ZonedDateTimeConverter)));
            TypeDescriptor.AddAttributes(typeof(LocalDateTime), new TypeConverterAttribute(typeof(LocalDateTimeConverter)));
            TypeDescriptor.AddAttributes(typeof(Duration), new TypeConverterAttribute(typeof(DurationConverter)));
            TypeDescriptor.AddAttributes(typeof(Period), new TypeConverterAttribute(typeof(PeriodConverter)));
            TypeDescriptor.AddAttributes(typeof(CalendarSystem), new TypeConverterAttribute(typeof(CalendarSystemConverter)));
            TypeDescriptor.AddAttributes(typeof(DateTimeZone), new TypeConverterAttribute(typeof(DateTimeZoneConverter)));
            TypeDescriptor.AddAttributes(typeof(XNamespace), new TypeConverterAttribute(typeof(XNamespaceConverter)));
            TypeDescriptor.AddAttributes(typeof(MailAddress), new TypeConverterAttribute(typeof(MailAddressConverter)));
            TypeDescriptor.AddAttributes(typeof(MailAddressCollection), new TypeConverterAttribute(typeof(MailAddressCollectionConverter)));
            TypeDescriptor.AddAttributes(typeof(SecureString), new TypeConverterAttribute(typeof(SecureStringConverter)));
        }
    }
}