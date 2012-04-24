#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Database 
// Project: Utilities.Database
// File: SqlObjectType.cs
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

namespace WebApplications.Testing.Data
{
    /// <summary>
    /// Sql Object types.
    /// </summary>
    /// <remarks></remarks>
    public enum SqlObjectType
    {
        /// <summary>
        ///   User-Defined Table.
        /// </summary>
        U,

        /// <summary>
        ///   User-Defined Table.
        /// </summary>
        Table = U,

        /// <summary>
        ///   Internal Table.
        /// </summary>
        IT,

        /// <summary>
        ///   Internal Table.
        /// </summary>
        InternalTable = IT,

        /// <summary>
        ///   System Base Table.
        /// </summary>
        S,

        /// <summary>
        ///   System Base Table.
        /// </summary>
        SystemBaseTable = S,

        /// <summary>
        ///   Table type.
        /// </summary>
        TT,

        /// <summary>
        ///   Table type.
        /// </summary>
        TableType = TT,

        /// <summary>
        ///   View.
        /// </summary>
        V,

        /// <summary>
        ///   View.
        /// </summary>
        View = V,

        /// <summary>
        ///   DEFAULT Constraint.
        /// </summary>
        D,

        /// <summary>
        ///   DEFAULT Constraint.
        /// </summary>
        DefaultConstraint = D,

        /// <summary>
        ///   CHECK Constraint.
        /// </summary>
        C,

        /// <summary>
        ///   CHECK Constraint.
        /// </summary>
        CheckConstraint = C,

        /// <summary>
        ///   FOREIGN KEY Constraint.
        /// </summary>
        F,

        /// <summary>
        ///   FOREIGN KEY Constraint.
        /// </summary>
        ForeignKeyConstraint = F,

        /// <summary>
        ///   UNIQUE Constraint.
        /// </summary>
        UQ,

        /// <summary>
        ///   UNIQUE Constraint.
        /// </summary>
        UniqueConstraint = UQ,

        /// <summary>
        ///   PRIMARY KEY Constraint.
        /// </summary>
        PK,

        /// <summary>
        ///   PRIMARY KEY Constraint.
        /// </summary>
        PrimaryKeyConstraint = PK,

        /// <summary>
        ///   Scalar-Valued function.
        /// </summary>
        FN,

        /// <summary>
        ///   Scalar-Valued function.
        /// </summary>
        ScalarFunction = FN,

        /// <summary>
        ///   CLR Scalar-Valued Function.
        /// </summary>
        FS,

        /// <summary>
        ///   CLR Scalar-Valued Function.
        /// </summary>
        CLRScalarFunction = FS,

        /// <summary>
        ///   Table-Valued Function.
        /// </summary>
        TF,

        /// <summary>
        ///   Table-Valued Function.
        /// </summary>
        TableValuedFunction = TF,

        /// <summary>
        ///   Inline Table-Valued Function.
        /// </summary>
        IF,

        /// <summary>
        ///   Inline Table-Valued Function.
        /// </summary>
        InlineTableValuedFunction = IF,

        /// <summary>
        ///   CLR Table-Valued Function.
        /// </summary>
        FT,

        /// <summary>
        ///   CLR Table-Valued Function.
        /// </summary>
        CLRTableValuedFunction = FT,

        /// <summary>
        ///   Aggregate Function.
        /// </summary>
        AF,

        /// <summary>
        ///   Aggregate Function.
        /// </summary>
        AggregateFunction = AF,

        /// <summary>
        ///   Stored Procedure.
        /// </summary>
        P,

        /// <summary>
        ///   Stored Procedure.
        /// </summary>
        StoredProcedure = P,

        /// <summary>
        ///   CLR Stored Procedure.
        /// </summary>
        PC,

        /// <summary>
        ///   CLR Stored Procedure.
        /// </summary>
        CLRStoredProcedure = PC,

        /// <summary>
        ///   Extended Stored Procedure.
        /// </summary>
        X,

        /// <summary>
        ///   Extended Stored Procedure.
        /// </summary>
        ExtendedStoredProcedure = X,

        /// <summary>
        ///   Transactional Replication filter.
        /// </summary>
        RF,

        /// <summary>
        ///   Transactional Replication filter.
        /// </summary>
        ReplicationFilterProcedure = RF,

        /// <summary>
        ///   Plan Guide.
        /// </summary>
        PG,

        /// <summary>
        ///   Plan Guide.
        /// </summary>
        PlanGuide = PG,

        /// <summary>
        ///   RULE object.
        /// </summary>
        R,

        /// <summary>
        ///   RULE object.
        /// </summary>
        Rule = R,

        /// <summary>
        ///   A SYNONYM.
        /// </summary>
        SN,

        /// <summary>
        ///   A SYNONYM.
        /// </summary>
        Synonym = SN,

        /// <summary>
        ///   Service Broker Queue.
        /// </summary>
        SQ,

        /// <summary>
        ///   Service Broker Queue.
        /// </summary>
        ServiceQueue = SQ,

        /// <summary>
        ///   DML Trigger.
        /// </summary>
        TR,

        /// <summary>
        ///   DML Trigger.
        /// </summary>
        DMLTrigger = TR,

        /// <summary>
        ///   CLR DML Trigger.
        /// </summary>
        TA,

        /// <summary>
        ///   CLR DML Trigger.
        /// </summary>
        CLRDMLTrigger = TA
    }
}