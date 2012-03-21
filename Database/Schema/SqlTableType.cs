#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Database 
// Project: Utilities.Database
// File: SqlTableType.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   Extends SqlType to store column information for table types.
    /// </summary>
    public class SqlTableType : SqlType
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlTableType"/> class.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="name">The table name.</param>
        /// <param name="defaultSize">The default size information.</param>
        /// <param name="isNullable">
        ///   If set to <see langword="true"/> the value can be <see langword="null"/>.
        /// </param>
        /// <param name="isUserDefined">
        ///   If set to <see langword="true"/> the value is a user defined type.
        /// </param>
        /// <param name="isClr">
        ///   If set to <see langword="true"/> the value is a CLR type.
        /// </param>
        internal SqlTableType(
            [CanBeNull]SqlType baseType,
            [NotNull]string schemaName,
            [NotNull]string name, 
            SqlTypeSize defaultSize,
            bool isNullable,
            bool isUserDefined,
            bool isClr)
            : base(baseType, schemaName, name, defaultSize, isNullable, isUserDefined, isClr, true)
        {
        }

        /// <summary>
        ///   Gets the <see cref="SqlTableDefinition"/> associated with this type.
        /// </summary>
        /// <value>The definition for the table.</value>
        [NotNull]
        public SqlTableDefinition TableDefinition { get; internal set; }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   <para>A <see cref="string"/> representation of this instance.</para>
        ///   <para><b>Format:</b> <see cref="SqlType.FullName"/> + "Table Type".</para>
        /// </returns>
        public override string ToString()
        {
            return FullName + " Table Type";
        }
    }
}