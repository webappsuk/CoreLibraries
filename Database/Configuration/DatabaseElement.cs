using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   An element that represents a database.
    /// </summary>
    public partial class DatabaseElement : Utilities.Configuration.ConfigurationElement
    {
        /// <summary>
        ///   Gets or sets the identifier for a database.
        /// </summary>
        /// <value>
        ///   The identifier for this database.
        /// </value>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("id", IsRequired = true, IsKey = true)]
        [NotNull]
        public string Id
        {
            get { return GetProperty<string>("id"); }
            set { SetProperty("id", value); }
        }

        /// <summary>
        ///   Gets or sets a <see cref="bool"/> value indicating whether the database is enabled.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the database is enabled; otherwise <see langword="false"/>.
        /// </value>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
        public bool Enabled
        {
            get { return GetProperty<bool>("enabled"); }
            set { SetProperty("enabled", value); }
        }

        /// <summary>
        ///   Gets or sets the connections for this database.
        /// </summary>
        /// <value>The connections.</value>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("connections", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(LoadBalancedConnectionCollection))]
        [NotNull]
        public LoadBalancedConnectionCollection Connections
        {
            get { return GetProperty<LoadBalancedConnectionCollection>("connections"); }
            set { SetProperty("connections", value); }
        }

        /// <summary>
        ///   Gets or sets the <see cref="WebApplications.Utilities.Database.SqlProgram">programs</see> for this database.
        /// </summary>
        /// <value>
        ///   The stored procedures and functions for this database.
        /// </value>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("programs", IsRequired = false, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ProgramCollection), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
        [NotNull]
        public ProgramCollection Programs
        {
            get { return GetProperty<ProgramCollection>("programs"); }
            set { SetProperty("programs", value); }
        }

        /// <summary>
        ///   Used to initialize a default set of values for the <see cref="ConfigurationElement"/> object.
        /// </summary>
        /// <remarks>
        ///   Called to set the internal state to appropriate default values.
        /// </remarks>
        protected override void InitializeDefault()
        {
            // ReSharper disable ConstantNullCoalescingCondition
            Connections = Connections ?? new LoadBalancedConnectionCollection();
            Programs = Programs ?? new ProgramCollection();
            // ReSharper restore ConstantNullCoalescingCondition

            base.InitializeDefault();
        }

        /// <summary>
        ///   Gets the <see cref="WebApplications.Utilities.Database.SqlProgram"/> with the specified name and parameters,
        ///   respecting configured options.
        /// </summary>
        /// <param name="name">The name of the stored procedure or function.</param>
        /// <param name="parameters">The program parameters.</param>
        /// <param name="ignoreValidationErrors">
        ///   If set to <see langword="true"/> will ignore validation errors regardless of configuration.
        /// </param>
        /// <param name="checkOrder">
        ///   If set to <see langword="true"/> will check parameter order matches regardless of configuration.
        /// </param>
        /// <param name="defaultCommandTimeout">
        ///   <para>The default command timeout.</para>
        ///   <para>If set will override the configuration from <see cref="ProgramElement.DefaultCommandTimeout"/>.</para>
        /// </param>
        /// <param name="constraintMode">
        ///   <para>The constraint mode</para>
        ///   <para>If set will override the configuration from <see cref="ProgramElement.ConstraintMode"/>.</para>
        /// </param>
        /// <returns>The retrieved <see cref="WebApplications.Utilities.Database.SqlProgram"/>.</returns>
        /// <exception cref="LoggingException">
        ///   <para>Could not find a default load balanced connection for the database with this <see cref="Id"/>.</para>
        ///   <para>-or-</para>
        ///   <para>A parameter with no name map was found.</para>
        /// </exception>
        [NotNull]
        public SqlProgram GetSqlProgram(
            [NotNull] string name,
            [CanBeNull] IEnumerable<KeyValuePair<string, Type>> parameters = null,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode? constraintMode = null)
        {
            // Grab the default load balanced connection for the database.
            LoadBalancedConnectionElement connection = this.Connections.FirstOrDefault(c => c.Enabled);

            if (connection == null)
                throw new LoggingException(
                    Resources.DatabaseElement_GetSqlProgram_DefaultLoadBalanceConnectionNotFound,
                    LogLevel.Error, Id);
            
            // Look for program mapping information
            ProgramElement prog = this.Programs[name];
            if (prog != null)
            {
                // Check for name mapping
                if (!String.IsNullOrWhiteSpace(prog.MapTo))
                    name = prog.MapTo;

                // Set options if not already set.
                ignoreValidationErrors = ignoreValidationErrors && prog.IgnoreValidationErrors;
                checkOrder = checkOrder && prog.CheckOrder;
                defaultCommandTimeout = defaultCommandTimeout ?? prog.DefaultCommandTimeout;
                constraintMode = constraintMode ?? prog.ConstraintMode;

                if (!String.IsNullOrEmpty(prog.Connection))
                {
                    connection = this.Connections[prog.Connection];
                    if ((connection == null) ||
                        (!connection.Enabled))
                        throw new LoggingException(
                            Resources.DatabaseElement_GetSqlProgram_LoadBalanceConnectionNotFound,
                            LogLevel.Error, prog.Connection, Id, name);
                }

                // Check for parameter mappings
                if ((parameters != null) &&
                    prog.Parameters.Any())
                {
                    parameters = parameters
                        .Select(kvp =>
                        {
                            ParameterElement param = prog.Parameters[kvp.Key];
                            if (param == null) return kvp;
                            if (String.IsNullOrWhiteSpace(param.MapTo))
                                throw new LoggingException(
                                    Resources.DatabaseElement_GetSqlProgram_MappingNotSpecified,
                                    LogLevel.Error, kvp.Key, prog.Name);

                            return new KeyValuePair<string, Type>(param.MapTo, kvp.Value);
                        }).ToList();
                }
            }

            if (constraintMode == null) constraintMode = TypeConstraintMode.Warn;

            return new SqlProgram(connection, name, parameters, ignoreValidationErrors, checkOrder,
                                  defaultCommandTimeout, (TypeConstraintMode) constraintMode);
        }
    }
}