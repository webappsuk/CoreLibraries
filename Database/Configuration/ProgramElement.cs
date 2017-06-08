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

using System.Configuration;
using System.Xml.Linq;
using NodaTime;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Configuration.Validators;
using WebApplications.Utilities.Database.Schema;
using ConfigurationElement = WebApplications.Utilities.Configuration.ConfigurationElement;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   Allows specifying of a <see cref="WebApplications.Utilities.Database.SqlProgram"/> in a configuration.
    /// </summary>
    public class ProgramElement : ConfigurationElement
    {
        private const string NameName = "name";
        private const string MapToName = "mapTo";
        private const string SelectFromName = "selectFrom";
        private const string TextPathName = "textPath";
        private const string TextName = "text";
        private const string ConnectionName = "connection";
        private const string IgnoreValidationErrorsName = "ignoreValidationErrors";
        private const string CheckOrderName = "checkOrder";
        private const string DefaultCommandTimeoutName = "defaultCommandTimeout";
        private const string ConstraintModeName = "constraintMode";
        private const string MaxConcurrencyName = "maxConcurrency";

        [NotNull]
        private static readonly XName _textXName = TextName;

        /// <summary>
        ///   Gets a name for the <see cref="WebApplications.Utilities.Database.SqlProgram"/>.
        /// </summary>
        /// <value>
        ///   The name of the <see cref="WebApplications.Utilities.Database.SqlProgram"/>.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty(NameName, IsRequired = true, IsKey = true)]
        [NotNull]
        public string Name
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            get => GetProperty<string>(NameName);
            set => SetProperty(NameName, value);
        }

        /// <summary>
        ///   Gets or sets the name of the stored procedure or function that this program maps to.
        /// </summary>
        /// <remarks>
        /// Only one of <see cref="MapTo"/>, <see cref="SelectFrom"/>, <see cref="TextPath"/>, and <see cref="Text"/>
        /// can have a value at any time. Setting this property to a non-null value will set the other properties to <see langword="null"/>.
        /// </remarks>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty(MapToName, DefaultValue = null, IsRequired = false)]
        [CanBeNull]
        public string MapTo
        {
            get => GetProperty<string>(MapToName);
            set => SetImplProperty(MapToName, value);
        }

        /// <summary>
        ///   Gets or sets the name of the table that this program should select data from.
        /// </summary>
        /// <remarks>
        /// Only one of <see cref="MapTo"/>, <see cref="SelectFrom"/>, <see cref="TextPath"/>, and <see cref="Text"/>
        /// can have a value at any time. Setting this property to a non-null value will set the other properties to <see langword="null"/>.
        /// </remarks>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty(SelectFromName, DefaultValue = null, IsRequired = false)]
        [CanBeNull]
        public string SelectFrom
        {
            get => GetProperty<string>(SelectFromName);
            set => SetImplProperty(SelectFromName, value);
        }

        /// <summary>
        ///   Gets or sets the path to the file containing the raw text that the program should execute.
        /// </summary>
        /// <remarks>
        /// Only one of <see cref="MapTo"/>, <see cref="SelectFrom"/>, <see cref="TextPath"/>, and <see cref="Text"/>
        /// can have a value at any time. Setting this property to a non-null value will set the other properties to <see langword="null"/>.
        /// </remarks>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty(TextPathName, DefaultValue = null, IsRequired = false)]
        [CanBeNull]
        public string TextPath
        {
            get => GetProperty<string>(TextPathName);
            set => SetImplProperty(TextPathName, value);
        }

        /// <summary>
        /// Gets or sets the raw text that the program should execute.
        /// </summary>
        /// <remarks>
        /// Only one of <see cref="MapTo"/>, <see cref="SelectFrom"/>, <see cref="TextPath"/>, and <see cref="Text"/>
        /// can have a value at any time. Setting this property to a non-null value will set the other properties to <see langword="null"/>.
        /// </remarks>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [CanBeNull]
        public string Text
        {
            get => GetElement(_textXName)?.Value;
            set => SetImplProperty(TextName, value);
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
        [ConfigurationProperty(ConnectionName, DefaultValue = null, IsRequired = false)]
        [CanBeNull]
        public string Connection
        {
            get => GetProperty<string>(ConnectionName);
            set => SetProperty(ConnectionName, value);
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
        [ConfigurationCollection(typeof(ParameterCollection),
            CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
        [NotNull]
        [ItemNotNull]
        public ParameterCollection Parameters
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            get => GetProperty<ParameterCollection>(string.Empty);
            set => SetProperty(string.Empty, value);
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
        [ConfigurationProperty(IgnoreValidationErrorsName, DefaultValue = false, IsRequired = false)]
        public bool IgnoreValidationErrors
        {
            get => GetProperty<bool>(IgnoreValidationErrorsName);
            set => SetProperty(IgnoreValidationErrorsName, value);
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
        [ConfigurationProperty(CheckOrderName, DefaultValue = false, IsRequired = false)]
        public bool CheckOrder
        {
            get => GetProperty<bool>(CheckOrderName);
            set => SetProperty(CheckOrderName, value);
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
        [ConfigurationProperty(DefaultCommandTimeoutName, DefaultValue = "00:00:30", IsRequired = false)]
        [DurationValidator(MinValueString = "00:00:00.5", MaxValueString = "00:10:00")]
        public Duration DefaultCommandTimeout
        {
            get => GetProperty<Duration>(DefaultCommandTimeoutName);
            set => SetProperty(DefaultCommandTimeoutName, value);
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
        [ConfigurationProperty(ConstraintModeName, DefaultValue = TypeConstraintMode.Warn, IsRequired = false)]
        public TypeConstraintMode ConstraintMode
        {
            get => GetProperty<TypeConstraintMode>(ConstraintModeName);
            set => SetProperty(ConstraintModeName, value);
        }

        /// <summary>
        /// Gets or sets the maximum number of concurrent executions that are allowed for this program.
        /// </summary>
        /// <value>
        /// The maximum concurrency.
        /// </value>
        /// <remarks>A negative value indicates no limit.</remarks>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty(MaxConcurrencyName, DefaultValue = -1, IsRequired = false)]
        public int MaximumConcurrency
        {
            get => GetProperty<int>(MaxConcurrencyName);
            set => SetProperty(MaxConcurrencyName, value);
        }

        /// <summary>
        /// Sets the implementation property given, nulling out the other properties.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        private void SetImplProperty([NotNull] string name, string value)
        {
            // If the value is null, only set the single property
            if (string.IsNullOrWhiteSpace(value))
            {
                SetProperty(name, value);
                return;
            }

            string mapTo = null;
            string selectFrom = null;
            string textPath = null;
            string text = null;
            bool cdata = false;

            switch (name)
            {
                case MapToName:
                    mapTo = value;
                    break;
                case SelectFromName:
                    selectFrom = value;
                    break;
                case TextPathName:
                    textPath = value;
                    break;
                case TextName:
                    text = value;
                    // If the text contains any <, > or & characters, it should be wrapped in CDATA
                    cdata = text.IndexOfAny("<>&".ToCharArray()) >= 0;
                    break;
                default:
                    SetProperty(name, value);
                    return;
            }

            SetProperty(MapToName, mapTo);
            SetProperty(SelectFromName, selectFrom);
            SetProperty(TextPathName, textPath);
            if (string.IsNullOrWhiteSpace(text))
                RemoveElement(_textXName);
            else
                SetElement(_textXName, new XElement(_textXName, cdata ? (object)new XCData(text) : text));
        }

        /// <summary>Called after deserialization.</summary>
        protected override void PostDeserialize()
        {
            base.PostDeserialize();

            int notNullCount =
                (string.IsNullOrWhiteSpace(MapTo) ? 0 : 1) +
                (string.IsNullOrWhiteSpace(SelectFrom) ? 0 : 1) +
                (string.IsNullOrWhiteSpace(TextPath) ? 0 : 1) +
                (string.IsNullOrWhiteSpace(Text) ? 0 : 1);

            if (notNullCount > 1)
                throw new ConfigurationErrorsException(Resources.ProgramElement_PostDeserialize_MultipleSpecified);
        }
    }
}