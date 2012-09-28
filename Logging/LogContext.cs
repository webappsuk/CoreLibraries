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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   Allows for additional contextual information to be stored against a <see cref="Log">log item</see>.
    /// </summary>
    /// <remarks>
    ///   As well as constructing a <see cref="LogContext"/> directly, it is equally valid to use one of the
    ///   implicit casts, or the static <see cref="Empty">new LogContext()</see>.
    /// </remarks>
    [Serializable]
    public class LogContext : IEnumerable<KeyValuePair<string, string>>
    {
        [NonSerialized] private const string NodeContexts = "Contexts";
        [NonSerialized] private const string NodeContext = "Context";
        [NonSerialized] private const string AttributeKey = "key";
        [NonSerialized] private const string AttributeValue = "value";


        /// <summary>
        ///   Holds <see cref="LogContext"/> against the thread call stack.
        /// </summary>
        private static readonly ContextStack<LogContext> _contextStack = new ContextStack<LogContext>();

        /// <summary>
        ///   The context information.
        /// </summary>
        [NotNull] private readonly Dictionary<string, string> _context;

        /// <summary>
        ///   The cached <see cref="string"/> representation.
        /// </summary>
        [NonSerialized] private string _string;

        /// <summary>
        ///   The cached XML representation.
        /// </summary>
        [NonSerialized] private XElement _xml;

        /// <summary>
        ///   Initializes a new instance of the <see cref="LogContext"/> class.
        /// </summary>
        [UsedImplicitly]
        private LogContext()
        {
            // Get the context
            LogContext current = _contextStack.Current;

            // Either create a new blank dictionary or a copy.
            _context = current == null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(current._context);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LogContext"/> class.
        ///   Adds a parameter collection to an optional existing context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="parameters">The parameters.</param>
        public LogContext([CanBeNull] LogContext context, [NotNull] params object[] parameters) : this()
        {
            if (context != null)
                // Update dictionary.
                foreach (KeyValuePair<string, string> kvp in context._context)
                {
                    if (kvp.Key == null) continue;
                    if (_context.ContainsKey(kvp.Key))
                        _context[kvp.Key] = kvp.Value;
                    else
                        _context.Add(kvp.Key, kvp.Value);
                }

            if (parameters.Length > 0)
            {
                // Update dictionary.
                int i = 1;
                foreach (object p in parameters)
                {
                    string key = "Parameter" + i++;
                    string value = p == null ? null : p.ToString();
                    if (_context.ContainsKey(key))
                        _context[key] = value;
                    else
                        _context.Add(key, value);
                }
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LogContext"/> class.
        /// </summary>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <remarks>
        ///   This can accept any object that implements the interface, which includes objects that implement
        ///   <see cref="IDictionary{TKey, TValue}">IDictionary&lt;string, string&gt;</see>.
        /// </remarks>
        [UsedImplicitly]
        public LogContext([NotNull] params KeyValuePair<string, string>[] keyValuePairs)
            : this((IEnumerable<KeyValuePair<string, string>>) keyValuePairs)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LogContext"/> class.
        /// </summary>
        /// <param name="key">The first key.</param>
        /// <param name="value">The first value.</param>
        /// <param name="keyValuePairs">Subsequent key value pairs.</param>
        /// <remarks>
        ///   This can accept any object that implements the interface, which includes objects that implement
        ///   <see cref="IDictionary{TKey, TValue}">IDictionary&lt;string, string&gt;</see>.
        /// </remarks>
        [UsedImplicitly]
        public LogContext([NotNull] string key, [CanBeNull] string value, [NotNull] params string[] keyValuePairs)
            : this(Convert(key, value, keyValuePairs))
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LogContext"/> class.
        /// </summary>
        /// <remarks>
        ///   This can accept any object that implements the interface, which includes objects that implement
        ///   <see cref="IDictionary{TKey, TValue}">IDictionary&lt;string, string&gt;</see>.
        /// </remarks>
        /// <param name="keyValuePairs">The key value pairs.</param>
        [UsedImplicitly]
        public LogContext([NotNull] IEnumerable<KeyValuePair<string, string>> keyValuePairs)
            : this()
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (keyValuePairs == null) return;
            // ReSharper restore ConditionIsAlwaysTrueOrFalse

            // Update dictionary.
            foreach (KeyValuePair<string, string> kvp in keyValuePairs)
            {
                if (kvp.Key == null) continue;
                if (_context.ContainsKey(kvp.Key))
                    _context[kvp.Key] = kvp.Value;
                else
                    _context.Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        ///   Creates a region from the current <see cref="LogContext"/>.
        /// </summary>
        public IDisposable Region
        {
            get { return _contextStack.Region(_context); }
        }

        // ReSharper restore ParameterTypeCanBeEnumerable.Global

        /// <summary>
        ///   Gets the value with the specified key, or a <see langword="null"/> if not found.
        /// </summary>
        [CanBeNull]
        [UsedImplicitly]
        public string this[[CanBeNull] string key]
        {
            get
            {
                string value;
                return _context.TryGetValue(key, out value) ? value : null;
            }
        }

        /// <summary>
        ///   Gets the XML version of the operation.
        ///   This is cached to avoid reconstructing the XML each time.
        /// </summary>
        /// <value>The XML version of the operation.</value>
        [NotNull]
        [UsedImplicitly]
        public XElement Xml
        {
            get
            {
                if (_xml == null)
                {
                    // Create XML for context.
                    _xml = new XElement(NodeContexts,
                        _context
                            .Select(kvp =>
                                new XElement(NodeContext,
                                    new XAttribute(AttributeKey, kvp.Key ?? "xs:null"),
                                    new XAttribute(AttributeValue, kvp.Value ?? "xs:null")))
                            .ToArray());
                }

                // Return copy of cached XElement
                return new XElement(_xml);
            }
        }

        #region IEnumerable<KeyValuePair<string,string>> Members
        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1">T:System.Collections.Generic.IEnumerator`1</see>
        ///   that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _context.GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        /// <summary>
        ///   Converts the <see cref="Array">array</see> of <see cref="string"/>s into key value pairs.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <returns>The converted result.</returns>
        [NotNull]
        private static IEnumerable<KeyValuePair<string, string>> Convert([NotNull] string key, [CanBeNull] string value,
                                                                         [NotNull] string[] keyValuePairs)
        {
            // Create initial list.
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>
                                                          {new KeyValuePair<string, string>(key, value)};

            int l = keyValuePairs.Length;
            if (l < 1) return list;

            int i = 0;
            while (i < keyValuePairs.Length)
            {
                string k = keyValuePairs[i];
                string v = i + 1 < l
                    ? keyValuePairs[i + 1]
                    : null;
                if (k != null)
                    list.Add(new KeyValuePair<string, string>(k, v));
                i += 2;
            }
            return list;
        }

        /// <summary>
        ///   Creates a context region.
        /// </summary>
        /// <remarks>
        ///   Although this accepts a <see cref="LogContext"/>, you can implicitly convert a <see cref="Dictionary{TKey,TValue}">Dictionary&lt;string,string&gt;</see>
        ///   or <see cref="IEnumerable{T}">enumeration</see> of <see cref="KeyValuePair{TKey,TValue}">KeyValuePair&lt;string,string&gt;</see> into a <see cref="LogContext"/>.
        ///   In doing so it automatically prepends the existing context from the thread stack.
        /// </remarks>
        [UsedImplicitly]
        public static IDisposable CreateRegion([NotNull] string key, [CanBeNull] string value, [NotNull] string[] keyValuePairs)
        {
            return _contextStack.Region(new LogContext(key, value, keyValuePairs));
        }

        /// <summary>
        ///   Creates a context region.
        /// </summary>
        /// <remarks>
        ///   Although this accepts a <see cref="LogContext"/>, you can implicitly convert a <see cref="Dictionary{TKey,TValue}">Dictionary&lt;string,string&gt;</see>
        ///   or <see cref="IEnumerable{T}">enumeration</see> of <see cref="KeyValuePair{TKey,TValue}">KeyValuePair&lt;string,string&gt;</see>
        ///   into a <see cref="LogContext"/>. In doing so it automatically prepends the existing context from the thread stack.
        /// </remarks>
        // ReSharper disable ParameterTypeCanBeEnumerable.Global
        public static IDisposable CreateRegion(LogContext context)
        {
            return _contextStack.Region(context);
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance. The format strings can be changed in the 
        ///   Resources.resx resource file at the key 'LogContextToString'
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The format string was a <see langword="null"/>.
        /// </exception>
        /// <exception cref="FormatException">
        ///   An index from the format string is either less than zero or greater than or equal to the number of arguments.
        /// </exception>
        public override string ToString()
        {
            if (_string == null)
            {
                StringBuilder stringBuilder =
                    new StringBuilder(String.Format(Resources.LogContext_ToString, _context.Count,
                        _context.Count == 1
                            ? "y"
                            : "ies"));
                foreach (KeyValuePair<string, string> kvp in _context)
                {
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append("\t\t");
                    if (kvp.Key == null)
                        stringBuilder.Append("null");
                    else
                    {
                        stringBuilder.Append("'");
                        stringBuilder.Append(kvp.Key);
                        stringBuilder.Append("'");
                    }
                    stringBuilder.Append(" = ");
                    if (kvp.Value == null)
                        stringBuilder.Append("null");
                    else
                    {
                        stringBuilder.Append("'");
                        stringBuilder.Append(kvp.Value);
                        stringBuilder.Append("'");
                    }
                }
                _string = stringBuilder.ToString();
            }
            return _string;
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="Dictionary&lt;T, T&gt;">Dictionary&lt;string, string&gt;</see> 
        ///   to <see cref="WebApplications.Utilities.Logging.LogContext"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator LogContext([NotNull] Dictionary<string, string> dictionary)
        {
            return new LogContext((IEnumerable<KeyValuePair<string, string>>) dictionary);
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="KeyValuePair&lt;T, T&gt;">KeyValuePair&lt;string, string&gt;</see> 
        ///   to a <see cref="WebApplications.Utilities.Logging.LogContext"/>.
        /// </summary>
        /// <param name="keyValuePair">The key value pair.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator LogContext(KeyValuePair<string, string> keyValuePair)
        {
            return new LogContext(keyValuePair);
        }
    }
}