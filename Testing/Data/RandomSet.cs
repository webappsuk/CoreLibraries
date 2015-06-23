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
using System.Linq;
using WebApplications.Testing.Annotations;

namespace WebApplications.Testing.Data
{
    /// <summary>
    /// Create a random record set.
    /// </summary>
    /// <remarks></remarks>
    public class RandomSet : ObjectSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSet" /> class.
        /// </summary>
        /// <param name="columns">The number of columns, if less than one, then a random number is chosen.</param>
        /// <param name="minRows">The minimum number of rows [defaults to 0].</param>
        /// <param name="maxRows">The maximum number of rows [defaults to 1000].</param>
        /// <param name="nullProbability">The probability of a column's value being set to SQL null (0.0 for no nulls) [Defaults to 0.1 = 10%].</param>
        /// <param name="columnGenerators">The column generators is an array of functions that generate a value for each column, if the function is
        /// <see langword="null"/> for a particular index then a random value is generated, if it is not null then the function is used.  The function takes
        /// the current row number as it's only parameter and must return an object of the correct type for the column.</param>
        /// <remarks></remarks>
        public RandomSet(
            int columns = 0,
            int minRows = 0,
            int maxRows = 1000,
            double nullProbability = 0.1,
            [CanBeNull] Func<int, object>[] columnGenerators = null)
            : this(
                Tester.RandomGenerator.RandomRecordSetDefinition(columns),
                minRows,
                maxRows,
                nullProbability,
                columnGenerators)
        {
            if (minRows < 0) throw new ArgumentOutOfRangeException("minRows");
            if (maxRows < 0) throw new ArgumentOutOfRangeException("maxRows");
            if (minRows > maxRows) throw new ArgumentOutOfRangeException("maxRows");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSet" /> class.
        /// </summary>
        /// <param name="recordSetDefinition">The record set definition.</param>
        /// <param name="minRows">The min rows.</param>
        /// <param name="maxRows">The max rows.</param>
        /// <param name="nullProbability">The probability of a column's value being set to SQL null (0.0 for no nulls) [Defaults to 0.1 = 10%].</param>
        /// <param name="columnGenerators">The column generators is an array of functions that generate a value for each column, if the function is
        /// <see langword="null"/> for a particular index then a random value is generated, if it is not null then the function is used.  The function takes
        /// the current row number as it's only parameter and must return an object of the correct type for the column.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        public RandomSet(
            [NotNull] RecordSetDefinition recordSetDefinition,
            int minRows = 0,
            int maxRows = 1000,
            double nullProbability = 0.1,
            [CanBeNull] Func<int, object>[] columnGenerators = null)
            : base(
                recordSetDefinition,
                GenerateRecords(recordSetDefinition, minRows, maxRows, nullProbability, columnGenerators))
        {
        }

        /// <summary>
        /// Generates the records.
        /// </summary>
        /// <param name="recordSetDefinition">The record set definition.</param>
        /// <param name="minRows">The min rows.</param>
        /// <param name="maxRows">The max rows.</param>
        /// <param name="nullProbability">The probability of a column's value being set to SQL null (0.0 for no nulls) [Defaults to 0.1 = 10%].</param>
        /// <param name="columnGenerators">The column generators is an array of functions that generate a value for each column, if the function is
        /// <see langword="null"/> for a particular index then a random value is generated, if it is not null then the function is used.  The function takes
        /// the current row number as it's only parameter and must return an object of the correct type for the column.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        ///   <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        [NotNull]
        private static IEnumerable<IObjectRecord> GenerateRecords(
            [NotNull] RecordSetDefinition recordSetDefinition,
            int minRows,
            int maxRows,
            double nullProbability,
            [CanBeNull] Func<int, object>[] columnGenerators = null)
        {
            if (recordSetDefinition == null) throw new ArgumentNullException("recordSetDefinition");
            if (minRows < 0) throw new ArgumentOutOfRangeException("minRows");
            if (maxRows < 0) throw new ArgumentOutOfRangeException("maxRows");
            if (minRows > maxRows) throw new ArgumentOutOfRangeException("maxRows");

            // Calculate number of rows.
            int rows = minRows == maxRows
                ? minRows
                : Tester.RandomGenerator.Next(minRows, maxRows);

            if (rows < 1)
                return Enumerable.Empty<IObjectRecord>();

            // Create random records
            List<IObjectRecord> records = new List<IObjectRecord>();
            for (int r = 0; r < rows; r++)
                records.Add(new ObjectRecord(recordSetDefinition, true, nullProbability, columnGenerators, r + 1));

            return records;
        }
    }
}