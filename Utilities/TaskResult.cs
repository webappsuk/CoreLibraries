#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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

using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Usefull completed Tasks.
    /// </summary>
    public static class TaskResult
    {
        /// <summary>
        /// The completed result
        /// </summary>
        [NotNull, PublicAPI]
        public static readonly Task Completed;

        /// <summary>
        /// A task that returns a <see langword="true"/>
        /// </summary>
        [NotNull, PublicAPI]
        public static readonly Task<bool> True;

        /// <summary>
        /// A task that returns a <see langword="false"/>
        /// </summary>
        [NotNull, PublicAPI]
        public static readonly Task<bool> False;

        /// <summary>
        /// A task that returns a <c>0</c>
        /// </summary>
        [NotNull, PublicAPI]
        public static readonly Task<int> Zero;

        /// <summary>
        /// A task that returns <see cref="System.Int32.MinValue"/>
        /// </summary>
        [NotNull, PublicAPI]
        public static readonly Task<int> MinInt;

        /// <summary>
        /// A task that returns <see cref="System.Int32.MaxValue"/>
        /// </summary>
        [NotNull, PublicAPI]
        public static readonly Task<int> MaxInt;

        /// <summary>
        /// Initializes static members of the <see cref="TaskResult"/> class.
        /// </summary>
        static TaskResult()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            True = Task.FromResult(true);
            Completed = True;
            False = Task.FromResult(false);
            Zero = Task.FromResult(0);
            MinInt = Task.FromResult(int.MinValue);
            MaxInt = Task.FromResult(int.MaxValue);
            // ReSharper restore AssignNullToNotNullAttribute
        }
    }

    /// <summary>
    /// Usefull completed Tasks.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class TaskResult<T>
    {
        /// <summary>
        /// A task that returns the <see langword="default"/> value for the type <see cref="T"/>.
        /// </summary>
        [NotNull, PublicAPI]
        public static readonly Task<T> Default;

        /// <summary>
        /// Initializes static members of the <see cref="TaskResult{T}"/> class.
        /// </summary>
        static TaskResult()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Default = Task.FromResult(default(T));
            // ReSharper restore AssignNullToNotNullAttribute
        }
    }
}