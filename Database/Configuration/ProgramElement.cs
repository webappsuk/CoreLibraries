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
using System.Configuration;
using JetBrains.Annotations;
using WebApplications.Utilities.Database.Schema;
using ConfigurationElement = WebApplications.Utilities.Configuration.ConfigurationElement;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   Allows specifying of a <see cref="WebApplications.Utilities.Database.SqlProgram"/> in a configuration.
    /// </summary>
    public class ProgramElement : ConfigurationElement
    {
        /// <summary>
        ///   Gets a name for the <see cref="WebApplications.Utilities.Database.SqlProgram"/>.
        /// </summary>
        /// <value>
        ///   The name of the <see cref="WebApplications.Utilities.Database.SqlProgram"/>.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        [NotNull]
        public string Name
        {
            get { return GetProperty<string>("name"); }
            set { SetProperty("name", value); }
        }

        /// <summary>
        ///   Gets or sets the mapTo property, which maps the element to the
        ///   <see cref="WebApplications.Utilities.Database.SqlProgram"/> it represents.
        /// </summary>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("mapTo", DefaultValue = null, IsRequired = false)]
        [CanBeNull]
        public string MapTo
        {
            get { return GetProperty<string>("mapTo"); }
            set { SetProperty("mapTo", value); }
        }

        /// <summary>
        ///   Gets the name of the connection element to use.
        /// </summary>
        /// <value>
        ///   The name of the connection element to use.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("connection", DefaultValue = null, IsRequired = false)]
        [CanBeNull]
        public string Connection
        {
            get { return GetProperty<string>("connection"); }
            set { SetProperty("connection", value); }
        }

        /// <summary>
        ///   Gets or sets the parameters for the <see cref="WebApplications.Utilities.Database.SqlProgram"/>.
        /// </summary>
        /// <value>
        ///   The parameters for the <see cref="WebApplications.Utilities.Database.SqlProgram"/>.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("", IsRequired = false, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof (ParameterCollection),
            CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
        [NotNull]
        public ParameterCollection Parameters
        {
            get { return GetProperty<ParameterCollection>(""); }
            set { SetProperty("", value); }
        }

        /// <summary>
        ///   Gets or sets a <see cref="bool"/> value indicating whether validation errors should be ignored.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if validation errors should be ignored; otherwise <see langword="false"/>.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("ignoreValidationErrors", DefaultValue = false, IsRequired = false)]
        public bool IgnoreValidationErrors
        {
            get { return GetProperty<bool>("ignoreValidationErrors"); }
            set { SetProperty("ignoreValidationErrors", value); }
        }

        /// <summary>
        ///   Gets or sets a <see cref="bool"/> value indicating whether parameter order should be ignored.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the parameter order should be ignored; otherwise <see langword="false"/>.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("checkOrder", DefaultValue = false, IsRequired = false)]
        public bool CheckOrder
        {
            get { return GetProperty<bool>("checkOrder"); }
            set { SetProperty("checkOrder", value); }
        }

        /// <summary>
        ///   Gets the default command timeout.
        /// </summary>
        /// <value>
        ///   The time to wait before terminating the command.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("defaultCommandTimeout", DefaultValue = "00:00:30", IsRequired = false)]
        [TimeSpanValidator(MinValueString = "00:00:00.5", MaxValueString = "00:10:00")]
        [UsedImplicitly]
        public TimeSpan DefaultCommandTimeout
        {
            get { return GetProperty<TimeSpan>("defaultCommandTimeout"); }
            set { SetProperty("defaultCommandTimeout", value); }
        }

        /// <summary>
        ///   Gets the <see cref="TypeConstraintMode">constraint mode</see>.
        /// </summary>
        /// <value>
        ///   The <see cref="TypeConstraintMode">constraint mode</see>.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("constraintMode", DefaultValue = TypeConstraintMode.Warn, IsRequired = false)]
        [UsedImplicitly]
        public TypeConstraintMode ConstraintMode
        {
            get { return GetProperty<TypeConstraintMode>("constraintMode"); }
            set { SetProperty("constraintMode", value); }
        }

        /// <summary>
        ///   Used to initialize a default set of values for the <see cref="ProgramElement"/> object.
        /// </summary>
        /// <remarks>
        ///   Called to set the internal state to appropriate default values.
        /// </remarks>
        protected override void InitializeDefault()
        {
            // ReSharper disable ConstantNullCoalescingCondition
            Parameters = Parameters ?? new ParameterCollection();
            // ReSharper restore ConstantNullCoalescingCondition
            base.InitializeDefault();
        }
    }
}