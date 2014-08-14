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
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   Holds a definition for SQL stored procedure or function.
    /// </summary>
    public class SqlProgramDefinition : IEqualityComparer<SqlProgramDefinition>, IEquatable<SqlProgramDefinition>
    {
        /// <summary>
        ///   The program name.
        /// </summary>
        public readonly string Name;

        /// <summary>
        ///   The schema name.
        /// </summary>
        [UsedImplicitly] public readonly string SchemaName;

        /// <summary>
        ///   The <see cref="SqlObjectType"/> of the program.
        /// </summary>
        public readonly SqlObjectType Type;

        /// <summary>
        ///   The program parameters, indexed by the parameter name.
        /// </summary>
        private readonly Dictionary<string, SqlProgramParameter> _parameters =
            new Dictionary<string, SqlProgramParameter>();

        /// <summary>
        ///   Hash code cache.
        /// </summary>
        private int? _hashCode;

        private IEnumerable<SqlProgramParameter> _parametersOrdered;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlProgramDefinition" /> class.
        /// </summary>
        /// <param name="type">The type of program.</param>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="name">
        ///   The <see cref="SqlProgramDefinition.Name">program name</see>.
        /// </param>
        internal SqlProgramDefinition(SqlObjectType type, string schemaName, string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(schemaName));
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Type = type;
            Name = name;
            SchemaName = schemaName;
        }

        /// <summary>
        ///   Gets the parameters in ordinal order.
        /// </summary>
        /// <value>
        ///   The parameters in ordinal order.
        /// </value>
        [NotNull]
        public IEnumerable<SqlProgramParameter> Parameters
        {
            get
            {
                return _parametersOrdered ??
// ReSharper disable PossibleNullReferenceException
                       (_parametersOrdered = _parameters.Values.OrderBy(p => p.Ordinal).ToList());
// ReSharper restore PossibleNullReferenceException
            }
        }

        /// <summary>
        ///   Gets the full name.
        /// </summary>
        /// <value>
        ///   The full name, which has a format of: <see cref="SchemaName"/>.<see cref="Name"/>.
        /// </value>
        public string FullName
        {
            get { return string.Format("{0}.{1}", SchemaName, Name); }
        }

        #region IEqualityComparer<SqlProgramDefinition> Members
        /// <summary>
        ///   Determines whether the specified <see cref="SqlProgramDefinition"/> objects are equal.
        /// </summary>
        /// <param name="x">The first <see cref="SqlProgramDefinition"/> to compare.</param>
        /// <param name="y">The second <see cref="SqlProgramDefinition"/> to compare.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified <see cref="SqlProgramDefinition"/>s are equal;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        public bool Equals(SqlProgramDefinition x, SqlProgramDefinition y)
        {
            if (x == null)
                return y == null;
            return x.Equals(y);
        }

        /// <summary>
        ///   Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        ///   A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="object"/> for which a hash code is to be returned.</param>
        /// <exception cref="ArgumentNullException">
        ///   The type of <paramref name="obj"/> is a reference type and is <see langword="null"/>.
        /// </exception>
        public int GetHashCode([NotNull] SqlProgramDefinition obj)
        {
            if (obj._hashCode == null)
            {
                obj._hashCode =
                    Parameters.Aggregate(
                        Type.GetHashCode() ^ Name.GetHashCode() ^ SchemaName.GetHashCode(),
// ReSharper disable PossibleNullReferenceException
                        (h, p) => h ^ p.GetHashCode());
// ReSharper restore PossibleNullReferenceException
            }
            return (int) obj._hashCode;
        }
        #endregion

        #region IEquatable<SqlProgramDefinition> Members
        /// <summary>
        ///   Indicates whether the current <see cref="SqlProgramDefinition"/> is another instance.
        /// </summary>
        /// <returns>
        ///   Returns <see langword="true"/> if the current <see cref="SqlProgramDefinition"/> is equal to the
        ///   <paramref name="other"/> <see cref="SqlProgramDefinition"/>; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="other"/> is <see langword="null"/>.
        /// </exception>
        public bool Equals(SqlProgramDefinition other)
        {
            if (other == null)
                return false;
            return (Type == other.Type) && (FullName == other.FullName) &&
                   (Parameters.SequenceEqual(other.Parameters));
        }
        #endregion

        /// <summary>
        ///   Adds the parameter specified to the parameters collection.
        /// </summary>
        /// <param name="parameter">The parameter to add.</param>
        internal void AddParameter([NotNull] SqlProgramParameter parameter)
        {
            Contract.Requires(parameter != null);
            _parameters.Add(parameter.Name, parameter);
        }

        /// <summary>
        ///   Tries to get the parameter with the specified name.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="parameter">The retrieved parameter (if found).</param>
        /// <returns>
        ///   Returns <see langword="true"/> if a parameter with the specified <paramref name="parameterName"/> was found;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parameterName"/> is <see langword="null"/>
        /// </exception>
        public bool TryGetParameter([NotNull] string parameterName, out SqlProgramParameter parameter)
        {
            return _parameters.TryGetValue(parameterName.ToLower(), out parameter);
        }

        /// <summary>
        ///   Determines whether the specified parameters are valid.
        /// </summary>
        /// <param name="validateOrder">
        ///   If set to <see langword="true"/> then validate the parameter order.
        /// </param>
        /// <param name="parameters">The parameters to validate.</param>
        /// <returns>The parameters that were validated (in the order specified).</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parameters"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="LoggingException">Parameter counts not equal.</exception>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SqlProgramParameter> ValidateParameters(bool validateOrder,
                                                                   [NotNull] params string[] parameters)
        {
            return ValidateParameters(parameters.Select(p => new KeyValuePair<string, Type>(p, null)), validateOrder);
        }

        /// <summary>
        ///   Determines whether the specified parameters are valid.
        /// </summary>
        /// <param name="validateOrder">
        ///   If set to <see langword="true"/> then validate the parameter order.
        /// </param>
        /// <param name="parameters">The parameters to validate.</param>
        /// <returns>The parameters that were validated (in the order specified).</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parameters"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="LoggingException">Parameter counts not equal.</exception>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SqlProgramParameter> ValidateParameters([NotNull] IEnumerable<string> parameters,
                                                                   bool validateOrder = false)
        {
            return ValidateParameters(parameters.Select(p => new KeyValuePair<string, Type>(p, null)), validateOrder);
        }

        /// <summary>
        ///   Determines whether the specified parameter types are valid on this program (in ordinal order).
        /// </summary>
        /// <param name="parameters">The parameter types.</param>
        /// <returns>The types that were validated (in the order specified).</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parameters"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="LoggingException">
        ///   The supplied parameters count did not match the parameters of the SQL program.
        /// </exception>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SqlProgramParameter> ValidateParameters([NotNull] params Type[] parameters)
        {
            return ValidateParameters(parameters.Select(t => new KeyValuePair<string, Type>(null, t)), true);
        }

        /// <summary>
        ///   Determines whether the specified parameter types are valid on this program (in ordinal order).
        /// </summary>
        /// <param name="parameters">The parameters and their type (or <see langword="null"/> to bypass type check).</param>
        /// <returns>The parameters that were validated (in the order specified).</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parameters"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="LoggingException">
        ///   The supplied <paramref name="parameters"/> count did not match the parameters of the SQL program.
        /// </exception>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SqlProgramParameter> ValidateParameters([NotNull] IEnumerable<Type> parameters)
        {
            return ValidateParameters(parameters.Select(t => new KeyValuePair<string, Type>(null, t)), true);
        }

        /// <summary>
        ///   Converts parameters to KVP enumeration.
        /// </summary>
        /// <param name="names">The parameter names.</param>
        /// <param name="types">The parameter types.</param>
        /// <exception cref="LoggingException">
        ///   The supplied <paramref name="names"/> count did not match the <paramref name="types"/> count.
        /// </exception>
        [NotNull]
        internal static IEnumerable<KeyValuePair<string, Type>> ToKVP(
            [NotNull] IEnumerable<string> names,
                                                                      [NotNull] params Type[] types)
        {
            Contract.Requires(names != null);
            Contract.Requires(types != null);
            List<Type> t = types.ToList();
            int nCount = names.Count();
            int tCount = t.Count();
            if (nCount != tCount)
                throw new LoggingException(
                    LoggingLevel.Critical,
                    Resources.SqlProgramDefinition_ToKVP_TypeAndNameCountNotEqual,
                    nCount,
                    tCount);

            // Build a parameters collection for validation.
            List<KeyValuePair<string, Type>> parameters = new List<KeyValuePair<string, Type>>();
            using (IEnumerator<string> ne = names.GetEnumerator())
            using (List<Type>.Enumerator te = t.GetEnumerator())
                do
                {
                    if (!te.MoveNext())
                    {
                        // Sanity check
                        if (ne.MoveNext())
                            throw new LoggingException(
                                LoggingLevel.Critical,
                                Resources.SqlProgramDefinition_ToKVP_TypeAndNameCountNotEqual,
                                nCount,
                                tCount);
                        break;
                    }
                    Type type = te.Current;
                    string name = (ne.MoveNext()) ? ne.Current : null;

                    parameters.Add(new KeyValuePair<string, Type>(name, type));
                } while (true);
            return parameters;
        }

        /// <summary>
        ///   Determines whether the specified parameters are valid on this program, and checks the types.
        /// </summary>
        /// <param name="names">The parameter names.</param>
        /// <param name="types">The parameter types.</param>
        /// <returns>The parameters that were validated (in the order specified).</returns>
        /// <exception cref="LoggingException">
        ///   The supplied parameters count did not match the parameters of the SQL program.
        /// </exception>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SqlProgramParameter> ValidateParameters([NotNull] IEnumerable<string> names,
                                                                   [NotNull] params Type[] types)
        {
            // Validate the parameters
            return ValidateParameters(ToKVP(names, types));
        }

        /// <summary>
        ///   <para>Determines whether the specified parameters are valid on this program.</para>
        ///   <para>Also checks that the provided types are accepted by the program parameter
        ///   <see cref="SqlProgramParameter.Type">types</see>.</para>
        /// </summary>
        /// <param name="parameters">
        ///   <para>The parameters and their types.</para>
        ///   <para>Specify <see langword="null"/> to bypass type checking.</para>
        /// </param>
        /// <param name="validateOrder">
        ///   If set to <see langword="true"/> then validate the parameter order.
        /// </param>
        /// <returns>The parameters that were validated (in the order specified).</returns>
        /// <exception cref="LoggingException">
        ///   <para>The supplied parameters count did not match the parameters of the SQL program.</para>
        ///   <para>-or-</para>
        ///   <para>A parameter name was <see langword="null"/> and <paramref name="validateOrder"/> was <see langword="false"/>.</para>
        ///   <para>-or-</para>
        ///   <para>A parameter name was provided that does not exist in the SQL program.</para>
        ///   <para>-or-</para>
        ///   <para>The parameter type does not accept the CLR type specified.</para>
        /// </exception>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SqlProgramParameter> ValidateParameters(
            [NotNull] IEnumerable<KeyValuePair<string, Type>> parameters,
            bool validateOrder = false)
        {
            int sCount;

            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if ((parameters == null) || ((sCount = parameters.Count()) < 1))
                return Enumerable.Empty<SqlProgramParameter>();
            // ReSharper restore ConditionIsAlwaysTrueOrFalse

            int dCount = Parameters.Count();
            if (dCount < sCount)
                throw new LoggingException(
                    LoggingLevel.Critical,
                    Resources.SqlProgramDefinition_ValidateParameters_ParameterCountsNotEqual,
                    FullName,
                    dCount,
                    sCount);

            // Create list of parameters for output
            List<SqlProgramParameter> sqlParameters = new List<SqlProgramParameter>(sCount);

            if (validateOrder)
            {
                using (IEnumerator<SqlProgramParameter> p1 = Parameters.GetEnumerator())
                using (IEnumerator<KeyValuePair<string, Type>> p2 = parameters.GetEnumerator())
                {
                    do
                    {
                        if (!p1.MoveNext())
                        {
                            // If we still have parameters to check, we don't match - should be caught above - but sanity check.
                            if (p2.MoveNext())
                                throw new LoggingException(
                                    LoggingLevel.Critical,
                                    Resources.SqlProgramDefinition_ValidateParameters_ParameterCountsNotEqual,
                                    FullName,
                                    dCount,
                                    sCount);

                            // We're done.
                            return sqlParameters;
                        }

                        // If no more supplied parameters, matching is done.
                        if (!p2.MoveNext())
                            return sqlParameters;

                        SqlProgramParameter parameter = p1.Current;

                        // Add parameter to return enumeration
                        sqlParameters.Add(parameter);

                        string name = p2.Current.Key;
                        // Only check name if not null.
                        if ((name != null) && (parameter.Name != (name = name.ToLower())))
                            throw new LoggingException(
                                LoggingLevel.Critical,
                                Resources.SqlProgramDefinition_ValidateParameters_ParameterDoesNotExist,
                                name,
                                FullName);

                        Type type = p2.Current.Value;
                        // Only check type if not null.
                        if (type == null)
                            continue;

                        // Check to see if parameter accepts type
                        if (!parameter.Type.AcceptsCLRType(type))
                            throw new LoggingException(
                                LoggingLevel.Critical,
                                Resources.SqlProgramDefinition_ValidateParameters_TypeDoesNotAcceptClrType,
                                parameter.Name,
                                FullName,
                                parameter.Type.FullName,
                                type);
                    } while (true);
                }
            }

            // We are not validating order
            foreach (KeyValuePair<string, Type> kvp in parameters)
            {
                // If we have a null parameter name, cannot possibly match.
                if (kvp.Key == null)
                    throw new LoggingException(
                        LoggingLevel.Critical,
                        Resources.SqlProgramDefinition_ValidateParameters_MustSpecifyParameterName,
                        FullName);

                SqlProgramParameter parameter;
                string name = kvp.Key.ToLower();
                if (!_parameters.TryGetValue(name, out parameter))
                    throw new LoggingException(
                        LoggingLevel.Critical,
                        Resources.SqlProgramDefinition_ValidateParameters_ParameterDoesNotExist,
                        name,
                        FullName);

                // Add parameter to return enumeration
                sqlParameters.Add(parameter);

                Type type = kvp.Value;
                if ((type != null) && (!parameter.Type.AcceptsCLRType(type)))
                    throw new LoggingException(
                        LoggingLevel.Critical,
                        Resources.SqlProgramDefinition_ValidateParameters_TypeDoesNotAcceptClrType,
                        parameter.Name,
                        FullName,
                        parameter.Type.FullName,
                        type);
            }
            return sqlParameters;
        }

        /// <summary>
        ///   Determines whether the specified parameters are valid on this program, and checks the types, in ordinal order.
        /// </summary>
        /// <param name="parameters">
        ///   The parameters and their type (or <see langword="null"/> to bypass type checking).
        /// </param>
        /// <returns>
        ///   The parameters that were validated (in the order specified).
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SqlProgramParameter> ValidateParameters([NotNull] params SqlDbType[] parameters)
        {
            return ValidateParameters(parameters.Select(t => new KeyValuePair<string, SqlDbType>(null, t)), true);
        }

        /// <summary>
        ///   Determines whether the specified parameters are valid on this program, and checks the types, in ordinal order.
        /// </summary>
        /// <param name="parameters">
        ///   The parameters and their type (or <see langword="null"/> to bypass type checking).
        /// </param>
        /// <returns>The parameters that were validated (in the order specified).</returns>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SqlProgramParameter> ValidateParameters([NotNull] IEnumerable<SqlDbType> parameters)
        {
            return ValidateParameters(parameters.Select(t => new KeyValuePair<string, SqlDbType>(null, t)), true);
        }

        /// <summary>
        ///   Determines whether the specified parameters are valid on this program, and checks the types.
        /// </summary>
        /// <param name="parameters">
        ///   The parameters and their type (or <see langword="null"/> to bypass type checking).
        /// </param>
        /// <param name="validateOrder">
        ///   <para>If set to <see langword="true"/> validates the parameter order.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   The parameters that were validated (in the order specified).
        ///   If <paramref name="parameters"/> is <see langword="null"/> then an empty list is returned.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   <para>The parameter counts were not equal.</para>
        ///   <para>-or-</para>
        ///   <para>One of the parameters does not exist on the specified program.</para>
        ///   <para>-or-</para>
        ///   <para>One of the parameter types specified is not compatible with the <see cref="SqlDbType"/>.</para>
        ///   <para>-or-</para>
        ///   <para>Must specify parameter names when <see paramref="validateOrder"/> is <see langword="false"/>.</para>
        /// </exception>
        [NotNull]
        [UsedImplicitly]
        public IEnumerable<SqlProgramParameter> ValidateParameters(
            [NotNull] IEnumerable<KeyValuePair<string, SqlDbType>> parameters,
            bool validateOrder = false)
        {
            int sCount;

            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if ((parameters == null) || ((sCount = parameters.Count()) < 1))
                return Enumerable.Empty<SqlProgramParameter>();
            // ReSharper restore ConditionIsAlwaysTrueOrFalse

            int dCount = Parameters.Count();
            if (dCount < sCount)
                throw new LoggingException(
                    LoggingLevel.Critical,
                    Resources.SqlProgramDefinition_ValidateParameters_ParameterCountsNotEqual,
                    FullName,
                    dCount,
                    sCount);

            // Create list of parameters for output
            List<SqlProgramParameter> sqlParameters = new List<SqlProgramParameter>(sCount);

            if (validateOrder)
            {
                using (IEnumerator<SqlProgramParameter> p1 = Parameters.GetEnumerator())
                using (IEnumerator<KeyValuePair<string, SqlDbType>> p2 = parameters.GetEnumerator())
                {
                    do
                    {
                        if (!p1.MoveNext())
                        {
                            // If we still have parameters to check, we don't match - should be caught above - but sanity check.
                            if (p2.MoveNext())
                                throw new LoggingException(
                                    LoggingLevel.Critical,
                                    Resources.SqlProgramDefinition_ValidateParameters_ParameterCountsNotEqual,
                                    FullName,
                                    dCount,
                                    sCount);

                            // We're done.
                            return sqlParameters;
                        }

                        // If no more supplied parameters, matching is done.
                        if (!p2.MoveNext())
                            return sqlParameters;

                        SqlProgramParameter parameter = p1.Current;

                        // Add parameter to return enumeration
                        sqlParameters.Add(parameter);

                        string name = p2.Current.Key;
                        // Only check name if not null.
                        if ((name != null) && (parameter.Name != (name = name.ToLower())))
                            throw new LoggingException(
                                LoggingLevel.Critical,
                                Resources.SqlProgramDefinition_ValidateParameters_ParameterDoesNotExist,
                                name,
                                FullName);

                        SqlDbType type = p2.Current.Value;
                        // Check to see if parameter accepts type
                        if (parameter.Type.SqlDbType != type)
                            throw new LoggingException(
                                LoggingLevel.Critical,
                                Resources.SqlProgramDefinition_ValidateParameters_TypeDoesNotAcceptSqlDbType,
                                parameter.Name,
                                FullName,
                                parameter.Type.FullName,
                                type);
                    } while (true);
                }
            }

            // We are not validating order
            foreach (KeyValuePair<string, SqlDbType> kvp in parameters)
            {
                // If we have a null parameter name, cannot possibly match.
                if (kvp.Key == null)
                    throw new LoggingException(
                        LoggingLevel.Critical,
                        Resources.SqlProgramDefinition_ValidateParameters_MustSpecifyParameterName,
                        FullName);

                SqlProgramParameter parameter;
                string name = kvp.Key.ToLower();
                if (!_parameters.TryGetValue(name, out parameter))
                    throw new LoggingException(
                        LoggingLevel.Critical,
                        Resources.SqlProgramDefinition_ValidateParameters_ParameterDoesNotExist,
                        name,
                        FullName);

                // Add parameter to return enumeration
                sqlParameters.Add(parameter);

                SqlDbType type = kvp.Value;
                // Check to see if parameter accepts type
                if (parameter.Type.SqlDbType != type)
                    throw new LoggingException(
                        LoggingLevel.Critical,
                        Resources.SqlProgramDefinition_ValidateParameters_TypeDoesNotAcceptSqlDbType,
                        parameter.Name,
                        FullName,
                        parameter.Type.FullName,
                        type);
            }
            return sqlParameters;
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance. The format string can be changed in the 
        ///   Resources.resx resource file at the key 'SqlProgramDefinitionToString'.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///   The format string was a <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.FormatException">
        ///   An index from the format string is either less than zero or greater than or equal to the number of arguments.
        /// </exception>
        public override string ToString()
        {
            return String.Format(Resources.SqlProgramDefinition_ToString, FullName);
        }
    }
}