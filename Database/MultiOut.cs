#region © Copyright Web Applications (UK) Ltd, 2016.  All rights reserved.
// Copyright (c) 2016, Web Applications UK Ltd
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
using System.Data.SqlClient;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Used to get the value of an output parameter from a <see cref="SqlProgram"/> 
    /// when executing the program against multiple connections.
    /// </summary>
    /// <remarks>The value of an output parameter is only available after the <see cref="SqlProgram"/> 
    /// has completed and the <see cref="SqlDataReader"/> has been disposed.</remarks>
    /// <typeparam name="T">The type of the value to get.</typeparam>
    public class MultiOut<T> : Out<T>, IMultiOut, IReadOnlyCollection<Out<T>>
    {
        [NotNull]
        private readonly Dictionary<SqlParameter, Out<T>> _outputs = new Dictionary<SqlParameter, Out<T>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiOut{T}"/> class with no input value.
        /// </summary>
        public MultiOut()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiOut{T}"/> class with an input value.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        public MultiOut(T initialValue)
            : base(initialValue)
        {
        }

        /// <summary>
        /// Sets the parameter to get the value from.
        /// </summary>
        /// <param name="parameter">The parameter to get the value from.</param>
        /// <returns>
        /// The instance of <see cref="Out{T}" /> for the parameter.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is <see langword="null" />.</exception>
        protected override void SetParameter(SqlParameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            Out<T> output;
            if (_outputs.TryGetValue(parameter, out output))
                return;

            output = InputValue.IsAssigned ? new Out<T>(InputValue.Value) : new Out<T>();

            _outputs.Add(parameter, output);
        }

        /// <summary>
        /// Sets the output value for the parameter given.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameter">The parameter to set the value of.</param>
        /// <exception cref="ArgumentException">The value of this parameter has already been set.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is <see langword="null" />.</exception>
        protected internal override void SetOutputValue(object value, [NotNull] SqlParameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            Out<T> output;
            if (_outputs.TryGetValue(parameter, out output))
                output.SetOutputValue(value, parameter);
            else
                throw new ArgumentException(Resources.MultiOut_SetOutputValue_ParameterMismatch);
        }

        /// <summary>
        /// Indicates that the value returned by the database is invalid for the type of the parameter.
        /// </summary>
        /// <param name="exception">The exception that indicates the error.</param>
        /// <param name="parameter">The parameter to set the exception of.</param>
        /// <exception cref="ArgumentException">The value of this parameter has already been set.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is <see langword="null" />.</exception>
        protected internal override void SetOutputError(Exception exception, SqlParameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            Out<T> output;
            if (_outputs.TryGetValue(parameter, out output))
                output.SetOutputError(exception, parameter);
            else
                throw new ArgumentException(Resources.MultiOut_SetOutputValue_ParameterMismatch);
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count => _outputs.Count;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}" /> object that can be used to iterate through the collection.</returns>
        public IEnumerator<Out<T>> GetEnumerator() => ((IEnumerable<Out<T>>)_outputs.Values).GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}" /> object that can be used to iterate through the collection.</returns>
        IEnumerator<IOut> IEnumerable<IOut>.GetEnumerator() => ((IEnumerable<IOut>)_outputs.Values).GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_outputs.Values).GetEnumerator();
    }

    /// <summary>
    /// Internal interface used to identify <see cref="MultiOut{T}"/> from non-generic methods.
    /// </summary>
    internal interface IMultiOut : IOut, IEnumerable<IOut>
    {
    }
}