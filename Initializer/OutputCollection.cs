using System;
using System.Collections;
using System.Collections.Generic;

namespace WebApplications.Utilities.Initializer
{
    /// <summary>
    /// Collection of outputs.
    /// </summary>
    /// <remarks></remarks>
    internal class OutputCollection : IEnumerable<Output>
    {
        /// <summary>
        /// List of outputs.
        /// </summary>
        private readonly List<Output> _outputs = new List<Output>();

        /// <summary>
        /// Has errors.
        /// </summary>
        public bool HasErrors;

        /// <summary>
        /// Has warnings.
        /// </summary>
        public bool HasWarnings;

        /// <summary>
        /// Adds the specified importance.
        /// </summary>
        /// <param name="importance">The importance.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Output Add(OutputImportance importance, string format, params object[] args)
        {
            Output o = new Output(importance, format, args);
            _outputs.Add(o);
            switch (importance)
            {
                case OutputImportance.Error:
                    HasErrors = true;
                    break;
                case OutputImportance.Warning:
                    HasWarnings = true;
                    break;
            }
            return o;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.</returns>
        /// <remarks></remarks>
        public IEnumerator<Output> GetEnumerator()
        {
            return _outputs.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        /// <remarks></remarks>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}