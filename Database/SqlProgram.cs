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
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using NodaTime;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Database
{

    #region Delegates
    /// <summary>
    /// A delegate that accepts any method that takes a <see cref="SqlProgramCommand"/> and set's its parameters.
    /// </summary>
    /// <param name="command">The command.</param>
    public delegate void SetParametersDelegate([NotNull] SqlProgramCommand command);

    /// <summary>
    /// A delegate that accepts any method that accepts a <see cref="SqlDataReader"/>.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <remarks>The reader is disposed automatically after use, so should not be disposed by this method.</remarks>
    public delegate void ResultDelegate([NotNull] SqlDataReader reader);

    /// <summary>
    /// An asynchronous delegate that accepts any method that accepts a <see cref="SqlDataReader" />.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An awaitable task.</returns>
    /// <remarks>The reader is disposed automatically after use, so should not be disposed by this method.</remarks>
    [NotNull]
    public delegate Task ResultDelegateAsync([NotNull] SqlDataReader reader, CancellationToken cancellationToken);

    /// <summary>
    /// A delegate that accepts any method that accepts a <see cref="SqlDataReader" /> and returns a result.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="reader">The reader.</param>
    /// <returns>The result.</returns>
    /// <remarks>The reader is disposed automatically after use, so should not be disposed by this method.</remarks>
    [CanBeNull]
    public delegate T ResultDelegate<out T>([NotNull] SqlDataReader reader);

    /// <summary>
    /// An asynchronous delegate that accepts any method that accepts a <see cref="SqlDataReader" /> and returns a result.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An awaitable task, containing the result.</returns>
    /// <remarks>The reader is disposed automatically after use, so should not be disposed by this method.</remarks>
    [NotNull]
    public delegate Task<T> ResultDelegateAsync<T>([NotNull] SqlDataReader reader, CancellationToken cancellationToken);

    /// <summary>
    /// A delegate that accepts any method that accepts a <see cref="XmlReader"/>.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <remarks>The reader is disposed automatically after use, so should not be disposed by this method.</remarks>
    public delegate void XmlResultDelegate([NotNull] XmlReader reader);

    /// <summary>
    /// An asynchronous delegate that accepts any method that accepts a <see cref="XmlReader" />.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An awaitable task.</returns>
    /// <remarks>The reader is disposed automatically after use, so should not be disposed by this method.</remarks>
    [NotNull]
    public delegate Task XmlResultDelegateAsync([NotNull] XmlReader reader, CancellationToken cancellationToken);

    /// <summary>
    /// A delegate that accepts any method that accepts a <see cref="XmlReader" /> and returns a result.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="reader">The reader.</param>
    /// <returns>The result.</returns>
    /// <remarks>The reader is disposed automatically after use, so should not be disposed by this method.</remarks>
    [CanBeNull]
    public delegate T XmlResultDelegate<out T>([NotNull] XmlReader reader);

    /// <summary>
    /// An asynchronous delegate that accepts any method that accepts a <see cref="XmlReader" /> and returns a result.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An awaitable task, containing the result.</returns>
    /// <remarks>The reader is disposed automatically after use, so should not be disposed by this method.</remarks>
    [NotNull]
    public delegate Task<T> XmlResultDelegateAsync<T>([NotNull] XmlReader reader, CancellationToken cancellationToken);
    #endregion

    /// <summary>
    ///   Used to create an object for easy calling of stored procedures or functions in a database.
    /// </summary>
    [PublicAPI]
    public class SqlProgram
    {
        private class CurrentMapping
        {
            public readonly Instant Loaded;
            public readonly SqlProgramMapping Mapping;
            public readonly ExceptionDispatchInfo ExceptionDispatchInfo;

            /// <summary>
            /// Initializes a new instance of the <see cref="CurrentMapping"/> class.
            /// </summary>
            /// <param name="mapping">The mapping.</param>
            public CurrentMapping([NotNull] SqlProgramMapping mapping)
            {
                Loaded = TimeHelpers.Clock.Now;
                Mapping = mapping;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CurrentMapping"/> class.
            /// </summary>
            /// <param name="exceptionDispatchInfo">The exception dispatch information.</param>
            public CurrentMapping([NotNull] ExceptionDispatchInfo exceptionDispatchInfo)
            {
                Loaded = TimeHelpers.Clock.Now;
                ExceptionDispatchInfo = exceptionDispatchInfo;
            }
        }

        /// <summary>
        ///   The <see cref="TypeConstraintMode">type constraint mode</see>,
        ///   determines what happens if truncation or loss of precision occurs.
        /// </summary>
        public readonly TypeConstraintMode ConstraintMode;

        /// <summary>
        /// The connection.
        /// </summary>
        [NotNull]
        public readonly LoadBalancedConnection Connection;

        /// <summary>
        ///   The name of the stored procedure or function.
        /// </summary>
        [NotNull]
        public readonly string Name;

        /// <summary>
        ///   The parameters required by this <see cref="SqlProgram"/> (if specified).
        /// </summary>
        [NotNull]
        public readonly IEnumerable<KeyValuePair<string, Type>> Parameters;

        /// <summary>
        /// The parameter count
        /// </summary>
        public readonly int ParameterCount;

        /// <summary>
        ///  A lock object to prevent multiple validations at the same time.
        /// </summary>
        [NotNull]
        private readonly AsyncLock _validationLock = new AsyncLock();

        /// <summary>
        ///   The default command timeout to use.
        ///   This is the time to wait for the program to execute before generating an error.
        /// </summary>
        private TimeSpan _defaultCommandTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        ///   Gets or sets the default command timeout.
        ///   This is the time to wait for the program to execute before generating an error.
        /// </summary>
        /// <value>
        ///   The default command timeout, this is initialized to be 30 seconds by default.
        ///   When setting the default timeout if the value is less than <see cref="TimeSpan.Zero"/>
        ///   then this will be set to the original interval of 30 seconds.
        /// </value>
        public TimeSpan DefaultCommandTimeout
        {
            get { return _defaultCommandTimeout; }
            set
            {
                if (_defaultCommandTimeout == value)
                    return;
                _defaultCommandTimeout = value < TimeSpan.Zero
                    ? TimeSpan.FromSeconds(30)
                    : value;
            }
        }

        /// <summary>
        /// The connection mappings, map the program to a stored procedure on each connection.
        /// </summary>
        [NotNull]
        private readonly CurrentMapping[] _connectionMappings;

        /// <summary>
        /// The semaphore for controlling the maximum number of concurrent executions of this program.
        /// </summary>
        [CanBeNull]
        internal readonly AsyncSemaphore Semaphore;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram" /> class.
        /// </summary>
        /// <param name="connection">The load balanced connection.</param>
        /// <param name="name">The <see cref="Name">name</see> of the program.</param>
        /// <param name="parameters">The program <see cref="Parameters">parameters</see>.</param>
        /// <param name="defaultCommandTimeout"><para>The <see cref="DefaultCommandTimeout">default command timeout</see></para>
        /// <para>This is the time to wait for the command to execute.</para>
        /// <para>If set to <see langword="null" /> then the timeout will be 30 seconds.</para></param>
        /// <param name="constraintMode"><para>The type constraint mode.</para>
        /// <para>By default this is set to log a warning if truncation/loss of precision occurs.</para></param>
        protected SqlProgram(
            [NotNull] LoadBalancedConnection connection,
            [NotNull] string name,
            [CanBeNull] IEnumerable<KeyValuePair<string, Type>> parameters = null,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (name == null) throw new ArgumentNullException("name");

            Name = name;
            Connection = connection;
            if (connection.DatabaseId != null)
                Semaphore = ConcurrencyController.GetProgramSemaphore(connection.DatabaseId, name);

            DefaultCommandTimeout = (defaultCommandTimeout == null || defaultCommandTimeout < TimeSpan.Zero)
                ? TimeSpan.FromSeconds(30)
                : (TimeSpan)defaultCommandTimeout;

            ConstraintMode = constraintMode;

            if (parameters == null)
            {
                Parameters = Enumerable.Empty<KeyValuePair<string, Type>>();
                ParameterCount = 0;
            }
            else
            {
                KeyValuePair<string, Type>[] p = parameters.ToArray();
                ParameterCount = p.Length;
                Parameters = p;
            }

            _connectionMappings = new CurrentMapping[connection.Count()];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram" /> class.
        /// </summary>
        /// <param name="program">The base program (stored procedure/function).</param>
        /// <param name="parameters">The program <see cref="Parameters">parameters</see>.</param>
        /// <param name="defaultCommandTimeout"><para>The <see cref="DefaultCommandTimeout">default command timeout</see></para>
        /// <para>This is the time to wait for the command to execute.</para>
        /// <para>If set to <see langword="null" /> then the timeout the default timeout from the base program.</para></param>
        /// <param name="constraintMode">The type constraint mode, this defined the behavior when truncation/loss of precision occurs.</param>
        protected SqlProgram(
            [NotNull] SqlProgram program,
            [NotNull] IEnumerable<KeyValuePair<string, Type>> parameters,
            TimeSpan? defaultCommandTimeout,
            TypeConstraintMode constraintMode)
        {
            if (program == null) throw new ArgumentNullException("program");
            if (parameters == null) throw new ArgumentNullException("parameters");

            Connection = program.Connection;
            Name = program.Name;
            Semaphore = program.Semaphore;

            // If we have no specific default command timeout set it to the existing one for the program.
            if (defaultCommandTimeout == null)
                DefaultCommandTimeout = program.DefaultCommandTimeout;
            else
                DefaultCommandTimeout = (TimeSpan)defaultCommandTimeout;
            ConstraintMode = constraintMode;

            IReadOnlyCollection<KeyValuePair<string, Type>> p = parameters.Enumerate();
            ParameterCount = p.Count;
            Parameters = p;

            _connectionMappings = new CurrentMapping[Connection.Count];
            Array.Copy(program._connectionMappings, _connectionMappings, _connectionMappings.Length);
        }
        #endregion

        #region Create overloads
        /// <summary>
        /// Creates a new instance of the <see cref="SqlProgram" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="name">The <see cref="Name">name</see> of the program.</param>
        /// <param name="parameters">The program <see cref="Parameters">parameters</see>.</param>
        /// <param name="ignoreValidationErrors"><para>If set to <see langword="true" /> then don't throw any parameter validation errors.</para>
        /// <para>By default this is set to <see langword="false" />.</para></param>
        /// <param name="checkOrder"><para>If set to <see langword="true" /> then check the order of the <see paramref="parameters" />.</para>
        /// <para>By default this is set to <see langword="false" />.</para></param>
        /// <param name="defaultCommandTimeout"><para>The <see cref="DefaultCommandTimeout">default command timeout</see></para>
        /// <para>This is the time to wait for the command to execute.</para>
        /// <para>If set to <see langword="null" /> then the timeout will be 30 seconds.</para></param>
        /// <param name="constraintMode"><para>The type constraint mode.</para>
        /// <para>By default this is set to log a warning if truncation/loss of precision occurs.</para></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task resulting in the <see cref="SqlProgram" />.</returns>
        [NotNull]
        public static Task<SqlProgram> Create(
            [NotNull] Connection connection,
            [NotNull] string name,
            [CanBeNull] IEnumerable<KeyValuePair<string, Type>> parameters = null,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            [CanBeNull] TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (connection == null) throw new ArgumentNullException("connection");
            return Create(
                new LoadBalancedConnection(connection),
                name,
                parameters,
                ignoreValidationErrors,
                checkOrder,
                defaultCommandTimeout,
                constraintMode,
                cancellationToken);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram" /> class.
        /// </summary>
        /// <param name="connection">The load balanced connection.</param>
        /// <param name="name">The <see cref="Name">name</see> of the program.</param>
        /// <param name="parameters">The program <see cref="Parameters">parameters</see>.</param>
        /// <param name="ignoreValidationErrors"><para>If set to <see langword="true" /> then don't throw any parameter validation errors.</para>
        /// <para>By default this is set to <see langword="false" />.</para></param>
        /// <param name="checkOrder"><para>If set to <see langword="true" /> then check the order of the <see paramref="parameters" />.</para>
        /// <para>By default this is set to <see langword="false" />.</para></param>
        /// <param name="defaultCommandTimeout"><para>The <see cref="DefaultCommandTimeout">default command timeout</see></para>
        /// <para>This is the time to wait for the command to execute.</para>
        /// <para>If set to <see langword="null" /> then the timeout will be 30 seconds.</para></param>
        /// <param name="constraintMode"><para>The type constraint mode.</para>
        /// <para>By default this is set to log a warning if truncation/loss of precision occurs.</para></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task resulting in the <see cref="SqlProgram" />.</returns>
        [NotNull]
        public static async Task<SqlProgram> Create(
            [NotNull] LoadBalancedConnection connection,
            [NotNull] string name,
            [CanBeNull] IEnumerable<KeyValuePair<string, Type>> parameters = null,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SqlProgram program = new SqlProgram(
                connection,
                name,
                parameters,
                defaultCommandTimeout,
                constraintMode);

            // Validate
            await program.Validate(checkOrder, false, !ignoreValidationErrors, cancellationToken).ConfigureAwait(false);

            return program;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram" /> class.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="connection">The load balanced connection.</param>
        /// <param name="name">The program name.</param>
        /// <param name="ignoreValidationErrors">If set to <see langword="true" /> then don't throw any parameter validation errors.</param>
        /// <param name="defaultCommandTimeout"><para>The <see cref="DefaultCommandTimeout">default command timeout</see></para>
        /// <para>This is the time to wait for the command to execute.</para>
        /// <para>If set to <see langword="null" /> then the timeout will be 30 seconds.</para></param>
        /// <param name="constraintMode">The type constraint mode, this defined the behavior when truncation/loss of precision occurs.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <returns>Task&lt;SqlProgram&gt;.</returns>
        /// <exception cref="LoggingException"><para>No <paramref name="connection" /> specified.</para>
        /// <para>-or-</para>
        /// <para>No program <paramref name="name" /> specified.</para>
        /// <para>-or-</para>
        /// <para>Invalid program definition.</para></exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameterTypes" /> is <see langword="null" />.</exception>
        [NotNull]
        protected internal Task<SqlProgram> Create(
            CancellationToken cancellationToken,
            [NotNull] LoadBalancedConnection connection,
            [NotNull] string name,
            bool ignoreValidationErrors,
            [CanBeNull] TimeSpan? defaultCommandTimeout,
            TypeConstraintMode constraintMode,
            [NotNull] params Type[] parameterTypes)
        {
            if (parameterTypes == null) throw new ArgumentNullException("parameterTypes");
            return Create(
                connection,
                name,
                parameterTypes.Select(t => new KeyValuePair<string, Type>(null, t)),
                ignoreValidationErrors,
                true,
                defaultCommandTimeout,
                constraintMode,
                cancellationToken);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram" /> class.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="connection">The load balanced connection.</param>
        /// <param name="name">The program name.</param>
        /// <param name="ignoreValidationErrors">If set to <see langword="true" /> then don't throw any parameter validation errors.</param>
        /// <param name="checkOrder">If set to <see langword="true" /> then check the order of the parameters.</param>
        /// <param name="defaultCommandTimeout"><para>The <see cref="DefaultCommandTimeout">default command timeout</see></para>
        /// <para>This is the time to wait for the command to execute.</para>
        /// <para>If set to <see langword="null" /> then the timeout will be 30 seconds.</para></param>
        /// <param name="constraintMode">The type constraint mode, this defined the behavior when truncation/loss of precision occurs.</param>
        /// <param name="parameterNames">The parameter names.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <returns>Task&lt;SqlProgram&gt;.</returns>
        /// <exception cref="LoggingException"><para>The supplied <paramref name="parameterNames" /> count did not match the <paramref name="parameterTypes" /> count.</para>
        /// <para>-or-</para>
        /// <para>No <paramref name="connection" /> specified.</para>
        /// <para>-or-</para>
        /// <para>No program <paramref name="name" /> specified.</para>
        /// <para>-or-</para>
        /// <para>Invalid program definition.</para></exception>
        [NotNull]
        protected internal Task<SqlProgram> Create(
            CancellationToken cancellationToken,
            [NotNull] LoadBalancedConnection connection,
            [NotNull] string name,
            bool ignoreValidationErrors,
            bool checkOrder,
            TimeSpan? defaultCommandTimeout,
            TypeConstraintMode constraintMode,
            [NotNull] IEnumerable<string> parameterNames,
            [NotNull] params Type[] parameterTypes)
        {
            if (parameterNames == null) throw new ArgumentNullException("parameterNames");
            if (parameterTypes == null) throw new ArgumentNullException("parameterTypes");

            return Create(
                connection,
                name,
                SqlProgramDefinition.ToKvp(parameterNames, parameterTypes),
                ignoreValidationErrors,
                checkOrder,
                defaultCommandTimeout,
                constraintMode,
                cancellationToken);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram" /> class.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="program">The base program (stored procedure/function).</param>
        /// <param name="ignoreValidationErrors">If set to <see langword="true" /> then don't throw any parameter validation errors.</param>
        /// <param name="defaultCommandTimeout"><para>The <see cref="DefaultCommandTimeout">default command timeout</see></para>
        /// <para>This is the time to wait for the command to execute.</para>
        /// <para>If set to <see langword="null" /> then the timeout the default timeout from the base program.</para></param>
        /// <param name="constraintMode">The type constraint mode, this defined the behavior when truncation/loss of precision occurs.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <returns>SqlProgram.</returns>
        /// <exception cref="LoggingException">Invalid parameters</exception>
        [NotNull]
        protected internal static async Task<SqlProgram> Create(
            CancellationToken cancellationToken,
            [NotNull] SqlProgram program,
            bool ignoreValidationErrors,
            TimeSpan? defaultCommandTimeout,
            TypeConstraintMode constraintMode,
            [NotNull] params Type[] parameterTypes)
        {
            if (parameterTypes == null) throw new ArgumentNullException("parameterTypes");

            SqlProgram newProgram = new SqlProgram(
                program,
                parameterTypes.Select(t => new KeyValuePair<string, Type>(null, t)),
                defaultCommandTimeout,
                constraintMode);

            // Validate
            await newProgram.Validate(true, false, !ignoreValidationErrors, cancellationToken).ConfigureAwait(false);

            return newProgram;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram" /> class.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="program">The base program (stored procedure/function).</param>
        /// <param name="ignoreValidationErrors">If set to <see langword="true" /> then don't throw any parameter validation errors.</param>
        /// <param name="checkOrder"><para>If set to <see langword="true" /> then check the order of the <see paramref="parameters" />.</para>
        /// <para>By default this is set to <see langword="false" />.</para></param>
        /// <param name="defaultCommandTimeout"><para>The <see cref="DefaultCommandTimeout">default command timeout</see></para>
        /// <para>This is the time to wait for the command to execute.</para>
        /// <para>If set to <see langword="null" /> then the timeout the default timeout from the base program.</para></param>
        /// <param name="constraintMode">The type constraint mode, this defined the behavior when truncation/loss of precision occurs.</param>
        /// <param name="parameterNames">The parameter names.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <returns>SqlProgram.</returns>
        /// <exception cref="LoggingException">Invalid parameters</exception>
        [NotNull]
        protected internal static async Task<SqlProgram> Create(
            CancellationToken cancellationToken,
            [NotNull] SqlProgram program,
            bool ignoreValidationErrors,
            bool checkOrder,
            TimeSpan? defaultCommandTimeout,
            TypeConstraintMode constraintMode,
            [NotNull] IEnumerable<string> parameterNames,
            [NotNull] params Type[] parameterTypes)
        {
            if (parameterNames == null) throw new ArgumentNullException("parameterNames");
            if (parameterTypes == null) throw new ArgumentNullException("parameterTypes");

            SqlProgram newProgram = new SqlProgram(
                program,
                SqlProgramDefinition.ToKvp(parameterNames, parameterTypes),
                defaultCommandTimeout,
                constraintMode);

            // Validate
            await newProgram.Validate(checkOrder, false, !ignoreValidationErrors, cancellationToken)
                .ConfigureAwait(false);

            return newProgram;
        }
        #endregion

        /// <summary>
        /// Gets the valid <see cref="SqlProgramMapping">mappings</see> for the <see cref="SqlProgram"/>.
        /// </summary>
        [NotNull]
        public IEnumerable<SqlProgramMapping> Mappings
        {
            get
            {
                return _connectionMappings.Where(m => m != null && m.ExceptionDispatchInfo == null)
                    .Select(m => m.Mapping)
                    .ToArray();
            }
        }

        /// <summary>
        /// Re-validates the <see cref="SqlProgram">SQL Program</see>, throwing any errors.
        /// </summary>
        /// <param name="checkOrder"><para>If set to <see langword="true"/> then check the order of the parameters.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <param name="forceSchemaReload">If set to <see langword="true"/> forces a schema reload.</param>
        /// <param name="throwOnError">If <see langword="true"/> throws any validation exceptions.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Any <see cref="LoggingException">validation errors</see> that occurred (if any).</returns>
        [NotNull]
        public async Task<IEnumerable<LoggingException>> Validate(
            bool checkOrder = false,
            bool forceSchemaReload = false,
            bool throwOnError = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Instant requested = TimeHelpers.Clock.Now;

            using (await _validationLock.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
                ExceptionDispatchInfo[] errors = (await
                    Task.WhenAll(
                        Connection.Select(
                            (c, i) => Validate(requested, i, c, checkOrder, forceSchemaReload, cancellationToken)))
                        .ConfigureAwait(false))
                    .Select(m => m.ExceptionDispatchInfo)
                    .Where(e => e != null)
                    .ToArray();
                // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
                Debug.Assert(errors != null);

                if (errors.Length < 1) return Enumerable.Empty<LoggingException>();

                if (!throwOnError)
                    // ReSharper disable once PossibleNullReferenceException
                    return errors.Select(edi => edi.SourceException as LoggingException).Where(l => l != null);

                if (errors.Length == 1)
                    // ReSharper disable once PossibleNullReferenceException
                    errors[0].Throw();

                // Throw aggregate exception.
                // ReSharper disable once PossibleNullReferenceException
                throw new AggregateException(errors.Select(edi => edi.SourceException));
            }
        }

        /// <summary>
        /// Validates the specified connection.
        /// </summary>
        /// <param name="requested">The requested.</param>
        /// <param name="index">The connection index.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="checkOrder">if set to <see langword="true" /> [check order].</param>
        /// <param name="forceSchemaReload">if set to <see langword="true" /> [force schema reload].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task resulting in the current mapping.</returns>
        /// <exception cref="WebApplications.Utilities.Logging.LoggingException"></exception>
        [NotNull]
        private async Task<CurrentMapping> Validate(
            Instant requested,
            int index,
            [NotNull] Connection connection,
            bool checkOrder,
            bool forceSchemaReload,
            CancellationToken cancellationToken)
        {
            Debug.Assert(connection != null);

            // Get the current mapping.
            CurrentMapping mapping = _connectionMappings[index];
            if (mapping == null ||
                (mapping.Loaded < requested))
            {
                try
                {
                    // Grab the schema for the connection string.
                    DatabaseSchema schema =
                        await DatabaseSchema.GetOrAdd(connection, forceSchemaReload, cancellationToken)
                            .ConfigureAwait(false);

                    cancellationToken.ThrowIfCancellationRequested();
                    Debug.Assert(schema != null);

                    // Find the program
                    SqlProgramDefinition programDefinition;
                    if (!schema.ProgramDefinitionsByName.TryGetValue(Name, out programDefinition))
                        throw new LoggingException(
                            LoggingLevel.Critical,
                            () => Resources.SqlProgram_Validate_DefinitionsNotFound,
                            Name);
                    Debug.Assert(programDefinition != null);

                    // Validate parameters
                    IEnumerable<SqlProgramParameter> parameters = programDefinition.ValidateParameters(
                        Parameters,
                        checkOrder);

                    mapping = new CurrentMapping(new SqlProgramMapping(connection, programDefinition, parameters));
                }
                catch (LoggingException loggingException)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    mapping = new CurrentMapping(ExceptionDispatchInfo.Capture(loggingException));
                }
                catch (Exception exception)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    mapping =
                        new CurrentMapping(
                            ExceptionDispatchInfo.Capture(
                                new LoggingException(exception, () => Resources.SqlProgram_Validate_Fatal, Name)));
                }

                _connectionMappings[index] = mapping;
            }

            Debug.Assert(mapping != null);
            return mapping;
        }

        /// <summary>
        ///   Creates a new command against a random connection.  This should always be used within a using statement.
        /// </summary>
        /// <param name="timeout">
        ///   <para>The time to wait for an executing program before throwing an error.</para>
        ///   <para>If <see langword="null"/> is specified then <see cref="DefaultCommandTimeout"/> is used.</para>
        /// </param>
        /// <returns>A <see cref="SqlProgramCommand"/>.</returns>
        [NotNull]
        public SqlProgramCommand CreateCommand(TimeSpan? timeout = null)
        {
            // Select a random valid mapping.
            // ReSharper disable once PossibleNullReferenceException
            SqlProgramMapping sqlProgramMapping = Mappings.Choose(mapping => mapping.Connection.Weight);
            if (sqlProgramMapping == null)
                throw new LoggingException(() => Resources.SqlProgram_CreateCommand_No_Mapping, Name);

            return new SqlProgramCommand(
                this,
                sqlProgramMapping,
                (timeout == null || timeout < TimeSpan.Zero)
                    ? DefaultCommandTimeout
                    : (TimeSpan)timeout);
        }

        /// <summary>
        ///   Creates the commands for all connections in a load balanced connection.
        /// </summary>
        /// <param name="timeout">
        ///   <para>The time to wait for an executing program before throwing an error.</para>
        ///   <para>If <see langword="null"/> or less than <see cref="TimeSpan.Zero"/> then <see cref="DefaultCommandTimeout"/> is used.</para>
        /// </param>
        /// <returns>An enumeration of <see cref="SqlProgramCommand"/> (one for each connection).</returns>
        [NotNull]
        public IEnumerable<SqlProgramCommand> CreateCommandsForAllConnections(TimeSpan? timeout = null)
        {
            TimeSpan t = (timeout == null || timeout < TimeSpan.Zero)
                ? DefaultCommandTimeout
                : (TimeSpan)timeout;
            // ReSharper disable once PossibleNullReferenceException
            return Mappings
                // ReSharper disable once AssignNullToNotNullAttribute
                .Select(mapping => new SqlProgramCommand(this, mapping, t))
                .ToArray();
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   <para>A <see cref="string"/> representation of this instance.</para>
        ///   <para><b>Format:</b> <see cref="Name"/> + "Program".</para>
        /// </returns>
        public override string ToString()
        {
            return Name + " Program";
        }

        #region ExecuteScalar and ExecuteScalarAll overloads
        /// <summary>
        ///   Executes the query, and returns the first column of the first row in the result set returned by the query.
        ///   Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <returns>The scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [CanBeNull]
        public T ExecuteScalar<T>()
        {
            SqlProgramCommand command = CreateCommand();
            return command.ExecuteScalar<T>();
        }

        /// <summary>
        ///   Executes the query against each connection, and returns the first column of the first row in the result set returned by the query.
        ///   Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <returns>
        ///   An enumerable containing the scalar values returned by each connection.
        /// </returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="InvalidOperationException">
        ///   The connection did not specify a source or server.
        /// </exception>
        /// <exception cref="SqlException">
        ///   A connection-level error occurred whilst opening the connection.
        ///   See <see cref="SqlConnection.Open"/> for more details.
        /// </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [NotNull]
        public IEnumerable<T> ExecuteScalarAll<T>()
        {
            // ReSharper disable once PossibleNullReferenceException
            return CreateCommandsForAllConnections().Select(command => command.ExecuteScalar<T>()).ToArray();
        }

        /// <summary>
        ///   Executes the query, and returns the first column of the first row in the result set returned by the query.
        ///   Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <returns>The scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [CanBeNull]
        public T ExecuteScalar<T>([NotNull] SetParametersDelegate setParameters)
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");

            SqlProgramCommand command = CreateCommand();
            setParameters(command);
            return command.ExecuteScalar<T>();
        }

        /// <summary>
        ///   Executes the query against each connection, and returns the first column of the first row in the result set returned by the query.
        ///   Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <returns>
        ///   An enumerable containing the scalar values returned by each connection.
        /// </returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="InvalidOperationException">
        ///   The connection did not specify a source or server.
        /// </exception>
        /// <exception cref="SqlException">
        ///   A connection-level error occurred whilst opening the connection.
        ///   See <see cref="SqlConnection.Open"/> for more details.
        /// </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [NotNull]
        public IEnumerable<T> ExecuteScalarAll<T>([NotNull] SetParametersDelegate setParameters)
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");

            // ReSharper disable PossibleNullReferenceException
            return CreateCommandsForAllConnections().Select(
                command =>
                {
                    Debug.Assert(command != null);
                    setParameters(command);
                    return command.ExecuteScalar<T>();
                })
                .ToArray();
            // ReSharper restore PossibleNullReferenceException
        }
        #endregion

        #region ExecuteScalarAsync and ExecuteScalarAllAsync overloads
        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query.
        /// Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task containing the resulting scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="LoggingException">Failed to select a valid connection string from the <see cref="LoadBalancedConnection"/>.</exception>
        /// <exception cref="InvalidOperationException">The connection did not specify a source or server.</exception>
        /// <exception cref="SqlException">A connection-level error occurred whilst opening the connection.
        /// See <see cref="SqlConnection.Open"/> for more details.</exception>
        /// 
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [NotNull]
        public Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
        {
            SqlProgramCommand command = CreateCommand();
            return command.ExecuteScalarAsync<T>(cancellationToken);
        }

        /// <summary>
        /// Executes the query against each connection, and returns the first column of the first row in the result set returned by the query.
        /// Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task&lt;TResult&gt;"/> containing an enumerable of the scalar values returned by each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// 
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [NotNull]
        public Task<IEnumerable<T>> ExecuteScalarAllAsync<T>(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return
                Task.WhenAll(
                    CreateCommandsForAllConnections()
                        .Select(command => command.ExecuteScalarAsync<T>(cancellationToken)))
                    .ContinueWith(
                        t => (IEnumerable<T>)t.Result,
                        cancellationToken,
                        TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.Current);
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task containing the resulting scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="LoggingException">Failed to select a valid connection string from the <see cref="LoadBalancedConnection" />.</exception>
        /// <exception cref="InvalidOperationException">The connection did not specify a source or server.</exception>
        /// <exception cref="SqlException">A connection-level error occurred whilst opening the connection.
        /// See <see cref="SqlConnection.Open" /> for more details.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<T> ExecuteScalarAsync<T>(
            [NotNull] SetParametersDelegate setParameters,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");

            SqlProgramCommand command = CreateCommand();
            setParameters(command);
            return command.ExecuteScalarAsync<T>(cancellationToken);
        }

        /// <summary>
        /// Executes the query against each connection, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The output type expected.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task&lt;TResult&gt;" /> containing an enumerable of the scalar values returned by each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="SqlException">A connection-level error occurred.
        /// See <see cref="SqlConnection.Open" />.</exception>
        /// <exception cref="SqlProgramExecutionException">An exception occurred whilst trying to set the parameters to the command.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<IEnumerable<T>> ExecuteScalarAllAsync<T>(
            [NotNull] SetParametersDelegate setParameters,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return Task.WhenAll(
                CreateCommandsForAllConnections()
                    .Select(
                        command =>
                        {
                            setParameters(command);
                            return command.ExecuteScalarAsync<T>(cancellationToken);
                        }))
                .ContinueWith(
                    t => (IEnumerable<T>)t.Result,
                    cancellationToken,
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.Current);
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }
        #endregion

        #region ExecuteNonQuery and ExecuteNonQueryAll overloads
        /// <summary>
        ///   Executes a Transact-SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public int ExecuteNonQuery()
        {
            SqlProgramCommand command = CreateCommand();
            return command.ExecuteNonQuery();
        }

        /// <summary>
        ///   Executes a Transact-SQL statement against every connection and returns the number of rows affected for each connection.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [NotNull]
        public IEnumerable<int> ExecuteNonQueryAll()
        {
            // ReSharper disable once PossibleNullReferenceException
            return CreateCommandsForAllConnections().Select(command => command.ExecuteNonQuery()).ToArray();
        }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public int ExecuteNonQuery([NotNull] SetParametersDelegate setParameters)
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");

            SqlProgramCommand command = CreateCommand();
            setParameters(command);
            return command.ExecuteNonQuery();
        }

        /// <summary>
        ///   Executes a Transact-SQL statement against every connection and returns the number of rows affected for each connection.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <returns>An enumerable containing the number of rows affected for each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [NotNull]
        public IEnumerable<int> ExecuteNonQueryAll([NotNull] SetParametersDelegate setParameters)
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");

            // ReSharper disable PossibleNullReferenceException
            return CreateCommandsForAllConnections().Select(
                command =>
                {
                    Debug.Assert(command != null);
                    setParameters(command);
                    return command.ExecuteNonQuery();
                })
                .ToArray();
            // ReSharper restore PossibleNullReferenceException
        }
        #endregion

        #region ExecuteNonQueryAsync and ExecuteNonQueryAllAsync overloads
        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            SqlProgramCommand command = CreateCommand();
            return command.ExecuteNonQueryAsync(cancellationToken);
        }

        /// <summary>
        /// Executes a Transact-SQL statement against every connection and returns the number of rows affected for each connection.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<IEnumerable<int>> ExecuteNonQueryAllAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return
                Task.WhenAll(
                    CreateCommandsForAllConnections()
                        .Select(command => command.ExecuteNonQueryAsync(cancellationToken)))
                    .ContinueWith(
                        t => (IEnumerable<int>)t.Result,
                        cancellationToken,
                        TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.Current);
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="LoggingException">Failed to select a valid connection string from the <see cref="LoadBalancedConnection" />.</exception>
        /// <exception cref="InvalidOperationException">The connection did not specify a source or server.</exception>
        /// <exception cref="SqlException">A connection-level error occurred whilst opening the connection.
        /// See <see cref="SqlConnection.Open" /> for more details.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<int> ExecuteNonQueryAsync(
            [NotNull] SetParametersDelegate setParameters,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");

            SqlProgramCommand command = CreateCommand();
            setParameters(command);
            return command.ExecuteNonQueryAsync(cancellationToken);
        }

        /// <summary>
        /// Executes a Transact-SQL statement against every connection and returns the number of rows affected for each connection.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<IEnumerable<int>> ExecuteNonQueryAllAsync(
            [NotNull] SetParametersDelegate setParameters,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return Task.WhenAll(
                CreateCommandsForAllConnections()
                    .Select(
                        command =>
                        {
                            setParameters(command);
                            return command.ExecuteNonQueryAsync(cancellationToken);
                        }))
                .ContinueWith(
                    t => (IEnumerable<int>)t.Result,
                    cancellationToken,
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.Current);
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }
        #endregion

        #region ExecuteReader and ExecuteReaderAll overloads
        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public void ExecuteReader()
        {
            SqlProgramCommand command = CreateCommand();
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Executes the program with the specified parameters against every connection.
        /// </summary>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void ExecuteReaderAll()
        {
            // ReSharper disable once PossibleNullReferenceException, ReturnValueOfPureMethodIsNotUsed
            foreach (SqlProgramCommand command in CreateCommandsForAllConnections())
                command.ExecuteNonQuery();
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <returns>The scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [CanBeNull]
        public T ExecuteReader<T>()
        {
            SqlProgramCommand command = CreateCommand();
            return command.ExecuteScalar<T>();
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <returns>An enumerable containing the scalar value returned from each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public IEnumerable<T> ExecuteReaderAll<T>()
        {
            // ReSharper disable once PossibleNullReferenceException
            return CreateCommandsForAllConnections().Select(command => command.ExecuteScalar<T>()).ToArray();
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="resultAction">The action to use to process the results.</param>
        /// <param name="behavior">The behaviour of the results and how they effect the database.</param>
        /// <exception cref="SqlProgramExecutionException">A
        /// An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void ExecuteReader(
            [NotNull] ResultDelegate resultAction,
            CommandBehavior behavior = CommandBehavior.Default)
        {
            if (resultAction == null) throw new ArgumentNullException("resultAction");
            SqlProgramCommand command = CreateCommand();
            command.ExecuteReader(resultAction, behavior);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="resultAction">The action to use to process the results.</param>
        /// <param name="behavior">The behaviour of the results and how they effect the database.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void ExecuteReaderAll(
            [NotNull] ResultDelegate resultAction,
            CommandBehavior behavior = CommandBehavior.Default)
        {
            if (resultAction == null) throw new ArgumentNullException("resultAction");
            foreach (SqlProgramCommand command in CreateCommandsForAllConnections())
                // ReSharper disable once PossibleNullReferenceException
                command.ExecuteReader(resultAction, behavior);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="resultFunc">The function to use to process the results.</param>
        /// <param name="behavior">The behaviour of the results and how they effect the database.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [CanBeNull]
        public T ExecuteReader<T>(
            [NotNull] ResultDelegate<T> resultFunc,
            CommandBehavior behavior = CommandBehavior.Default)
        {
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");
            SqlProgramCommand command = CreateCommand();
            return command.ExecuteReader(resultFunc, behavior);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="resultFunc">The function to use to process the results.</param>
        /// <param name="behavior">The behaviour of the results and how they effect the database.</param>
        /// <returns>An enumerable containing the results from all connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="InvalidOperationException">The connection did not specify a source or server.</exception>
        /// <exception cref="SqlException">A connection-level error occurred whilst opening the connection.
        /// See <see cref="SqlConnection.Open" /> for more details.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [CanBeNull]
        public IEnumerable<T> ExecuteReaderAll<T>(
            [NotNull] ResultDelegate<T> resultFunc,
            CommandBehavior behavior = CommandBehavior.Default)
        {
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");
            // ReSharper disable once PossibleNullReferenceException
            return
                CreateCommandsForAllConnections()
                    .Select(command => command.ExecuteReader(resultFunc, behavior))
                    .ToArray();
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultAction">The action to use to process the results.</param>
        /// <param name="behavior">The behaviour of the results and how they effect the database.</param>
        /// <exception cref="LoggingException">An error occurred executing the program.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void ExecuteReader(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] ResultDelegate resultAction,
            CommandBehavior behavior = CommandBehavior.Default)
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultAction == null) throw new ArgumentNullException("resultAction");

            SqlProgramCommand command = CreateCommand();
            setParameters(command);
            command.ExecuteReader(resultAction, behavior);
        }

        /// <summary>
        /// Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultAction">The action to use to process the results.</param>
        /// <param name="behavior">The behaviour of the results and how they effect the database.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void ExecuteReaderAll(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] ResultDelegate resultAction,
            CommandBehavior behavior = CommandBehavior.Default)
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultAction == null) throw new ArgumentNullException("resultAction");

            foreach (SqlProgramCommand command in CreateCommandsForAllConnections())
            {
                Debug.Assert(command != null);
                setParameters(command);
                command.ExecuteReader(resultAction, behavior);
            }
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type to return from the result function</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultFunc">The function to use to process the results.</param>
        /// <param name="behavior">The behaviour of the results and how they effect the database.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [CanBeNull]
        public T ExecuteReader<T>(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] ResultDelegate<T> resultFunc,
            CommandBehavior behavior = CommandBehavior.Default)
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");

            SqlProgramCommand command = CreateCommand();
            setParameters(command);
            return command.ExecuteReader(resultFunc, behavior);
        }

        /// <summary>
        /// Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultFunc">The function to use to process the results.</param>
        /// <param name="behavior">The behaviour of the results and how they effect the database.</param>
        /// <returns>An enumerable containing the results from all the connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="InvalidOperationException">The connection did not specify a source or server.</exception>
        /// <exception cref="SqlException">A connection-level error occurred whilst opening the connection.
        /// See <see cref="SqlConnection.Open" /> for more details.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public IEnumerable<T> ExecuteReaderAll<T>(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] ResultDelegate<T> resultFunc,
            CommandBehavior behavior = CommandBehavior.Default)
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");

            foreach (SqlProgramCommand command in CreateCommandsForAllConnections())
            {
                Debug.Assert(command != null);
                setParameters(command);
                yield return command.ExecuteReader(resultFunc, behavior);
            }
        }
        #endregion

        #region ExecuteReaderAsync and ExecuteReaderAllAsync overloads
        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="LoggingException">Failed to select a valid connection string from the <see cref="LoadBalancedConnection" />.</exception>
        /// <exception cref="InvalidOperationException">The connection did not specify a source or server.</exception>
        /// <exception cref="SqlException">A connection-level error occurred whilst opening the connection.
        /// See <see cref="SqlConnection.Open" /> for more details.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task ExecuteReaderAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            SqlProgramCommand command = CreateCommand();
            return command.ExecuteNonQueryAsync(cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task ExecuteReaderAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return
                Task.WhenAll(
                    CreateCommandsForAllConnections()
                        .Select(command => command.ExecuteNonQueryAsync(cancellationToken)))
                    .ContinueWith(
                        t => (IEnumerable<int>)t.Result,
                        cancellationToken,
                        TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.Current);
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<T> ExecuteReaderAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
        {
            SqlProgramCommand command = CreateCommand();
            return command.ExecuteScalarAsync<T>(cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An enumerable containing the results from all connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<IEnumerable<T>> ExecuteReaderAllAsync<T>(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return
                Task.WhenAll(
                    CreateCommandsForAllConnections()
                        .Select(command => command.ExecuteScalarAsync<T>(cancellationToken)))
                    .ContinueWith(
                        t => (IEnumerable<T>)t.Result,
                        cancellationToken,
                        TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.Current);
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task ExecuteReaderAsync(
            [NotNull] ResultDelegateAsync resultAction,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultAction == null) throw new ArgumentNullException("resultAction");

            SqlProgramCommand command = CreateCommand();
            return command.ExecuteReaderAsync(resultAction, behavior, cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task ExecuteReaderAllAsync(
            [NotNull] ResultDelegateAsync resultAction,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultAction == null) throw new ArgumentNullException("resultAction");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return
                Task.WhenAll(
                    CreateCommandsForAllConnections()
                        .Select(command => command.ExecuteReaderAsync(resultAction, behavior, cancellationToken)));
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<T> ExecuteReaderAsync<T>(
            [NotNull] ResultDelegateAsync<T> resultFunc,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");

            SqlProgramCommand command = CreateCommand();
            return command.ExecuteReaderAsync(resultFunc, behavior, cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An enumerable containing the results from all the connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<IEnumerable<T>> ExecuteReaderAllAsync<T>(
            [NotNull] ResultDelegateAsync<T> resultFunc,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return
                Task.WhenAll(
                    CreateCommandsForAllConnections()
                        .Select(command => command.ExecuteReaderAsync(resultFunc, behavior, cancellationToken)))
                    .ContinueWith(
                        t => (IEnumerable<T>)t.Result,
                        cancellationToken,
                        TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.Current);
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task ExecuteReaderAsync(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] ResultDelegateAsync resultAction,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultAction == null) throw new ArgumentNullException("resultAction");

            SqlProgramCommand command = CreateCommand();
            setParameters(command);
            return command.ExecuteReaderAsync(resultAction, behavior, cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task ExecuteReaderAllAsync(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] ResultDelegateAsync resultAction,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultAction == null) throw new ArgumentNullException("resultAction");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return
                Task.WhenAll(
                    CreateCommandsForAllConnections()
                        .Select(
                            command =>
                            {
                                setParameters(command);
                                return command.ExecuteReaderAsync(resultAction, behavior, cancellationToken);
                            }));
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type to return from the result function</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<T> ExecuteReaderAsync<T>(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] ResultDelegateAsync<T> resultFunc,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");

            SqlProgramCommand command = CreateCommand();
            setParameters(command);
            return command.ExecuteReaderAsync(resultFunc, behavior, cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An enumerable containing the results from all connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<IEnumerable<T>> ExecuteReaderAllAsync<T>(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] ResultDelegateAsync<T> resultFunc,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return Task.WhenAll(
                CreateCommandsForAllConnections()
                    .Select(
                        command =>
                        {
                            setParameters(command);
                            return command.ExecuteReaderAsync(resultFunc, behavior, cancellationToken);
                        }))
                .ContinueWith(
                    t => (IEnumerable<T>)t.Result,
                    cancellationToken,
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.Current);
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }
        #endregion

        #region ExecuteXmlReader and ExecuteXmlReaderAll overloads
        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public void ExecuteXmlReader()
        {
            SqlProgramCommand command = CreateCommand();
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Executes the program with the specified parameters against every connection.
        /// </summary>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void ExecuteXmlReaderAll()
        {
            // ReSharper disable once PossibleNullReferenceException, ReturnValueOfPureMethodIsNotUsed
            foreach (SqlProgramCommand command in CreateCommandsForAllConnections())
                command.ExecuteNonQuery();
        }

        /// <summary>
        ///   Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <returns>The scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/>
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [CanBeNull]
        public T ExecuteXmlReader<T>()
        {
            SqlProgramCommand command = CreateCommand();
            return command.ExecuteScalar<T>();
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <returns>An enumerable containing the scalar value returned from each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public IEnumerable<T> ExecuteXmlReaderAll<T>()
        {
            // ReSharper disable once PossibleNullReferenceException
            return CreateCommandsForAllConnections().Select(command => command.ExecuteScalar<T>()).ToArray();
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="resultAction">The action to use to process the results.</param>
        /// <exception cref="SqlProgramExecutionException">A
        /// An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void ExecuteXmlReader([NotNull] XmlResultDelegate resultAction)
        {
            if (resultAction == null) throw new ArgumentNullException("resultAction");

            SqlProgramCommand command = CreateCommand();
            command.ExecuteXmlReader(resultAction);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="resultAction">The action to use to process the results.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void ExecuteXmlReaderAll([NotNull] XmlResultDelegate resultAction)
        {
            if (resultAction == null) throw new ArgumentNullException("resultAction");

            foreach (SqlProgramCommand command in CreateCommandsForAllConnections())
                // ReSharper disable once PossibleNullReferenceException
                command.ExecuteXmlReader(resultAction);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="resultFunc">The function to use to process the results.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [CanBeNull]
        public T ExecuteXmlReader<T>([NotNull] XmlResultDelegate<T> resultFunc)
        {
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");

            SqlProgramCommand command = CreateCommand();
            return command.ExecuteXmlReader(resultFunc);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="resultFunc">The function to use to process the results.</param>
        /// <returns>An enumerable containing the results from all connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="InvalidOperationException">The connection did not specify a source or server.</exception>
        /// <exception cref="SqlException">A connection-level error occurred whilst opening the connection.
        /// See <see cref="SqlConnection.Open" /> for more details.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [CanBeNull]
        public IEnumerable<T> ExecuteXmlReaderAll<T>([NotNull] XmlResultDelegate<T> resultFunc)
        {
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");

            // ReSharper disable once PossibleNullReferenceException
            return CreateCommandsForAllConnections().Select(command => command.ExecuteXmlReader(resultFunc)).ToArray();
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultAction">The action to use to process the results.</param>
        /// <exception cref="LoggingException">An error occurred executing the program.</exception>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void ExecuteXmlReader(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] XmlResultDelegate resultAction)
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultAction == null) throw new ArgumentNullException("resultAction");

            SqlProgramCommand command = CreateCommand();
            setParameters(command);
            command.ExecuteXmlReader(resultAction);
        }

        /// <summary>
        /// Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultAction">The action to use to process the results.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void ExecuteXmlReaderAll(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] XmlResultDelegate resultAction)
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultAction == null) throw new ArgumentNullException("resultAction");

            foreach (SqlProgramCommand command in CreateCommandsForAllConnections())
            {
                Debug.Assert(command != null);
                setParameters(command);
                command.ExecuteXmlReader(resultAction);
            }
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type to return from the result function</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultFunc">The function to use to process the results.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [CanBeNull]
        public T ExecuteXmlReader<T>(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] XmlResultDelegate<T> resultFunc)
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");

            SqlProgramCommand command = CreateCommand();
            setParameters(command);
            return command.ExecuteXmlReader(resultFunc);
        }

        /// <summary>
        /// Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultFunc">The function to use to process the results.</param>
        /// <returns>An enumerable containing the results from all the connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="InvalidOperationException">The connection did not specify a source or server.</exception>
        /// <exception cref="SqlException">A connection-level error occurred whilst opening the connection.
        /// See <see cref="SqlConnection.Open" /> for more details.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public IEnumerable<T> ExecuteXmlReaderAll<T>(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] XmlResultDelegate<T> resultFunc)
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");

            foreach (SqlProgramCommand command in CreateCommandsForAllConnections())
            {
                Debug.Assert(command != null);
                setParameters(command);
                yield return command.ExecuteXmlReader(resultFunc);
            }
        }
        #endregion

        #region ExecuteXmlReaderAsync and ExecuteXmlReaderAllAsync overloads
        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <exception cref="LoggingException">Failed to select a valid connection string from the <see cref="LoadBalancedConnection" />.</exception>
        /// <exception cref="InvalidOperationException">The connection did not specify a source or server.</exception>
        /// <exception cref="SqlException">A connection-level error occurred whilst opening the connection.
        /// See <see cref="SqlConnection.Open" /> for more details.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task ExecuteXmlReaderAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            SqlProgramCommand command = CreateCommand();
            return command.ExecuteNonQueryAsync(cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task ExecuteXmlReaderAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return
                Task.WhenAll(
                    CreateCommandsForAllConnections()
                        .Select(command => command.ExecuteNonQueryAsync(cancellationToken)))
                    .ContinueWith(
                        t => (IEnumerable<int>)t.Result,
                        cancellationToken,
                        TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.Current);
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<T> ExecuteXmlReaderAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
        {
            SqlProgramCommand command = CreateCommand();
            return command.ExecuteScalarAsync<T>(cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An enumerable containing the results from all connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<IEnumerable<T>> ExecuteXmlReaderAllAsync<T>(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return
                Task.WhenAll(
                    CreateCommandsForAllConnections()
                        .Select(command => command.ExecuteScalarAsync<T>(cancellationToken)))
                    .ContinueWith(
                        t => (IEnumerable<T>)t.Result,
                        cancellationToken,
                        TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.Current);
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task ExecuteXmlReaderAsync(
            [NotNull] XmlResultDelegateAsync resultAction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultAction == null) throw new ArgumentNullException("resultAction");

            SqlProgramCommand command = CreateCommand();
            return command.ExecuteXmlReaderAsync(resultAction, cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task ExecuteXmlReaderAllAsync(
            [NotNull] XmlResultDelegateAsync resultAction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultAction == null) throw new ArgumentNullException("resultAction");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return Task.WhenAll(
                CreateCommandsForAllConnections()
                    .Select(command => command.ExecuteXmlReaderAsync(resultAction, cancellationToken)));
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<T> ExecuteXmlReaderAsync<T>(
            [NotNull] XmlResultDelegateAsync<T> resultFunc,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");

            SqlProgramCommand command = CreateCommand();
            return command.ExecuteXmlReaderAsync(resultFunc, cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters against all connections.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An enumerable containing the results from all the connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<IEnumerable<T>> ExecuteXmlReaderAllAsync<T>(
            [NotNull] XmlResultDelegateAsync<T> resultFunc,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return Task.WhenAll(
                CreateCommandsForAllConnections()
                    .Select(command => command.ExecuteXmlReaderAsync(resultFunc, cancellationToken)))
                .ContinueWith(
                    t => (IEnumerable<T>)t.Result,
                    cancellationToken,
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.Current);
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task ExecuteXmlReaderAsync(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] XmlResultDelegateAsync resultAction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultAction == null) throw new ArgumentNullException("resultAction");

            SqlProgramCommand command = CreateCommand();
            setParameters(command);
            return command.ExecuteXmlReaderAsync(resultAction, cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task ExecuteXmlReaderAllAsync(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] XmlResultDelegateAsync resultAction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultAction == null) throw new ArgumentNullException("resultAction");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return Task.WhenAll(
                CreateCommandsForAllConnections()
                    .Select(
                        command =>
                        {
                            setParameters(command);
                            return command.ExecuteXmlReaderAsync(resultAction, cancellationToken);
                        }));
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type to return from the result function</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<T> ExecuteXmlReaderAsync<T>(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] XmlResultDelegateAsync<T> resultFunc,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");

            SqlProgramCommand command = CreateCommand();
            setParameters(command);
            return command.ExecuteXmlReaderAsync(resultFunc, cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="setParameters">The action to set the command parameters.</param>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An enumerable containing the results from all connections.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [NotNull]
        public Task<IEnumerable<T>> ExecuteXmlReaderAllAsync<T>(
            [NotNull] SetParametersDelegate setParameters,
            [NotNull] XmlResultDelegateAsync<T> resultFunc,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (setParameters == null) throw new ArgumentNullException("setParameters");
            if (resultFunc == null) throw new ArgumentNullException("resultFunc");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return
                Task.WhenAll(
                    CreateCommandsForAllConnections()
                        .Select(
                            command =>
                            {
                                setParameters(command);
                                return command.ExecuteXmlReaderAsync(resultFunc, cancellationToken);
                            }))
                    .ContinueWith(
                        t => (IEnumerable<T>)t.Result,
                        cancellationToken,
                        TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.Current);
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }
        #endregion
    }
}