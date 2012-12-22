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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class LogContext
    {
        /// <summary>
        /// The Key reservations.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, Guid> _keyReservations = new ConcurrentDictionary<string, Guid>();

        /// <summary>
        /// The prefix reservations.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, Guid> _prefixReservations = new ConcurrentDictionary<string, Guid>();

        /// <summary>
        /// The context dictionary.
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<string, string> _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogContext" /> class.
        /// Adds a parameter collection to an optional existing context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reservation">The reservation.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="parameters">The parameters.</param>
        public LogContext([CanBeNull] LogContext context, Guid reservation, string prefix, [NotNull] params object[] parameters)
        {
            _context = context != null
                           ? new ConcurrentDictionary<string, string>(context._context)
                           : new ConcurrentDictionary<string, string>();

            if (parameters.Length < 1) return;

            // Update dictionary.
            int i = 1;
            foreach (object p in parameters)
            {
                string key = prefix + i++;
                string value = p == null ? null : p.ToString();
                Set(reservation, key, value);
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
        public LogContext([NotNull] params KeyValuePair<string, object>[] keyValuePairs)
        {
            _context =
                new ConcurrentDictionary<string, string>(
                    keyValuePairs.Select(
                        kvp =>
                        new KeyValuePair<string, string>(Validate(Guid.Empty, kvp.Key), kvp.Value == null ? null : kvp.Value.ToString())));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogContext" /> class.
        /// </summary>
        /// <param name="reservation">The reservation.</param>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <remarks>This can accept any object that implements the interface, which includes objects that implement
        /// <see cref="IDictionary{TKey, TValue}">IDictionary&lt;string, string&gt;</see>.</remarks>
        [UsedImplicitly]
        public LogContext(Guid reservation, [NotNull] params KeyValuePair<string, object>[] keyValuePairs)
        {
            _context =
                new ConcurrentDictionary<string, string>(
                    keyValuePairs.Select(
                        kvp =>
                        new KeyValuePair<string, string>(Validate(reservation, kvp.Key), kvp.Value == null ? null : kvp.Value.ToString())));
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
        {
            _context = new ConcurrentDictionary<string, string>(
                keyValuePairs.Select(kvp => new KeyValuePair<string, string>(Validate(Guid.Empty, kvp.Key), kvp.Value)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogContext" /> class.
        /// </summary>
        /// <param name="reservation">The reservation.</param>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <remarks>This can accept any object that implements the interface, which includes objects that implement
        /// <see cref="IDictionary{TKey, TValue}">IDictionary&lt;string, string&gt;</see>.</remarks>
        [UsedImplicitly]
        public LogContext(Guid reservation, [NotNull] params KeyValuePair<string, string>[] keyValuePairs)
        {
            _context = new ConcurrentDictionary<string, string>(
                keyValuePairs.Select(kvp => new KeyValuePair<string, string>(Validate(reservation, kvp.Key), kvp.Value)));
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
        {
            _context = new ConcurrentDictionary<string, string>(
                keyValuePairs.Select(kvp => new KeyValuePair<string, string>(Validate(Guid.Empty, kvp.Key), kvp.Value)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogContext" /> class.
        /// </summary>
        /// <param name="reservation">The reservation.</param>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <remarks>This can accept any object that implements the interface, which includes objects that implement
        /// <see cref="IDictionary{TKey, TValue}">IDictionary&lt;string, string&gt;</see>.</remarks>
        [UsedImplicitly]
        public LogContext(Guid reservation, [NotNull] IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            _context = new ConcurrentDictionary<string, string>(
                keyValuePairs.Select(kvp => new KeyValuePair<string, string>(Validate(reservation, kvp.Key), kvp.Value)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogContext" /> class.
        /// </summary>
        /// <param name="key">The first key.</param>
        /// <param name="value">The first value.</param>
        /// <param name="keyValuePairs">Subsequent key value pairs.</param>
        /// <remarks>This can accept any object that implements the interface, which includes objects that implement
        /// <see cref="IDictionary{TKey, TValue}">IDictionary&lt;string, string&gt;</see>.</remarks>
        [UsedImplicitly]
        public LogContext([NotNull] string key, [CanBeNull] string value, [NotNull] params string[] keyValuePairs)
        {
            // Add initial value
            _context.AddOrUpdate(Validate(Guid.Empty, key), k => value, (k, o) => value);

            int l = keyValuePairs.Length;
            if (l < 1) return;

            int i = 0;
            while (i < keyValuePairs.Length)
            {
                string k = keyValuePairs[i];
                string v = i + 1 < l
                    ? keyValuePairs[i + 1]
                    : null;
                if (k != null)
                    _context.AddOrUpdate(Validate(Guid.Empty, k), y => v, (y, o) => v);
                i += 2;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogContext" /> class.
        /// </summary>
        /// <param name="reservation">The reservation.</param>
        /// <param name="key">The first key.</param>
        /// <param name="value">The first value.</param>
        /// <param name="keyValuePairs">Subsequent key value pairs.</param>
        /// <remarks>This can accept any object that implements the interface, which includes objects that implement
        /// <see cref="IDictionary{TKey, TValue}">IDictionary&lt;string, string&gt;</see>.</remarks>
        [UsedImplicitly]
        public LogContext(Guid reservation, [NotNull] string key, [CanBeNull] string value, [NotNull] params string[] keyValuePairs)
        {
            // Add initial value
            _context.AddOrUpdate(Validate(reservation, key), k => value, (k, o) => value);

            int l = keyValuePairs.Length;
            if (l < 1) return;

            int i = 0;
            while (i < keyValuePairs.Length)
            {
                string k = keyValuePairs[i];
                string v = i + 1 < l
                    ? keyValuePairs[i + 1]
                    : null;
                if (k != null)
                    _context.AddOrUpdate(Validate(reservation, k), y => v, (y, o) => v);
                i += 2;
            }
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
        public LogContext([NotNull] string key, [CanBeNull] object value, [NotNull] params object[] keyValuePairs)
        {
            string v = value == null ? null : value.ToString();
            // Add initial value
            _context.AddOrUpdate(Validate(Guid.Empty, key), k => v, (k, o) => v);

            int l = keyValuePairs.Length;
            if (l < 1) return;

            int i = 0;
            while (i < keyValuePairs.Length)
            {
                string k = keyValuePairs[i] as string;
                value = i + 1 < l
                    ? keyValuePairs[i + 1]
                    : null;
                v = value == null ? null : value.ToString();
                if (k != null)
                    _context.AddOrUpdate(Validate(Guid.Empty, k), y => v, (y, o) => v);
                i += 2;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogContext" /> class.
        /// </summary>
        /// <param name="reservation">The reservation.</param>
        /// <param name="key">The first key.</param>
        /// <param name="value">The first value.</param>
        /// <param name="keyValuePairs">Subsequent key value pairs.</param>
        /// <remarks>This can accept any object that implements the interface, which includes objects that implement
        /// <see cref="IDictionary{TKey, TValue}">IDictionary&lt;string, string&gt;</see>.</remarks>
        [UsedImplicitly]
        public LogContext(Guid reservation, [NotNull] string key, [CanBeNull] object value, [NotNull] params object[] keyValuePairs)
        {
            string v = value == null ? null : value.ToString();
            // Add initial value
            _context.AddOrUpdate(Validate(reservation, key), k => v, (k, o) => v);

            int l = keyValuePairs.Length;
            if (l < 1) return;

            int i = 0;
            while (i < keyValuePairs.Length)
            {
                string k = keyValuePairs[i] as string;
                value = i + 1 < l
                    ? keyValuePairs[i + 1]
                    : null;
                v = value == null ? null : value.ToString();
                if (k != null)
                    _context.AddOrUpdate(Validate(reservation, k), y => v, (y, o) => v);
                i += 2;
            }
        }

        /// <summary>
        /// Reserves a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="reservation">The reservation (must not be <see cref="Guid.Empty" />.</param>
        /// <returns>The reserved key.</returns>
        /// <exception cref="WebApplications.Utilities.Logging.LoggingException">The key reservation fails.</exception>
        /// <remarks><para>The context key can only be modified with the specified reservation GUID after being reserved.</para>
        ///   <para>The recommended practice is to create a random static GUID in the class and use this for reservations, preventing
        /// anyone else modifying a reserved key for a context.</para>
        ///   <para>Trying to reserve a key when it has already been reserved with a different GUID will throw an exception.</para></remarks>
        public static string ReserveKey(string key, Guid reservation)
        {
            if (key == null)
                throw new LoggingException(Resources.LogContext_Null_Key);
            if (reservation == Guid.Empty)
                throw new LoggingException(Resources.LogContext_Empty_Reservation);
            
            // First check prefixes
            foreach (KeyValuePair<string, Guid> kvp in _prefixReservations)
            {
                if (!key.StartsWith(kvp.Key) ||
                    kvp.Value.Equals(reservation)) continue;

                throw new LoggingException(Resources.LogContext_Key_Reservation_Failed_Prefix_Match,
                                           key, kvp.Key);
            }

            Guid existing = _keyReservations.GetOrAdd(key, reservation);
            if (existing != reservation)
                throw new LoggingException(Resources.LogContext_Key_Already_Reserved, key);

            return key;
        }

        /// <summary>
        /// Reserves a prefix.
        /// </summary>
        /// <param name="prefix">The prefix (minimum of 3 characters).</param>
        /// <param name="reservation">The reservation (must not be <see cref="Guid.Empty"/>.</param>
        /// <returns>The reserved key.</returns>
        /// <remarks>
        /// <para>Any context key beginning with the prefix can only be modified with the specified reservation GUID after being reserved.</para>
        /// <para>The recommended practice is to create a random static GUID in the class and use this for reservations, preventing
        /// anyone else modifying a reserved key prefix for a context.</para>
        /// <para>Trying to reserve a key prefix when it has already been reserved with a different GUID, or when a reservation
        /// for a key that matches the prefix has already been reserved will throw an exception.</para>
        /// </remarks>
        public static string ReservePrefix(string prefix, Guid reservation)
        {
            if (prefix == null)
                throw new LoggingException(Resources.LogContext_Null_Prefix);
            if (prefix.Length < 3)
                throw new LoggingException(Resources.LogContext_Prefix_Too_Short, prefix, 3);
            if (reservation == Guid.Empty)
                throw new LoggingException(Resources.LogContext_Empty_Reservation);

            // First check prefixes
            foreach (KeyValuePair<string, Guid> kvp in _prefixReservations)
            {
                if (!prefix.StartsWith(kvp.Key) ||
                    kvp.Value.Equals(reservation)) continue;

                throw new LoggingException(Resources.LogContext_Prefix_Reservation_Failed_Prefix_Match, prefix, kvp.Key);
            }

            // Next check existing key reservations
            foreach (KeyValuePair<string, Guid> kvp in _keyReservations)
            {
                if (!kvp.Key.StartsWith(prefix) ||
                    kvp.Value.Equals(reservation)) continue;

                throw new LoggingException("",
                                           prefix, kvp.Key);
            }

            // Note that race conditions are possible here (i.e. checks above could succeed for more than one conflicting
            // pair) but the side effect is negligent.
            Guid existing = _prefixReservations.GetOrAdd(prefix, reservation);
            if (existing != reservation)
                throw new LoggingException(Resources.LogContext_Prefix_Already_Reserved, prefix);
            return prefix;
        }

        /// <summary>
        /// Determines whether the specified key is a reserved key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><see langword="true" /> if the specified key is a reserved key; otherwise, <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UsedImplicitly]
        public static bool IsReservedKey(string key)
        {
            // Check for reservation.
            return Reservation(key) != Guid.Empty;
        }

        /// <summary>
        /// Get's the reservation (if any) for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Guid reservation; otherwise <see cref="Guid.Empty"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Guid Reservation(string key)
        {
            if (key == null) return Guid.Empty;

            foreach (var kvp in _prefixReservations.Where(kvp => key.StartsWith(kvp.Key)))
                return kvp.Value;
            Guid reservation;
            return _keyReservations.TryGetValue(key, out reservation) ? reservation : Guid.Empty;
        }

        /// <summary>
        /// Validates the specified key, throwing an exception if it is reserved.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        private static string Validate(Guid reservation, string key)
        {
            // Check for reservation
            Guid r = Reservation(key);
            if ((r != Guid.Empty) && (r != reservation))
                throw new LoggingException(Resources.LogContext_Reserved_Key, key);
            if (key == null)
                throw new LoggingException(Resources.LogContext_Null_Key);
            return key;
        }

        /// <summary>
        /// Sets the specified key to the specified value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(string key, string value)
        {
            Set(Guid.Empty, key, value);
        }

        /// <summary>
        /// Sets the specified key to the specified value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(string key, object value)
        {
            string v = value == null ? null : value.ToString();
            Set(Guid.Empty, key, v);
        }

        /// <summary>
        /// Sets the specified key to the specified value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(Guid reservation, string key, object value)
        {
            string v = value == null ? null : value.ToString();
            Set(reservation, key, v);
        }

        /// <summary>
        /// Sets the specified key to the specified value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(Guid reservation, string key, string value)
        {
            _context.AddOrUpdate(Validate(reservation, key), k => value, (k, o) => value);
        }

        /// <summary>
        /// Gets the value of the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string Get(string key)
        {
            string value;
            return _context.TryGetValue(key, out value) ? value : null;
        }

        /// <summary>
        /// Gets all keys and their values that match the prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>IEnumerable{KeyValuePair{System.StringSystem.String}}.</returns>
        public IEnumerable<KeyValuePair<string, string>> GetPrefixed(string prefix)
        {
            return _context.Where(kvp => kvp.Key.StartsWith(prefix));
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> representation of this instance. The format strings can be changed in the
        /// Resources.resx resource file at the key 'LogContextToString'</returns>
        /// <exception cref="ArgumentNullException">The format string was a <see langword="null" />.</exception>
        /// <exception cref="FormatException">An index from the format string is either less than zero or greater than or equal to the number of arguments.</exception>
        public override string ToString()
        {
            StringBuilder stringBuilder =
                new StringBuilder(String.Format(Resources.LogContext_ToString, _context.Count, _context.Count == 1 ? "y" : "ies"));
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
            return stringBuilder.ToString();
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="Dictionary&lt;T, T&gt;">Dictionary&lt;string, string&gt;</see> 
        ///   to <see cref="WebApplications.Utilities.Logging.LogContext"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator LogContext([NotNull] Dictionary<string, string> dictionary)
        {
            return new LogContext((IEnumerable<KeyValuePair<string, string>>)dictionary);
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