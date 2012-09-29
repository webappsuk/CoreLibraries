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
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Caching;
using WebApplications.Utilities.Logging.Performance;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   An operation contains details of the currently executing command against a stack.
    /// </summary>
    [Serializable]
    public sealed class Operation : IDisposable, IEqualityComparer<Operation>, IEquatable<Operation>
    {
        [NonSerialized] private const string NodeOperation = "Operation";
        [NonSerialized] private const string NodeArguments = "Arguments";
        [NonSerialized] private const string NodeArgument = "Argument";
        [NonSerialized] private const string NodeMethod = "Method";
        [NonSerialized] private const string NodeThread = "Thread";
        [NonSerialized] private const string AttributeId = "id";
        [NonSerialized] private const string AttributeGuid = "guid";
        [NonSerialized] private const string AttributeKey = "key";
        [NonSerialized] private const string AttributeName = "name";
        [NonSerialized] private const string AttributeType = "type";
        [NonSerialized] private const string AttributeParent = "parent";

        /// <summary>
        ///   The context stack used for operations.
        /// </summary>
        private static readonly ContextStack<Operation> _contextStack = new ContextStack<Operation>();

        /// <summary>
        ///   Holds a weak reference to all operations in memory.
        /// </summary>
        private static readonly WeakConcurrentDictionary<CombGuid, Operation> _operations =
            new WeakConcurrentDictionary<CombGuid, Operation>(allowResurrection: true);

        /// <summary>
        ///   The category name.
        /// </summary>
        [NotNull] public readonly string CategoryName;

        /// <summary>
        ///   The time the operation was <see cref="CombGuid.Created">created</see>.
        /// </summary>
        public DateTime Created
        {
            get { return Guid.Created; }
        }

        /// <summary>
        ///   The unique ID for the current operation.
        /// </summary>
        public readonly CombGuid Guid;

        /// <summary>
        ///   The instance of the object on which the operation was started (if a not a <see langword="static"/> method).
        /// </summary>
        public readonly int? InstanceHash;

        /// <summary>
        ///   The method that initiated the operation.
        /// </summary>
        [NotNull] public readonly string Method = String.Empty;

        /// <summary>
        ///   The operation name.
        /// </summary>
        [NotNull] public readonly string Name = String.Empty;

        /// <summary>
        ///   The thread the operation began on.
        /// </summary>
        public readonly int ThreadId;

        /// <summary>
        ///   The <see cref="Thread.Name">name</see> (or the ID if no name is set) of the thread the operation began on.
        /// </summary>
        [NotNull] public readonly string ThreadName;

        /// <summary>
        ///   Gets the current operations arguments.
        /// </summary>
        /// <value>The arguments.</value>
        [NotNull] private readonly List<KeyValuePair<string, string>> _arguments;

        /// <summary>
        ///   The disposer for cleaning up the extended thread context.
        /// </summary>
        [NonSerialized] private IDisposable _contextStackDisposer;

        /// <summary>
        ///   Holds A reference to the parent <see cref="Operation"/>.
        /// </summary>
        [NonSerialized] private Operation _parent;

        /// <summary>
        ///   Holds the parent Guid which can be used in serialization to prevent the entire
        ///   object graph being serialized.
        /// </summary>
        private CombGuid _parentGuid;

        /// <summary>
        ///   Caches the <see cref="string"/> representation of the base operation, to prevent regeneration.
        /// </summary>
        [NonSerialized] private string _string;

        /// <summary>
        ///   Times the operation duration.
        /// </summary>
        [NonSerialized] private PerformanceTimer.Timer _timer;

        /// <summary>
        ///   Caches the XML representation of the log item, to prevent regeneration.
        /// </summary>
        [NonSerialized] private XElement _xml;

        /// <summary>
        ///   Wraps the specified <see cref="Action"/> in an <see cref="Operation"/>.
        /// </summary>
        /// <param name="action">The action to wrap.</param>
        /// <param name="name">The name of the operation.</param>
        /// <param name="categoryName">
        ///   <para>The name of the category used by performance counters</para>
        ///   <para>By default uses the <paramref name="name"/>.</para>
        /// </param>
        /// <param name="methodName">
        ///   <para>The name of the method.</para>
        ///   <para>By default uses the <paramref name="name"/></para>.
        /// </param>
        /// <param name="instance">
        ///   <para>The instance that the operation is started on.</para>
        ///   <para>By default the value is a <see langword="null"/>.</para>
        /// </param>
        /// <param name="arguments">The operation arguments.</param>
        /// <param name="warningDuration">
        ///   <para>The maximum duration before logging a warning.</para>
        ///   <para>By default this is <see cref="TimeSpan.MaxValue"/>.</para>
        /// </param>
        /// <param name="criticalDuration">
        ///   <para>The maximum duration before logging an error.</para>
        ///   <para>By default this is <see cref="TimeSpan.MaxValue"/>.</para>
        /// </param>
        /// <param name="logUnhandledExceptions">
        ///   If set to <see langword="true"/> log unhandled exceptions that are not <see cref="LoggingException"/>s.
        /// </param>
        public static void Wrap(
            [NotNull] Action action,
            [NotNull] string name,
            [CanBeNull] string categoryName = null,
            [CanBeNull] string methodName = null,
            [CanBeNull] object instance = null,
            [CanBeNull] IEnumerable<KeyValuePair<string, object>> arguments = null,
            TimeSpan warningDuration = default(TimeSpan), TimeSpan criticalDuration = default(TimeSpan),
            bool logUnhandledExceptions = true)
        {
            Wrap(o =>
                {
                    action();
                    return 0;
                },
                name,
                categoryName,
                methodName,
                instance,
                arguments,
                warningDuration,
                criticalDuration,
                logUnhandledExceptions);
        }

        /// <summary>
        ///   Wraps the specified <see cref="Action"/> in an <see cref="Operation"/>.
        /// </summary>
        /// <param name="action">The action to wrap.</param>
        /// <param name="name">The name of the operation.</param>
        /// <param name="categoryName">
        ///   <para>The name of the category used by performance counters</para>
        ///   <para>By default uses the <paramref name="name"/>.</para>
        /// </param>
        /// <param name="methodName">
        ///   <para>The name of the method.</para>
        ///   <para>By default uses the <paramref name="name"/></para>.
        /// </param>
        /// <param name="instance">
        ///   <para>The instance that the operation is started on.</para>
        ///   <para>By default the value is a <see langword="null"/>.</para>
        /// </param>
        /// <param name="arguments">The operation arguments.</param>
        /// <param name="warningDuration">
        ///   <para>The maximum duration before logging a warning.</para>
        ///   <para>By default this is <see cref="TimeSpan.MaxValue"/>.</para>
        /// </param>
        /// <param name="criticalDuration">
        ///   <para>The maximum duration before logging an error.</para>
        ///   <para>By default this is <see cref="TimeSpan.MaxValue"/>.</para>
        /// </param>
        /// <param name="logUnhandledExceptions">
        ///   If set to <see langword="true"/> log unhandled exceptions that are not <see cref="LoggingException"/>s.
        /// </param>
        public static void Wrap(
            [NotNull] Action<Operation> action,
            [NotNull] string name,
            [CanBeNull] string categoryName = null,
            [CanBeNull] string methodName = null,
            [CanBeNull] object instance = null,
            [CanBeNull] IEnumerable<KeyValuePair<string, object>> arguments = null,
            TimeSpan warningDuration = default(TimeSpan), TimeSpan criticalDuration = default(TimeSpan),
            bool logUnhandledExceptions = true)
        {
            Wrap(o =>
                {
                    action(o);
                    return 0;
                },
                name,
                categoryName,
                methodName,
                instance,
                arguments,
                warningDuration,
                criticalDuration,
                logUnhandledExceptions);
        }

        /// <summary>
        ///   Wraps the specified function in an <see cref="Operation"/>.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="function">The function to wrap.</param>
        /// <param name="name">The name of the operation.</param>
        /// <param name="categoryName">
        ///   <para>The name of the category used by performance counters</para>
        ///   <para>By default uses the <paramref name="name"/>.</para>
        /// </param>
        /// <param name="methodName">
        ///   <para>The name of the method.</para>
        ///   <para>By default uses the <paramref name="name"/></para>.
        /// </param>
        /// <param name="instance">
        ///   <para>The instance that the operation is started on.</para>
        ///   <para>By default the value is a <see langword="null"/>.</para>
        /// </param>
        /// <param name="arguments">The operation arguments.</param>
        /// <param name="warningDuration">
        ///   <para>The maximum duration before logging a warning.</para>
        ///   <para>By default this is <see cref="TimeSpan.MaxValue"/>.</para>
        /// </param>
        /// <param name="criticalDuration">
        ///   <para>The maximum duration before logging an error.</para>
        ///   <para>By default this is <see cref="TimeSpan.MaxValue"/>.</para>
        /// </param>
        /// <param name="logUnhandledExceptions">
        ///   If set to <see langword="true"/> log unhandled exceptions that are not <see cref="LoggingException"/>s.
        /// </param>
        /// <returns>The result of the action.</returns>
        public static T Wrap<T>(
            [NotNull] Func<T> function,
            [NotNull] string name,
            [CanBeNull] string categoryName = null,
            [CanBeNull] string methodName = null,
            [CanBeNull] object instance = null,
            [CanBeNull] IEnumerable<KeyValuePair<string, object>> arguments = null,
            TimeSpan warningDuration = default(TimeSpan), TimeSpan criticalDuration = default(TimeSpan),
            bool logUnhandledExceptions = true)
        {
            return Wrap(o => function(),
                name,
                categoryName,
                methodName,
                instance,
                arguments,
                warningDuration,
                criticalDuration,
                logUnhandledExceptions);
        }

        /// <summary>
        ///   Wraps the specified function in an <see cref="Operation"/>.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="function">The function to wrap.</param>
        /// <param name="name">The name of the operation.</param>
        /// <param name="categoryName">
        ///   <para>The name of the category used by performance counters</para>
        ///   <para>By default uses the <paramref name="name"/>.</para>
        /// </param>
        /// <param name="methodName">
        ///   <para>The name of the method.</para>
        ///   <para>By default uses the <paramref name="name"/></para>.
        /// </param>
        /// <param name="instance">
        ///   <para>The instance that the operation is started on.</para>
        ///   <para>By default the value is a <see langword="null"/>.</para>
        /// </param>
        /// <param name="arguments">The operation arguments.</param>
        /// <param name="warningDuration">
        ///   <para>The maximum duration before logging a warning.</para>
        ///   <para>By default this is <see cref="TimeSpan.MaxValue"/>.</para>
        /// </param>
        /// <param name="criticalDuration">
        ///   <para>The maximum duration before logging an error.</para>
        ///   <para>By default this is <see cref="TimeSpan.MaxValue"/>.</para>
        /// </param>
        /// <param name="logUnhandledExceptions">
        ///   If set to <see langword="true"/> log unhandled exceptions that are not <see cref="LoggingException"/>s.
        /// </param>
        /// <returns>The result of the function.</returns>
        public static T Wrap<T>(
            [NotNull] Func<Operation, T> function,
            [NotNull] string name,
            [CanBeNull] string categoryName = null,
            [CanBeNull] string methodName = null,
            [CanBeNull] object instance = null,
            [CanBeNull] IEnumerable<KeyValuePair<string, object>> arguments = null,
            TimeSpan warningDuration = default(TimeSpan), TimeSpan criticalDuration = default(TimeSpan),
            bool logUnhandledExceptions = true)
        {
            // Create operation
            Operation operation = new Operation(name, categoryName, methodName, instance, arguments, warningDuration,
                                                criticalDuration);
            if (logUnhandledExceptions)
            {
                // Catch exceptions
                try
                {
                    // Execute the function and return the result.
                    return function(operation);
                }
                catch (LoggingException)
                {
                    // Rethrow logging exceptions as already logged.
                    throw;
                }
                catch (ThreadAbortException t)
                {
                    // Just log, don't wrap.
                    Log.Add(t, LogLevel.Warning);
                }
                catch (Exception e)
                {
                    // Wrap the exception in a logging exception.
                    throw new LoggingException(e, Resources.Operation_Wrap_UnhandledExceptionOccurred,
                        LogLevel.Error, e.Message);
                }
                finally
                {
                    operation.Dispose();
                }
            }

            // Don't bother catching exceptions
            using (operation)
            {
                return function(operation);
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Operation"/> class.
        /// </summary>
        /// <param name="name">The name of the operation.</param>
        /// <param name="categoryName">
        ///   <para>The name of the category used by performance counters</para>
        ///   <para>By default uses the <paramref name="name"/>.</para>
        /// </param>
        /// <param name="methodName">
        ///   <para>The name of the method.</para>
        ///   <para>By default uses the <paramref name="name"/></para>.
        /// </param>
        /// <param name="instance">
        ///   <para>The instance that the operation is started on.</para>
        ///   <para>By default the value is a <see langword="null"/>.</para>
        /// </param>
        /// <param name="arguments">The operation arguments (defaults to empty).</param>
        /// <param name="warningDuration">
        ///   <para>The maximum duration before logging a warning.</para>
        ///   <para>By default this is <see cref="TimeSpan.MaxValue"/>.</para>
        /// </param>
        /// <param name="criticalDuration">
        ///   <para>The maximum duration before logging an error.</para>
        ///   <para>By default this is <see cref="TimeSpan.MaxValue"/>.</para>
        /// </param>
        private Operation(
            [NotNull] string name,
            [CanBeNull] string categoryName = null,
            [CanBeNull] string methodName = null,
            [CanBeNull] object instance = null,
            [CanBeNull] IEnumerable<KeyValuePair<string, object>> arguments = null,
            TimeSpan warningDuration = default(TimeSpan), TimeSpan criticalDuration = default(TimeSpan))
            : this(CombGuid.NewCombGuid(DateTime.Now))
        {
            // Grab the parent operation off the context stack (if any).
            Parent = _contextStack.Current;
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            ThreadName = String.IsNullOrWhiteSpace(Thread.CurrentThread.Name)
                ? ThreadId.ToString()
                : Thread.CurrentThread.Name;
            Name = name;
            CategoryName = categoryName ?? name;
            Method = methodName ?? name;
            InstanceHash = instance == null
                ? null
                : (int?) instance.GetHashCode();
            _arguments = arguments == null
                ? new List<KeyValuePair<string, string>>(0)
                : arguments
                    .Select(kvp => new KeyValuePair<string, string>(kvp.Key ?? string.Empty,
                         kvp.Value == null
                             ? null
                             : kvp.Value.ToString()))
                    .ToList();

            // Add this operation onto the context stack.
            _contextStackDisposer = _contextStack.Region(this);

            // Set 
            if (warningDuration == default(TimeSpan))
                warningDuration = TimeSpan.MaxValue;

            if (criticalDuration == default(TimeSpan))
                criticalDuration = TimeSpan.MaxValue;

            Log.Add(Guid, "Started {0}", LogLevel.Information, Name);

            _timer = PerformanceTimer.Get(String.Join(": ", CategoryName, Name)).Region(warningDuration, criticalDuration);
        }

#if false
        private Operation(
            OperationRow operationRow,
            IEnumerable<OperationArgument.OperationArgumentRow> arguments,
            Operation parent = null)
            : this(operationRow.ID)
        {
            Created = operationRow.Created;
            ThreadId = operationRow.ThreadID;
            ThreadName = operationRow.ThreadName;
            Name = operationRow.Name;
            CategoryName = operationRow.CategoryName;
            InstanceHash = operationRow.InstanceHash;
            Method = operationRow.Method;
            Parent = parent;

            // Add the arguments to the operation.
            _arguments = arguments.Select(a => OperationArgument.GetOperationArgument(a, this)).ToList();
        }
        
#endif

        /// <summary>
        ///   Creates a blank operation with the specified Guid.
        /// </summary>
        /// <param name="guid">The Guid.</param>
        private Operation(CombGuid guid)
        {
            Guid = guid;
            if (!_operations.TryAdd(Guid, this))
                throw new InvalidOperationException(Resources.Operation_DuplicateGuidProvided);
        }

        /// <summary>
        ///   Gets the current operation; or <see langword="null"/> if there is none.
        /// </summary>
        public static Operation Current
        {
            get { return _contextStack.Current; }
        }

        /// <summary>
        ///   The operation's arguments.
        /// </summary>
        [NotNull]
        public IEnumerable<KeyValuePair<string, string>> Arguments
        {
            get { return _arguments; }
        }

        /// <summary>
        ///   Gets the parent <see cref="Operation"/>.
        /// </summary>
        /// <value>The parent operation.</value>
        /// <exception cref="LoggingException">
        ///   Cannot recover the operation's parent with the stored parent Guid.
        /// </exception>
        public Operation Parent
        {
            get
            {
                if ((_parent == null) && (_parentGuid != CombGuid.Empty))
                {
                    if (!_operations.TryGetValue(_parentGuid, out _parent))
                        throw new LoggingException(Resources.Operation_Parent_CannotRecoverParent, LogLevel.Error, _parentGuid);
                }
                return _parent;
            }
            private set
            {
                _parent = value;
                _parentGuid = _parent == null ? CombGuid.Empty : _parent.Guid;
            }
        }

        /// <summary>
        ///   Gets the XML version of the operation.
        /// </summary>
        /// <value>The XML version of the operation.</value>
        /// <remarks>The XML version is cached to avoid regeneration.</remarks>
        public XElement Xml
        {
            get
            {
                if (_xml == null)
                {
                    _xml = new XElement(
                        NodeOperation,
                        new XAttribute(AttributeGuid, Guid.Guid),
                        new XAttribute(AttributeName, Name == null
                            ? string.Empty
                            : Name.XmlEscape() ?? string.Empty),
                        new XAttribute(AttributeType, GetType()),
                        new XElement(NodeThread, new XAttribute(AttributeId, ThreadId), ThreadName.XmlEscape()),
                        new XElement(NodeMethod, Method.XmlEscape()));

                    if (_parentGuid != CombGuid.Empty)
                        _xml.Add(AttributeParent, _parentGuid);

                    if (_arguments.Any())
                    {
                        _xml.Add(
                            new XElement(
                                NodeArguments,
                                from kvp in _arguments
                                select
                                    new XElement(
                                    NodeArgument,
                                    new XAttribute(AttributeKey, kvp.Key.XmlEscape() ?? string.Empty),
                                    kvp.Value.XmlEscape())));
                    }
                }

                // Return copy of cached XElement
                return new XElement(_xml);
            }
        }

        /// <summary>
        ///   Gets a <see cref="bool"/> value indicating whether the operation is currently in progress.
        /// </summary>
        [UsedImplicitly]
        public bool InProgress
        {
            get { return _contextStackDisposer != null; }
        }

        #region IDisposable Members
        /// <inheritdoc />
        public void Dispose()
        {
            // Dispose timer.
            var t = Interlocked.Exchange(ref _timer, null);
            if (!ReferenceEquals(t, null))
            {
                t.Dispose();
                TimeSpan elapsed = t.Elapsed;

                Log.Add(
                    Guid,
                    new LogContext("OperationDuration", elapsed.ToString()),
                    "Stopping {0} [Operation took {1:0.0000} ms]{2}",
                    t.Critical
                        ? LogLevel.Critical
                        : t.Warning
                              ? LogLevel.Warning
                              : LogLevel.Information,
                    Name,
                    elapsed.TotalMilliseconds);
            }

            // Dispose context stack
            IDisposable d = Interlocked.Exchange(ref _contextStackDisposer, null);
            if (!ReferenceEquals(d, null))
                d.Dispose();
        }
        #endregion

        #region IEqualityComparer<Operation> Members
        /// <inheritdoc />
        public bool Equals(Operation x, Operation y)
        {
            return x.Guid.Equals(y.Guid);
        }

        /// <inheritdoc />
        public int GetHashCode(Operation obj)
        {
            return obj.Guid.GetHashCode();
        }
        #endregion

        #region IEquatable<Operation> Members
        /// <summary>
        ///   Compares two base operations safely.
        /// </summary>
        /// <param name="operation">The base operation to compare with.</param>
        /// <returns>Returns <see langword="true"/> if operations are equal; otherwise returns <see langword="false"/>.</returns>
        public bool Equals(Operation operation)
        {
            return operation.Guid.Equals(Guid);
        }
        #endregion

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance. The format strings can be changed in the 
        ///   Resources.resx resource file at the key 'OperationToString', 'OperationArgumentsFormat' and
        ///   'OperationArgumentsKvp'
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The format string was a <see langword="null"/>.
        /// </exception>
        /// <exception cref="FormatException">
        ///   An index from the format string is either less than zero or greater than or equal to the number of arguments.
        /// </exception>
        public override string ToString()
        {
            // Build string representation only once
            if (_string == null)
            {
                // Generate context string
                string arguments = !_arguments.Any()
                    ? "None"
                    : String.Format(
                        "{0}\t\t\t{1}",
                        Environment.NewLine,
                        String.Join(
                            String.Format(",{0}\t\t\t", Environment.NewLine),
                            _arguments.Select(
                                kvp =>
                                string.Format("{0} = {1}", kvp.Key ?? string.Empty,
                                    kvp.Value == null
                                        ? "null"
                                        : string.Format("'{0}'", kvp.Value)))));

                _string = String.Format(
                    Resources.Operation_ToString,
                    Environment.NewLine,
                    Name,
                    Method,
                    InstanceHash == null
                        ? "No Instance"
                        : InstanceHash.ToString(),
                    ThreadId,
                    ThreadName,
                    Guid,
                    _parentGuid == CombGuid.Empty
                        ? "None"
                        : _parentGuid.ToString(),
                    arguments);
            }

            return _string;
        }

#if false
    /// <summary>
    ///   Gets the operation.
    /// </summary>
    /// <param name = "operationRow">The operation row.</param>
    /// <param name = "operations">The operations.</param>
    /// <param name = "arguments">The arguments.</param>
    /// <returns></returns>
    /// <exception cref="LoggingException">Could not find parent operation '{0}'</exception>
        internal static Operation GetOperation(
            [NotNull] OperationRow operationRow,
            [NotNull] Dictionary<Guid, OperationRow> operations,
            [NotNull] ILookup<Guid, OperationArgument.OperationArgumentRow> arguments)
        {
            Operation operation;

            // If we have the operation in the operation catalogue (weak reference dictionary), then just return that.
            if (Operations.TryGetValue(operationRow.ID, out operation))
                return operation;

            IEnumerable<OperationArgument.OperationArgumentRow> argumentRows =
                arguments.Contains(operationRow.ID)
                    ? arguments[operationRow.ID]
                    : new List<OperationArgument.OperationArgumentRow>(0);

            // Check if we have a parent or not.
            if (operationRow.ParentGuid ==
                Guid.Empty)
                return new Operation(operationRow, argumentRows);

            OperationRow parentOperation;

            // Retrieve the parent operation row, if it doesn't exist, then throw an exception.
            if (!operations.TryGetValue(operationRow.ParentGuid, out parentOperation))
                throw new LoggingException(
                    "Could not find parent operation '{0}'", LogLevel.Error, operationRow.ParentGuid);

            // We will have to recreate the operation
            return new Operation(operationRow, argumentRows, GetOperation(parentOperation, operations, arguments));
        }

        #region Nested type: OperationRow
        internal class OperationRow
        {
            public readonly string CategoryName;
            public readonly DateTime Created;
            public readonly Guid ID;
            public readonly int InstanceHash;
            public readonly Guid LogGroup;
            public readonly string Method;
            public readonly string Name;
            public readonly Guid ParentGuid;
            public readonly int ThreadID;
            public readonly string ThreadName;

            public OperationRow(SqlDataReader dataReader)
            {
                ID = dataReader.GetValue<Guid>("GUID");
                Created = dataReader.GetValue<DateTime>("Created");
                Method = dataReader.GetValue<string>("Method");
                ThreadID = dataReader.GetValue<int>("ThreadID");
                ThreadName = dataReader.GetValue<string>("ThreadName");
                InstanceHash = dataReader.GetValue<int>("InstanceHash");
                Name = dataReader.GetValue<string>("Name");
                CategoryName = dataReader.GetValue<string>("CategoryName");
                ParentGuid = dataReader.GetValue<Guid>("ParentGuid");
                LogGroup = dataReader.GetValue<Guid>("LogGroup");
            }

            public static Dictionary<Guid, OperationRow> GetOperationRows(SqlDataReader dataReader)
            {
                Dictionary<Guid, OperationRow> operations = new Dictionary<Guid, OperationRow>();

                while (dataReader.Read())
                {
                    OperationRow operationRow = new OperationRow(dataReader);
                    operations.Add(operationRow.ID, operationRow);
                }

                return operations;
            }
        }
        #endregion
#endif
    }
}