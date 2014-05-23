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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Service.PipeProtocol;

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
            /// Received the specified connected command.
            /// </summary>
            /// <param name="response">The response.</param>
            /// <returns><see langword="true"/> if the response is complete; otherwise <see langword="false"/>.</returns>
            public bool Received([NotNull] Response response)
            {
                Contract.Requires<RequiredContractException>(response != null, "Parameter_Null");

                IObserver<Response> observer = _observer;
                if (observer == null) return true;
                bool complete = false;
                try
                {
                    lock (_oooResponses)
                    {
                        CommandResponse commandResponse = response as CommandResponse;

                        if (commandResponse == null)
                        {
                            observer.OnNext(response);
                            observer.OnCompleted();
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
                                    // Suppress actual completion/error, as we
                                    observer.OnError(
                                        new LoggingException(
                                            () => ClientResources.Err_ConnectedCommand_Received_MissingSequenceElements));
                                    return true;
                                }

                                if (sequence == -1)
                                {
                                    observer.OnNext(response);
                                    observer.OnCompleted();
                                }
                                else
                                    observer.OnError(new LoggingException(((CommandResponse) response).Chunk));
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
                    }
                }
                return complete;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                // TODO Observe the oooResponses?

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

                TaskCompletionSource<bool> cts = Interlocked.Exchange(ref _completionTask, null);
                if (cts != null)
                    cts.TrySetCanceled();
            }
        }
    }
}