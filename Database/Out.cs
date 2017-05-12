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
using System.Data.Common;
using System.Data.SqlClient;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Used to get the value of an output parameter from a <see cref="SqlProgram"/>.
    /// </summary>
    /// <remarks>The value of an output parameter is only available after the <see cref="SqlProgram"/> 
    /// has completed and the <see cref="SqlDataReader"/> has been disposed.</remarks>
    /// <typeparam name="T">The type of the value to get.</typeparam>
    /// <exmaple>
    /// <code>
    /// SqlProgram&lt;int, Out&lt;string&gt;, Out&lt;int&gt;&gt; program = null;
    /// 
    /// int input = 123;
    /// Out&lt;string&gt; output = new Out&lt;string&gt;();
    /// Out&lt;int&gt; inputOutput = new Out&lt;int&gt;(456);
    /// 
    /// await program.ExecuteReaderAsync(
    ///     (reader, token) =&gt;
    ///     {
    ///         // Read results
    ///     },
    ///     input,
    ///     output,
    ///     inputOutput);
    /// 
    /// Console.WriteLine("output value: {0}", output.Value);
    /// Console.WriteLine("inputOutput value: {0}", inputOutput.Value);
    /// </code>
    /// </exmaple>
    /// <seealso cref="MultiOut{T}"/>
    [PublicAPI]
    public class Out<T> : IOut
    {
        private DbParameter _parameter;

        private readonly Optional<T> _inputValue;

        private Optional<T> _outputValue;

        private Exception _outputError;

        /// <summary>
        /// Initializes a new instance of the <see cref="Out{T}"/> class with no input value.
        /// </summary>
        public Out()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Out{T}"/> class with an input value.
        /// </summary>
        /// <param name="inputValue">The input value.</param>
        public Out(T inputValue)
        {
            _inputValue = new Optional<T>(inputValue);
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <value>
        /// The type of the value.
        /// </value>
        Type IOut.Type => typeof(T);

        /// <summary>
        /// Gets the optional input value.
        /// </summary>
        /// <value>
        /// The optional input value.
        /// </value>
        Optional<object> IOut.InputValue
            => _inputValue.IsAssigned ? new Optional<object>(_inputValue.Value) : Optional<object>.Unassigned;

        /// <summary>
        /// Gets the optional input value.
        /// </summary>
        /// <value>
        /// The optional input value.
        /// </value>
        public Optional<T> InputValue => _inputValue;

        /// <summary>
        /// Gets the output value.
        /// </summary>
        /// <value>
        /// The output value.
        /// </value>
        public virtual Optional<T> OutputValue
        {
            get
            {
                _outputError?.ReThrow();
                return _outputValue;
            }
        }

        /// <summary>
        /// Gets the error that occured when fetching the output, if any.
        /// </summary>
        /// <value>
        /// The output error.
        /// </value>
        [CanBeNull]
        public virtual Exception OutputError => _outputError;

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        /// <value>
        /// The <see cref="Optional{T}.Value" /> of <see cref="OutputValue" /> if it <see cref="Optional{T}.IsAssigned" />,
        /// otherwise the value of <see cref="InputValue" />.
        /// </value>
        public T Value => OutputValue.IsAssigned ? OutputValue.Value : InputValue.Value;

        /// <summary>
        /// Gets a value indicating whether this parameter has a value.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if either <see cref="InputValue"/> or <see cref="OutputValue"/> is 
        /// <see cref="Optional{T}.IsAssigned">assigned</see>; otherwise, <see langword="false" />.
        /// </value>
        public bool HasValue => _outputValue.IsAssigned || _inputValue.IsAssigned;

        /// <summary>
        /// Sets the parameter to get the value from.
        /// </summary>
        /// <param name="parameter">The parameter to get the value from.</param>
        /// <returns>
        /// The instance of <see cref="IOut" /> for the parameter.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is <see langword="null" />.</exception>
        void IOut.SetParameter(DbParameter parameter) => SetParameter(parameter);

        /// <summary>
        /// Sets the parameter to get the value from.
        /// </summary>
        /// <param name="parameter">The parameter to get the value from.</param>
        /// <returns>
        /// The instance of <see cref="Out{T}" /> for the parameter.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">This value is already being used by a parameter.</exception>
        protected internal virtual void SetParameter(DbParameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (_parameter != null || _outputValue.IsAssigned)
                throw new InvalidOperationException(Resources.Out_SetParameter_AlreadyInUse);

            _parameter = parameter;
        }

        /// <summary>
        /// Sets the output value for the parameter given.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameter">The parameter to set the value of.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">The value of this parameter has already been set.</exception>
        void IOut.SetOutputValue(object value, DbParameter parameter) => SetOutputValue(value, parameter);

        /// <summary>
        /// Sets the output value for the parameter given.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameter">The parameter to set the value of.</param>
        /// <exception cref="ArgumentException">The value of this parameter has already been set.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is <see langword="null" />.</exception>
        protected internal virtual void SetOutputValue(object value, DbParameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (_parameter == null || _outputValue.IsAssigned)
                throw new ArgumentException(Resources.Out_SetOutputValue_ValueAlreadySet);
            if (_parameter != parameter)
                throw new ArgumentException(Resources.Out_SetOutputValue_ParameterMismatch);
            _outputValue = new Optional<T>((T)value);
            _parameter = null;
        }

        /// <summary>
        /// Indicates that the value returned by the database is invalid for the type of the parameter.
        /// </summary>
        /// <param name="exception">The exception that indicates the error.</param>
        /// <param name="parameter">The parameter to set the exception of.</param>
        /// <exception cref="ArgumentException">The value of this parameter has already been set.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is <see langword="null" />.</exception>
        void IOut.SetOutputError(Exception exception, DbParameter parameter) => SetOutputError(exception, parameter);

        /// <summary>
        /// Indicates that the value returned by the database is invalid for the type of the parameter.
        /// </summary>
        /// <param name="exception">The exception that indicates the error.</param>
        /// <param name="parameter">The parameter to set the exception of.</param>
        /// <exception cref="ArgumentException">The value of this parameter has already been set.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is <see langword="null" />.</exception>
        protected internal virtual void SetOutputError(Exception exception, DbParameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (_parameter == null || _outputError != null)
                throw new ArgumentException(Resources.Out_SetOutputValue_ValueAlreadySet);
            if (_parameter != parameter)
                throw new ArgumentException(Resources.Out_SetOutputValue_ParameterMismatch);
            _outputError = exception;
            _parameter = null;
        }
    }
}