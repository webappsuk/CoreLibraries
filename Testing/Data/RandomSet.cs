using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

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
        /// <remarks></remarks>
        public RandomSet(
            int columns = 0,
            int minRows = 0,
            int maxRows = 1000,
            double nullProbability = 0.1)
            : this(Tester.RandomGenerator.RandomRecordSetDefinition(columns), minRows, maxRows, nullProbability)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSet" /> class.
        /// </summary>
        /// <param name="recordSetDefinition">The record set definition.</param>
        /// <param name="minRows">The min rows.</param>
        /// <param name="maxRows">The max rows.</param>
        /// <param name="nullProbability">The probability of a column's value being set to SQL null (0.0 for no nulls) [Defaults to 0.1 = 10%].</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        public RandomSet([NotNull]RecordSetDefinition recordSetDefinition, int minRows = 0, int maxRows = 1000, double nullProbability = 0.1)
            : base(recordSetDefinition, GenerateRecords(recordSetDefinition, minRows, maxRows, nullProbability))
        {
        }

        /// <summary>
        /// Generates the records.
        /// </summary>
        /// <param name="recordSetDefinition">The record set definition.</param>
        /// <param name="minRows">The min rows.</param>
        /// <param name="maxRows">The max rows.</param>
        /// <param name="nullProbability">The probability of a column's value being set to SQL null (0.0 for no nulls) [Defaults to 0.1 = 10%].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        [NotNull]
        private static IEnumerable<IObjectRecord> GenerateRecords([NotNull]RecordSetDefinition recordSetDefinition, int minRows, int maxRows, double nullProbability)
        {
            if (minRows < 0)
                throw new ArgumentOutOfRangeException("minRows", minRows,
                                                      String.Format(
                                                          "The minimum number of rows '{0}' cannot be negative.",
                                                          minRows));
            if (maxRows < 0)
                throw new ArgumentOutOfRangeException("maxRows", maxRows,
                                                      String.Format(
                                                          "The maximum number of rows '{0}' cannot be negative.",
                                                          maxRows));

            if (minRows > maxRows)
            {
                throw new ArgumentOutOfRangeException("minRows", minRows,
                                                      String.Format(
                                                          "The minimum number of rows '{0}' cannot exceed the maximum number of rows '{1}'.",
                                                          minRows,
                                                          maxRows));
            }

            // Calculate number of rows.
            int rows = minRows == maxRows
                           ? minRows
                           : Tester.RandomGenerator.Next(minRows, maxRows);

            if (rows < 1)
                return Enumerable.Empty<IObjectRecord>();

            // Create random records
            List<IObjectRecord> records = new List<IObjectRecord>();
            for (int r = 0; r < rows; r++)
                records.Add(new ObjectRecord(recordSetDefinition, true, nullProbability));

            return records;
        } 
    }
}