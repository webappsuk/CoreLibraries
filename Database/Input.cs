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
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Represents an input value to a batched program call.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public struct Input<T>
    {
        /// <summary>
        /// The output value, if <see cref="IsOutputValue"/>.
        /// </summary>
        /// <value>
        /// The output value.
        /// </value>
        public readonly Out<T> OutputValue;

        /// <summary>
        /// The value, if not <see cref="IsOutputValue"/>.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public readonly T Value;

        /// <summary>
        /// Gets a value indicating whether the value is the output of a previous command
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if the value is an output value; otherwise, <see langword="false" />.
        /// </value>
        public bool IsOutputValue => OutputValue != null;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Input{T}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Input(T value)
        {
            Value = value;
            OutputValue = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Input{T}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Input([NotNull] Out<T> value)
        {
            OutputValue = value ?? throw new ArgumentNullException(nameof(value));
            Value = default(T);
        }

        /// <summary>
        /// Performs an implicit conversion from <typeparamref name="T"/> to <see cref="Input{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Input<T>(T value) => new Input<T>(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Out{T}"/> to <see cref="Input{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Input<T>(Out<T> value) => new Input<T>(value);
    }
}