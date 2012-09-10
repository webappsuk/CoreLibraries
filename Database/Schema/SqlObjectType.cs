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

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   The type of a <see cref="WebApplications.Utilities.Database.SqlProgram"/> object.
    /// </summary>
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