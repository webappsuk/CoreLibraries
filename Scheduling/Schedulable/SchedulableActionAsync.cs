#region � Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
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

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling.Schedulable
{
    /// <summary>
    /// Creates an asyncrhonous schedulable action from an <see cref="Action"/>.
    /// </summary>
    /// <remarks></remarks>
    public class SchedulableActionAsync : SchedulableActionAsyncBase
    {
        /// <summary>
        /// Whether the action needs the task parameter.
        /// </summary>
        private readonly bool _needsTask = true;

        /// <summary>
        /// The action.
        /// </summary>
        [NotNull]
        private readonly Action<object> _action;

        /// <summary>
        /// The task state.
        /// </summary>
        [CanBeNull]
        private readonly object _state;

        /// <summary>
        /// The task creation options.
        /// </summary>
        private readonly TaskCreationOptions _creationOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulableActionAsync"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <remarks></remarks>
        public SchedulableActionAsync([NotNull] Action action)
            : this(o => action(), null, TaskCreationOptions.None)
        {
            _needsTask = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulableActionAsync"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="creationOptions">The creation options.</param>
        /// <remarks></remarks>
        public SchedulableActionAsync([NotNull] Action action, TaskCreationOptions creationOptions)
            : this(o => action(), null, creationOptions)
        {
            _needsTask = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulableActionAsync"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="state">The state.</param>
        /// <param name="creationOptions">The creation options.</param>
        /// <remarks></remarks>
        public SchedulableActionAsync(
            [NotNull] Action<object> action,
            [CanBeNull] object state,
            TaskCreationOptions creationOptions)
        {
            _action = action;
            _state = state;
            _creationOptions = creationOptions;
        }

        /// <inheritdoc/>
        public override Task ExecuteAsync()
        {
            return new Task(_action, _state, _creationOptions);
        }

        /// <inheritdoc/>
        public override void Execute()
        {
            if (!_needsTask)
                _action(null);
            else
                base.Execute();
        }
    }
}