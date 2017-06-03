#region © Copyright Web Applications (UK) Ltd, 2017.  All rights reserved.
// Copyright (c) 2017, Web Applications UK Ltd
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
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Schema;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Base class for a parameter to a <see cref="SqlBatchCommand"/>.
    /// </summary>
    /// <seealso cref="DbParameter" />
    [PublicAPI]
    public abstract class DbBatchParameter : DbParameter
    {
        private const int MaxParameterNameLength = 128;

        /// <summary>
        /// The base parameter.
        /// </summary>
        [NotNull]
        public DbParameter BaseParameter { get; internal set; }

        /// <summary>
        /// The parameter name de-dupe suffix.
        /// </summary>
        [NotNull]
        protected readonly string Dedupe;

        /// <summary>
        /// Gets the program parameter this batch parameter is for.
        /// </summary>
        /// <value>
        /// The program parameter.
        /// </value>
        [NotNull]
        public SqlProgramParameter ProgramParameter { get; }

        /// <summary>
        /// Gets a value indicating whether the output value of this parameter is used.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if the output value is used; otherwise, <see langword="false" />.
        /// </value>
        public bool IsOutputUsed { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbBatchParameter" /> class.
        /// </summary>
        /// <param name="programParameter">The program parameter.</param>
        /// <param name="baseParameter">The base parameter definition.</param>
        /// <param name="dedupe">The dedupe string.</param>
        protected DbBatchParameter(
            [NotNull] SqlProgramParameter programParameter,
            [NotNull] DbParameter baseParameter,
            [NotNull] string dedupe)
        {
            ProgramParameter = programParameter ?? throw new ArgumentNullException(nameof(programParameter));
            BaseParameter = baseParameter ?? throw new ArgumentNullException(nameof(baseParameter));
            Dedupe = dedupe ?? throw new ArgumentNullException(nameof(dedupe));
            ParameterName = baseParameter.ParameterName;
        }

        /// <summary>
        /// Sets the value of the parameter.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="mode">The mode.</param>
        internal void SetParameterValue<T>(
            SqlProgramParameter parameter,
            Input<T> value,
            TypeConstraintMode mode)
        {
            if (value.IsOutputValue)
                OutputValue = value.OutputValue;
            else
                SetParameterValue(parameter, value.Value, mode);
            IsOutputUsed = value.Value is IOut;
        }
        
        /// <summary>
        /// Sets the value of the parameter.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="mode">The mode.</param>
        protected internal abstract void SetParameterValue<T>(
            SqlProgramParameter parameter,
            T value,
            TypeConstraintMode mode);

        /// <summary>
        /// Gets the output value that is passed into this parameter.
        /// </summary>
        /// <value>The output value.</value>
        internal IOut OutputValue { get; private set; }

        [NotNull]
        private string _parameterName = String.Empty;

        /// <summary>Gets or sets the name of the <see cref="T:System.Data.Common.DbParameter" />.</summary>
        /// <returns>The name of the <see cref="T:System.Data.Common.DbParameter" />. The default is an empty string ("").</returns>
        public override string ParameterName
        {
            get => _parameterName;
            set
            {
                if (_parameterName == value) return;

                if (string.IsNullOrEmpty(value))
                {
                    _parameterName = string.Empty;
                    BaseParameter.ParameterName = "@" + Dedupe;
                }
                else
                {
                    string name = value;
                    if (name[0] != '@')
                        name = "@" + name;

                    _parameterName = name;

                    int maxLen = MaxParameterNameLength - Dedupe.Length;
                    BaseParameter.ParameterName = maxLen < name.Length
                        ? name.Substring(0, maxLen) + Dedupe
                        : name + Dedupe;
                }
            }
        }

        /// <summary>Resets the DbType property to its original settings.</summary>
        public override void ResetDbType() => BaseParameter.ResetDbType();

        /// <summary>Gets or sets the <see cref="T:System.Data.DbType" /> of the parameter.</summary>
        /// <returns>One of the <see cref="T:System.Data.DbType" /> values. The default is <see cref="F:System.Data.DbType.String" />.</returns>
        /// <exception cref="T:System.ArgumentException">The property is not set to a valid <see cref="T:System.Data.DbType" />.</exception>
        public override DbType DbType
        {
            get => BaseParameter.DbType;
            set => BaseParameter.DbType = value;
        }

        /// <summary>Gets or sets a value that indicates whether the parameter is input-only, output-only, bidirectional, or a stored procedure return value parameter.</summary>
        /// <returns>One of the <see cref="T:System.Data.ParameterDirection" /> values. The default is Input.</returns>
        /// <exception cref="T:System.ArgumentException">The property is not set to one of the valid <see cref="T:System.Data.ParameterDirection" /> values.</exception>
        public override ParameterDirection Direction
        {
            get => BaseParameter.Direction;
            set => BaseParameter.Direction = value;
        }

        /// <summary>Gets or sets a value that indicates whether the parameter accepts null values.</summary>
        /// <returns>true if null values are accepted; otherwise false. The default is false.</returns>
        public override bool IsNullable
        {
            get => BaseParameter.IsNullable;
            set => BaseParameter.IsNullable = value;
        }

        /// <summary>Gets or sets the maximum size, in bytes, of the data within the column.</summary>
        /// <returns>The maximum size, in bytes, of the data within the column. The default value is inferred from the parameter value.</returns>
        public override int Size
        {
            get => BaseParameter.Size;
            set => BaseParameter.Size = value;
        }

        /// <summary>Gets or sets the name of the source column mapped to the <see cref="T:System.Data.DataSet" /> and used for loading or returning the <see cref="P:System.Data.Common.DbParameter.Value" />.</summary>
        /// <returns>The name of the source column mapped to the <see cref="T:System.Data.DataSet" />. The default is an empty string.</returns>
        public override string SourceColumn
        {
            get => BaseParameter.SourceColumn;
            set => BaseParameter.SourceColumn = value;
        }

        /// <summary>Gets or sets the value of the parameter.</summary>
        /// <returns>An <see cref="T:System.Object" /> that is the value of the parameter. The default value is null.</returns>
        public override object Value
        {
            get => BaseParameter.Value;
            set => BaseParameter.Value = value;
        }

        /// <summary>Sets or gets a value which indicates whether the source column is nullable. This allows <see cref="T:System.Data.Common.DbCommandBuilder" /> to correctly generate Update statements for nullable columns.</summary>
        /// <returns>true if the source column is nullable; false if it is not.</returns>
        public override bool SourceColumnNullMapping
        {
            get => BaseParameter.SourceColumnNullMapping;
            set => BaseParameter.SourceColumnNullMapping = value;
        }

        /// <summary>Gets or sets the maximum number of digits used to represent the <see cref="P:System.Data.Common.DbParameter.Value" /> property.</summary>
        /// <returns>The maximum number of digits used to represent the <see cref="P:System.Data.Common.DbParameter.Value" /> property.</returns>
        public override byte Precision
        {
            get => BaseParameter.Precision;
            set => BaseParameter.Precision = value;
        }

        /// <summary>Gets or sets the number of decimal places to which <see cref="P:System.Data.Common.DbParameter.Value" /> is resolved.</summary>
        /// <returns>The number of decimal places to which <see cref="P:System.Data.Common.DbParameter.Value" /> is resolved.</returns>
        public override byte Scale
        {
            get => BaseParameter.Scale;
            set => BaseParameter.Scale = value;
        }

        /// <summary>Gets or sets the <see cref="T:System.Data.DataRowVersion" /> to use when you load <see cref="P:System.Data.Common.DbParameter.Value" />.</summary>
        /// <returns>One of the <see cref="T:System.Data.DataRowVersion" /> values. The default is Current.</returns>
        /// <exception cref="T:System.ArgumentException">The property is not set to one of the <see cref="T:System.Data.DataRowVersion" /> values.</exception>
        public override DataRowVersion SourceVersion
        {
            get => BaseParameter.SourceVersion;
            set => BaseParameter.SourceVersion = value;
        }
    }

    /// <summary>
    /// Represents a parameter to a <see cref="SqlBatchCommand"/>.
    /// </summary>
    /// <seealso cref="DbBatchParameter" />
    [PublicAPI]
    public sealed class SqlBatchParameter : DbBatchParameter
    {
        [NotNull]
        private new SqlParameter BaseParameter => (SqlParameter)base.BaseParameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbBatchParameter" /> class.
        /// </summary>
        /// <param name="programParameter">The program parameter.</param>
        /// <param name="baseParameter">The base parameter definition.</param>
        /// <param name="dedupe">The dedupe string.</param>
        public SqlBatchParameter(
            [NotNull] SqlProgramParameter programParameter,
            [NotNull] SqlParameter baseParameter,
            [NotNull] string dedupe)
            : base(programParameter, baseParameter, dedupe)
        {
        }

        /// <summary>
        /// Sets the value of the parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="mode">The mode.</param>
        protected internal override void SetParameterValue<T>(
            [NotNull] SqlProgramParameter parameter,
            T value,
            TypeConstraintMode mode)
        {
            parameter.SetSqlParameterValue(BaseParameter, value, mode);
            IsOutputUsed = value is IOut;
        }

        /// <summary>Gets or sets the <see cref="T:System.Globalization.CompareInfo" /> object that defines how string comparisons should be performed for this parameter.</summary>
        /// <returns>A <see cref="T:System.Globalization.CompareInfo" /> object that defines string comparison for this parameter.</returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*" />
        /// </PermissionSet>
        public SqlCompareOptions CompareInfo
        {
            get => BaseParameter.CompareInfo;
            set => BaseParameter.CompareInfo = value;
        }

        /// <summary>Gets or sets the locale identifier that determines conventions and language for a particular region.</summary>
        /// <returns>Returns the locale identifier associated with the parameter.</returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*" />
        /// </PermissionSet>
        public int LocaleId
        {
            get => BaseParameter.LocaleId;
            set => BaseParameter.LocaleId = value;
        }

        /// <summary>Resets the type associated with this <see cref="T:System.Data.SqlClient.SqlParameter" />.</summary>
        public void ResetSqlDbType() => BaseParameter.ResetSqlDbType();

        /// <summary>Gets or sets the <see cref="T:System.Data.SqlDbType" /> of the parameter.</summary>
        /// <returns>One of the <see cref="T:System.Data.SqlDbType" /> values. The default is NVarChar.</returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*" />
        /// </PermissionSet>
        public SqlDbType SqlDbType
        {
            get => BaseParameter.SqlDbType;
            set => BaseParameter.SqlDbType = value;
        }

        /// <summary>Gets or sets the value of the parameter as an SQL type.</summary>
        /// <returns>An <see cref="T:System.Object" /> that is the value of the parameter, using SQL types. The default value is null.</returns>
        public object SqlValue
        {
            get => BaseParameter.SqlValue;
            set => BaseParameter.SqlValue = value;
        }

        /// <summary>Gets or sets the type name for a table-valued parameter.</summary>
        /// <returns>The type name of the specified table-valued parameter.</returns>
        [NotNull]
        public string TypeName
        {
            get => BaseParameter.TypeName;
            set => BaseParameter.TypeName = value;
        }

        /// <summary>Gets or sets a string that represents a user-defined type as a parameter.</summary>
        /// <returns>A string that represents the fully qualified name of a user-defined type in the database.</returns>
        [NotNull]
        public string UdtTypeName
        {
            get => BaseParameter.UdtTypeName;
            set => BaseParameter.UdtTypeName = value;
        }

        /// <summary>Gets the name of the database where the schema collection for this XML instance is located.</summary>
        /// <returns>The name of the database where the schema collection for this XML instance is located.</returns>
        [NotNull]
        public string XmlSchemaCollectionDatabase
        {
            get => BaseParameter.XmlSchemaCollectionDatabase;
            set => BaseParameter.XmlSchemaCollectionDatabase = value;
        }

        /// <summary>Gets the name of the schema collection for this XML instance.</summary>
        /// <returns>The name of the schema collection for this XML instance.</returns>
        [NotNull]
        public string XmlSchemaCollectionName
        {
            get => BaseParameter.XmlSchemaCollectionName;
            set => BaseParameter.XmlSchemaCollectionName = value;
        }

        /// <summary>The owning relational schema where the schema collection for this XML instance is located.</summary>
        /// <returns>The owning relational schema for this XML instance.</returns>
        [NotNull]
        public string XmlSchemaCollectionOwningSchema
        {
            get => BaseParameter.XmlSchemaCollectionOwningSchema;
            set => BaseParameter.XmlSchemaCollectionOwningSchema = value;
        }
    }
}