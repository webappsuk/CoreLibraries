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
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Service.Common;
using WebApplications.Utilities.Service.Common.Protocol;

namespace WebApplications.Utilities.Service.Client
{
    public partial class NamedPipeClient
    {
        /// <summary>
        /// Information about an ongoing command.
        /// </summary>
        private class ConnectedCommand : IDisposable
        {
            /// <summary>
            /// The request.
            /// </summary>
            [NotNull]
            [PublicAPI]
            public readonly Request Request;

            /// <summary>
            /// The observer of responses.
            /// </summary>
            private IObserver<Response> _observer;

            /// <summary>
            /// The completion handle signal completion.
            /// </summary>
            private TaskCompletionSource<bool> _completionTask;

            /// <summary>
            /// Any responses received before they were expected.
            /// </summary>
            [NotNull]
            private readonly LinkedList<CommandResponse> _oooResponses = new LinkedList<CommandResponse>();

            /// <summary>
            /// The expected sequence number.
            /// </summary>
            private int _expectedSequence;

            private bool _isCancelled;

            /// <summary>
            /// Gets the completion task.
            /// </summary>
            /// <value>The completion task.</value>
            [NotNull]
            public Task CompletionTask
            {
                get
                {
                    TaskCompletionSource<bool> cts = _completionTask;
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return cts == null
                        ? TaskResult.False
                        : cts.Task;
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ConnectedCommand" /> class.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="observer">The observer.</param>
            public ConnectedCommand([NotNull] Request request, [NotNull] IObserver<Response> observer)
            {
                Request = request;
                _observer = observer;
                _completionTask = new TaskCompletionSource<bool>();
            }

            /// <summary>
            /// Gets a value indicating whether this instance is explicitly cancelled by the server.
            /// </summary>
            /// <value>
            /// <see langword="true" /> if this instance is cancelled; otherwise, <see langword="false" />.
            /// </value>
            public bool IsCancelled
            {
                get { return _isCancelled; }
            }

            /// <summary>
            /// Gets a value indicating whether this instance is completed.
            /// </summary>
            /// <value>
            /// <see langword="true" /> if this instance is completed; otherwise, <see langword="false" />.
            /// </value>
            public bool IsCompleted
            {
                get
                {
                    TaskCompletionSource<bool> tcs = _completionTask;
                    return tcs == null ||
                           // ReSharper disable once PossibleNullReferenceException
                           tcs.Task.IsCompleted ||
                           tcs.Task.IsFaulted ||
                           tcs.Task.IsCanceled;
                }
            }

            /// <summary>
            /// Cancels the specified request.
            /// </summary>
            /// <param name="response">The response.</param>
            // ReSharper disable once UnusedParameter.Local
            public void Cancel([NotNull] CommandCancelResponse response)
            {
                if (response == null) throw new ArgumentNullException("response");
                if (response.CancelledCommandId != Request.ID)
                    throw new ArgumentException(CommonResources.Cancel_IDMismatch, "response");

                _isCancelled = true;
                Dispose();
            }

            /// <summary>
            /// Received the specified connected command.
            /// </summary>
            /// <param name="response">The response.</param>
            /// <returns><see langword="true"/> if the response is complete; otherwise <see langword="false"/>.</returns>
            public bool Received([NotNull] Response response)
            {
                if (response == null) throw new ArgumentNullException("response");

                IObserver<Response> observer = _observer;
                if (observer == null) return true;
                bool complete = false;
                Exception error = null;
                try
                {
                    lock (_oooResponses)
                    {
                        CommandResponse commandResponse = response as CommandResponse;
                        if (commandResponse == null)
                        {
                            observer.OnNext(response);
                            complete = true;
                            return true;
                        }
                        int sequence = commandResponse.Sequence;

                        if (sequence != _expectedSequence)
                        {
                            if (sequence < 0)
                            {
                                complete = true;
                                if (_oooResponses.Count > 0)
                                {
                                    // Suppress actual completion/error, as we received out of order messages.
                                    error =
                                        new ApplicationException(
                                            ClientResources.Err_ConnectedCommand_Received_MissingSequenceElements);
                                    return true;
                                }

                                if (sequence == -1)
                                    observer.OnNext(response);
                                else
                                    error = new ApplicationException(((CommandResponse)response).Chunk);
                                return true;
                            }

                            if (sequence < _expectedSequence)
                            {
                                Log.Add(
                                    LoggingLevel.Warning,
                                    () => ClientResources.Wrn_ConnectedCommand_Received_DuplicateSequence);
                                return false;
                            }

                            LinkedListNode<CommandResponse> current = _oooResponses.First;
                            while (current != null &&
                                   // ReSharper disable once PossibleNullReferenceException
                                   current.Value.Sequence < sequence)
                                current = current.Next;

                            if (current == null)
                                _oooResponses.AddLast(commandResponse);
                            // ReSharper disable once PossibleNullReferenceException
                            else if (current.Value.Sequence < sequence)
                                _oooResponses.AddAfter(current, commandResponse);
                            else
                                _oooResponses.AddBefore(current, commandResponse);
                        }
                        else
                        {
                            observer.OnNext(response);
                            _expectedSequence++;
                            while (_oooResponses.First != null &&
                                   // ReSharper disable once PossibleNullReferenceException
                                   _oooResponses.First.Value.Sequence == _expectedSequence)
                            {
                                observer.OnNext(_oooResponses.First.Value);
                                _expectedSequence++;
                                _oooResponses.RemoveFirst();
                            }
                        }
                    }
                }
                catch
                {
                    complete = true;
                }
                finally
                {
                    if (complete)
                    {
                        _observer = null;
                        TaskCompletionSource<bool> cts = Interlocked.Exchange(ref _completionTask, null);
                        if (cts != null) cts.TrySetResult(true);

                        if (error != null)
                            observer.OnError(error);
                        else
                            observer.OnCompleted();
                    }
                }
                return complete;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                TaskCompletionSource<bool> cts = Interlocked.Exchange(ref _completionTask, null);
                if (cts != null)
                    cts.TrySetCanceled();

                IObserver<Response> observer = Interlocked.Exchange(ref _observer, null);
                if (observer != null)
                    try
                    {
                        observer.OnError(new TaskCanceledException());
                    }
                        // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                    }
            }
        }
    }
}