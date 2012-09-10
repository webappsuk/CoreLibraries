#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Collections.Generic;
using System.Configuration;
using JetBrains.Annotations;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   A configuration section used to specify database configurations.
    /// </summary>
    /// <seealso cref="T:WebApplications.Utilities.Configuration.ConfigurationSection`1"/>
    public partial class DatabasesConfiguration : ConfigurationSection<DatabasesConfiguration>
    {
        /// <summary>
        ///   Gets or sets the databases within the configuration section.
        /// </summary>
        /// <value>
        ///   The <see cref="DatabaseCollection">collection</see> of database elements.
        /// </value>
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof (DatabaseCollection),
            CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
        [NotNull]
        public DatabaseCollection Databases
        {
            get { return GetProperty<DatabaseCollection>(""); }
            set { SetProperty("", value); }
        }

        /// <summary>
        ///   Used to initialize a default set of values for the <see cref="DatabasesConfiguration"/> object.
        /// </summary>
        /// <remarks>
        ///   Called to set the internal state to appropriate default values.
        /// </remarks>
        protected override void InitializeDefault()
        {
            // ReSharper disable ConstantNullCoalescingCondition
            Databases = Databases ?? new DatabaseCollection();
            // ReSharper restore ConstantNullCoalescingCondition
            base.InitializeDefault();
        }

        /// <summary>
        ///   Gets the <see cref="WebApplications.Utilities.Database.SqlProgram"/> with the specified name and parameters,
        ///   respecting the active configured options.
        /// </summary>
        /// <param name="database">The database id.</param>
        /// <param name="name">The name of the stored procedure or function.</param>
        /// <param name="parameterNames">The parameter names.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <returns>
        ///   The retrieved <see cref="WebApplications.Utilities.Database.SqlProgram"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   The database corresponding to the ID provided in the <paramref name="database"/> parameter could not be found.
        /// </exception>
        [NotNull]
        protected static SqlProgram GetConfiguredSqlProgram(
            [NotNull] string database,
            [NotNull] string name,
            [NotNull] IEnumerable<string> parameterNames,
            [NotNull] params Type[] parameterTypes)
        {
            return Active.GetSqlProgram(database, name, SqlProgramDefinition.ToKVP(parameterNames, parameterTypes));
        }

        /// <summary>
        ///   Gets the <see cref="WebApplications.Utilities.Database.SqlProgram"/> with the specified name and parameters,
        ///   respecting the active configured options.
        /// </summary>
        /// <param name="database">The database id.</param>
        /// <param name="name">The name of the stored procedure or function.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="ignoreValidationErrors">
        ///   If set to <see langword="true"/> will ignore validation errors regardless of configuration.
        /// </param>
        /// <param name="checkOrder">
        ///   If set to <see langword="true"/> will check parameter order matches regardless of configuration.
        /// </param>
        /// <param name="defaultCommandTimeout">The default command timeout, if set will override the configuration.</param>
        /// <param name="constraintMode">
        ///   <para>The constraint mode</para>
        ///   <para>If set will override the configuration from <see cref="ProgramElement.ConstraintMode"/>.</para>
        /// </param>
        /// <returns>
        ///   The retrieved <see cref="WebApplications.Utilities.Database.SqlProgram"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   The database corresponding to the ID provided in the <paramref name="database"/> parameter could not be found.
        /// </exception>
        [NotNull]
        public static SqlProgram GetConfiguredSqlProgram(
            [NotNull] string database,
            [NotNull] string name,
            [CanBeNull] IEnumerable<KeyValuePair<string, Type>> parameters = null,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode? constraintMode = null)
        {
            return Active.GetSqlProgram(database, name, parameters, ignoreValidationErrors, checkOrder,
                                        defaultCommandTimeout, constraintMode);
        }

        /// <summary>
        ///   Gets the <see cref="WebApplications.Utilities.Database.SqlProgram"/> with the specified name and parameters,
        ///   respecting the active configured options.
        /// </summary>
        /// <param name="database">The database id.</param>
        /// <param name="name">The name of the stored procedure or function.</param>
        /// <param name="parameterNames">The parameter names.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <returns>
        ///   The retrieved <see cref="WebApplications.Utilities.Database.SqlProgram"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   The database corresponding to the ID provided in the <paramref name="database"/> parameter could not be found.
        /// </exception>
        [NotNull]
        protected SqlProgram GetSqlProgram(
            [NotNull] string database,
            [NotNull] string name,
            [NotNull] IEnumerable<string> parameterNames,
            [NotNull] params Type[] parameterTypes)
        {
            return GetSqlProgram(database, name, SqlProgramDefinition.ToKVP(parameterNames, parameterTypes));
        }

        /// <summary>
        ///   Gets the <see cref="WebApplications.Utilities.Database.SqlProgram"/> with the specified name and parameters,
        ///   respecting the active configured options.
        /// </summary>
        /// <param name="database">The database id.</param>
        /// <param name="name">The name of the stored procedure or function.</param>
        /// <param name="parameters">The program parameters.</param>
        /// <param name="ignoreValidationErrors">
        ///   If set to <see langword="true"/> will ignore validation errors regardless of configuration.
        /// </param>
        /// <param name="checkOrder">
        ///   If set to <see langword="true"/> will check parameter order matches regardless of configuration.
        /// </param>
        /// <param name="defaultCommandTimeout">The default command timeout, if set will override the configuration.</param>
        /// <param name="constraintMode">
        ///   <para>The constraint mode</para>
        ///   <para>If set will override the configuration from <see cref="ProgramElement.ConstraintMode"/>.</para>
        /// </param>
        /// <returns>
        ///   The retrieved <see cref="WebApplications.Utilities.Database.SqlProgram"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   The database corresponding to the ID provided in the <paramref name="database"/> parameter could not be found.
        /// </exception>
        [NotNull]
        public SqlProgram GetSqlProgram(
            [NotNull] string database,
            [NotNull] string name,
            [CanBeNull] IEnumerable<KeyValuePair<string, Type>> parameters = null,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode? constraintMode = null)
        {
            // We have to find the database otherwise we cannot get a load balanced connection.
            DatabaseElement db = Databases[database];
            if ((db == null) || (!db.Enabled))
                throw new LoggingException(Resources.DatabaseConfiguration_GetSqlProgram_DatabaseIdNotFound,
                                           LogLevel.Error, database);

            return db.GetSqlProgram(name, parameters, ignoreValidationErrors, checkOrder, defaultCommandTimeout,
                                    constraintMode);
        }
    }
}