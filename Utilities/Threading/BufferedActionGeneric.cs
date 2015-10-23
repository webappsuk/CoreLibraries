








 
 
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

using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Threading
{


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1>> action,
            long duration)
            : base(args => action(args.Select(a => (T1)a[0])), duration)        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        public void Run(T1 arg1)
        {
            Run(new object[] { arg1 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        public void Run(T1 arg1, T2 arg2)
        {
            Run(new object[] { arg1, arg2 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3)
        {
            Run(new object[] { arg1, arg2, arg3 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Run(new object[] { arg1, arg2, arg3, arg4 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    /// <typeparam name="T21">The type of argument 21.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19], (T21)a[20])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        /// <param name="arg21">Argument 21.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    /// <typeparam name="T21">The type of argument 21.</typeparam>
    /// <typeparam name="T22">The type of argument 22.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19], (T21)a[20], (T22)a[21])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        /// <param name="arg21">Argument 21.</param>
        /// <param name="arg22">Argument 22.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    /// <typeparam name="T21">The type of argument 21.</typeparam>
    /// <typeparam name="T22">The type of argument 22.</typeparam>
    /// <typeparam name="T23">The type of argument 23.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19], (T21)a[20], (T22)a[21], (T23)a[22])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        /// <param name="arg21">Argument 21.</param>
        /// <param name="arg22">Argument 22.</param>
        /// <param name="arg23">Argument 23.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    /// <typeparam name="T21">The type of argument 21.</typeparam>
    /// <typeparam name="T22">The type of argument 22.</typeparam>
    /// <typeparam name="T23">The type of argument 23.</typeparam>
    /// <typeparam name="T24">The type of argument 24.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19], (T21)a[20], (T22)a[21], (T23)a[22], (T24)a[23])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        /// <param name="arg21">Argument 21.</param>
        /// <param name="arg22">Argument 22.</param>
        /// <param name="arg23">Argument 23.</param>
        /// <param name="arg24">Argument 24.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    /// <typeparam name="T21">The type of argument 21.</typeparam>
    /// <typeparam name="T22">The type of argument 22.</typeparam>
    /// <typeparam name="T23">The type of argument 23.</typeparam>
    /// <typeparam name="T24">The type of argument 24.</typeparam>
    /// <typeparam name="T25">The type of argument 25.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19], (T21)a[20], (T22)a[21], (T23)a[22], (T24)a[23], (T25)a[24])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        /// <param name="arg21">Argument 21.</param>
        /// <param name="arg22">Argument 22.</param>
        /// <param name="arg23">Argument 23.</param>
        /// <param name="arg24">Argument 24.</param>
        /// <param name="arg25">Argument 25.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    /// <typeparam name="T21">The type of argument 21.</typeparam>
    /// <typeparam name="T22">The type of argument 22.</typeparam>
    /// <typeparam name="T23">The type of argument 23.</typeparam>
    /// <typeparam name="T24">The type of argument 24.</typeparam>
    /// <typeparam name="T25">The type of argument 25.</typeparam>
    /// <typeparam name="T26">The type of argument 26.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19], (T21)a[20], (T22)a[21], (T23)a[22], (T24)a[23], (T25)a[24], (T26)a[25])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        /// <param name="arg21">Argument 21.</param>
        /// <param name="arg22">Argument 22.</param>
        /// <param name="arg23">Argument 23.</param>
        /// <param name="arg24">Argument 24.</param>
        /// <param name="arg25">Argument 25.</param>
        /// <param name="arg26">Argument 26.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    /// <typeparam name="T21">The type of argument 21.</typeparam>
    /// <typeparam name="T22">The type of argument 22.</typeparam>
    /// <typeparam name="T23">The type of argument 23.</typeparam>
    /// <typeparam name="T24">The type of argument 24.</typeparam>
    /// <typeparam name="T25">The type of argument 25.</typeparam>
    /// <typeparam name="T26">The type of argument 26.</typeparam>
    /// <typeparam name="T27">The type of argument 27.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19], (T21)a[20], (T22)a[21], (T23)a[22], (T24)a[23], (T25)a[24], (T26)a[25], (T27)a[26])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        /// <param name="arg21">Argument 21.</param>
        /// <param name="arg22">Argument 22.</param>
        /// <param name="arg23">Argument 23.</param>
        /// <param name="arg24">Argument 24.</param>
        /// <param name="arg25">Argument 25.</param>
        /// <param name="arg26">Argument 26.</param>
        /// <param name="arg27">Argument 27.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    /// <typeparam name="T21">The type of argument 21.</typeparam>
    /// <typeparam name="T22">The type of argument 22.</typeparam>
    /// <typeparam name="T23">The type of argument 23.</typeparam>
    /// <typeparam name="T24">The type of argument 24.</typeparam>
    /// <typeparam name="T25">The type of argument 25.</typeparam>
    /// <typeparam name="T26">The type of argument 26.</typeparam>
    /// <typeparam name="T27">The type of argument 27.</typeparam>
    /// <typeparam name="T28">The type of argument 28.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19], (T21)a[20], (T22)a[21], (T23)a[22], (T24)a[23], (T25)a[24], (T26)a[25], (T27)a[26], (T28)a[27])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        /// <param name="arg21">Argument 21.</param>
        /// <param name="arg22">Argument 22.</param>
        /// <param name="arg23">Argument 23.</param>
        /// <param name="arg24">Argument 24.</param>
        /// <param name="arg25">Argument 25.</param>
        /// <param name="arg26">Argument 26.</param>
        /// <param name="arg27">Argument 27.</param>
        /// <param name="arg28">Argument 28.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27, arg28 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    /// <typeparam name="T21">The type of argument 21.</typeparam>
    /// <typeparam name="T22">The type of argument 22.</typeparam>
    /// <typeparam name="T23">The type of argument 23.</typeparam>
    /// <typeparam name="T24">The type of argument 24.</typeparam>
    /// <typeparam name="T25">The type of argument 25.</typeparam>
    /// <typeparam name="T26">The type of argument 26.</typeparam>
    /// <typeparam name="T27">The type of argument 27.</typeparam>
    /// <typeparam name="T28">The type of argument 28.</typeparam>
    /// <typeparam name="T29">The type of argument 29.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19], (T21)a[20], (T22)a[21], (T23)a[22], (T24)a[23], (T25)a[24], (T26)a[25], (T27)a[26], (T28)a[27], (T29)a[28])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        /// <param name="arg21">Argument 21.</param>
        /// <param name="arg22">Argument 22.</param>
        /// <param name="arg23">Argument 23.</param>
        /// <param name="arg24">Argument 24.</param>
        /// <param name="arg25">Argument 25.</param>
        /// <param name="arg26">Argument 26.</param>
        /// <param name="arg27">Argument 27.</param>
        /// <param name="arg28">Argument 28.</param>
        /// <param name="arg29">Argument 29.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27, arg28, arg29 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    /// <typeparam name="T21">The type of argument 21.</typeparam>
    /// <typeparam name="T22">The type of argument 22.</typeparam>
    /// <typeparam name="T23">The type of argument 23.</typeparam>
    /// <typeparam name="T24">The type of argument 24.</typeparam>
    /// <typeparam name="T25">The type of argument 25.</typeparam>
    /// <typeparam name="T26">The type of argument 26.</typeparam>
    /// <typeparam name="T27">The type of argument 27.</typeparam>
    /// <typeparam name="T28">The type of argument 28.</typeparam>
    /// <typeparam name="T29">The type of argument 29.</typeparam>
    /// <typeparam name="T30">The type of argument 30.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19], (T21)a[20], (T22)a[21], (T23)a[22], (T24)a[23], (T25)a[24], (T26)a[25], (T27)a[26], (T28)a[27], (T29)a[28], (T30)a[29])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        /// <param name="arg21">Argument 21.</param>
        /// <param name="arg22">Argument 22.</param>
        /// <param name="arg23">Argument 23.</param>
        /// <param name="arg24">Argument 24.</param>
        /// <param name="arg25">Argument 25.</param>
        /// <param name="arg26">Argument 26.</param>
        /// <param name="arg27">Argument 27.</param>
        /// <param name="arg28">Argument 28.</param>
        /// <param name="arg29">Argument 29.</param>
        /// <param name="arg30">Argument 30.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29, T30 arg30)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27, arg28, arg29, arg30 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    /// <typeparam name="T21">The type of argument 21.</typeparam>
    /// <typeparam name="T22">The type of argument 22.</typeparam>
    /// <typeparam name="T23">The type of argument 23.</typeparam>
    /// <typeparam name="T24">The type of argument 24.</typeparam>
    /// <typeparam name="T25">The type of argument 25.</typeparam>
    /// <typeparam name="T26">The type of argument 26.</typeparam>
    /// <typeparam name="T27">The type of argument 27.</typeparam>
    /// <typeparam name="T28">The type of argument 28.</typeparam>
    /// <typeparam name="T29">The type of argument 29.</typeparam>
    /// <typeparam name="T30">The type of argument 30.</typeparam>
    /// <typeparam name="T31">The type of argument 31.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19], (T21)a[20], (T22)a[21], (T23)a[22], (T24)a[23], (T25)a[24], (T26)a[25], (T27)a[26], (T28)a[27], (T29)a[28], (T30)a[29], (T31)a[30])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        /// <param name="arg21">Argument 21.</param>
        /// <param name="arg22">Argument 22.</param>
        /// <param name="arg23">Argument 23.</param>
        /// <param name="arg24">Argument 24.</param>
        /// <param name="arg25">Argument 25.</param>
        /// <param name="arg26">Argument 26.</param>
        /// <param name="arg27">Argument 27.</param>
        /// <param name="arg28">Argument 28.</param>
        /// <param name="arg29">Argument 29.</param>
        /// <param name="arg30">Argument 30.</param>
        /// <param name="arg31">Argument 31.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29, T30 arg30, T31 arg31)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27, arg28, arg29, arg30, arg31 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    /// <typeparam name="T21">The type of argument 21.</typeparam>
    /// <typeparam name="T22">The type of argument 22.</typeparam>
    /// <typeparam name="T23">The type of argument 23.</typeparam>
    /// <typeparam name="T24">The type of argument 24.</typeparam>
    /// <typeparam name="T25">The type of argument 25.</typeparam>
    /// <typeparam name="T26">The type of argument 26.</typeparam>
    /// <typeparam name="T27">The type of argument 27.</typeparam>
    /// <typeparam name="T28">The type of argument 28.</typeparam>
    /// <typeparam name="T29">The type of argument 29.</typeparam>
    /// <typeparam name="T30">The type of argument 30.</typeparam>
    /// <typeparam name="T31">The type of argument 31.</typeparam>
    /// <typeparam name="T32">The type of argument 32.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19], (T21)a[20], (T22)a[21], (T23)a[22], (T24)a[23], (T25)a[24], (T26)a[25], (T27)a[26], (T28)a[27], (T29)a[28], (T30)a[29], (T31)a[30], (T32)a[31])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        /// <param name="arg21">Argument 21.</param>
        /// <param name="arg22">Argument 22.</param>
        /// <param name="arg23">Argument 23.</param>
        /// <param name="arg24">Argument 24.</param>
        /// <param name="arg25">Argument 25.</param>
        /// <param name="arg26">Argument 26.</param>
        /// <param name="arg27">Argument 27.</param>
        /// <param name="arg28">Argument 28.</param>
        /// <param name="arg29">Argument 29.</param>
        /// <param name="arg30">Argument 30.</param>
        /// <param name="arg31">Argument 31.</param>
        /// <param name="arg32">Argument 32.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29, T30 arg30, T31 arg31, T32 arg32)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27, arg28, arg29, arg30, arg31, arg32 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    /// <typeparam name="T21">The type of argument 21.</typeparam>
    /// <typeparam name="T22">The type of argument 22.</typeparam>
    /// <typeparam name="T23">The type of argument 23.</typeparam>
    /// <typeparam name="T24">The type of argument 24.</typeparam>
    /// <typeparam name="T25">The type of argument 25.</typeparam>
    /// <typeparam name="T26">The type of argument 26.</typeparam>
    /// <typeparam name="T27">The type of argument 27.</typeparam>
    /// <typeparam name="T28">The type of argument 28.</typeparam>
    /// <typeparam name="T29">The type of argument 29.</typeparam>
    /// <typeparam name="T30">The type of argument 30.</typeparam>
    /// <typeparam name="T31">The type of argument 31.</typeparam>
    /// <typeparam name="T32">The type of argument 32.</typeparam>
    /// <typeparam name="T33">The type of argument 33.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19], (T21)a[20], (T22)a[21], (T23)a[22], (T24)a[23], (T25)a[24], (T26)a[25], (T27)a[26], (T28)a[27], (T29)a[28], (T30)a[29], (T31)a[30], (T32)a[31], (T33)a[32])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        /// <param name="arg21">Argument 21.</param>
        /// <param name="arg22">Argument 22.</param>
        /// <param name="arg23">Argument 23.</param>
        /// <param name="arg24">Argument 24.</param>
        /// <param name="arg25">Argument 25.</param>
        /// <param name="arg26">Argument 26.</param>
        /// <param name="arg27">Argument 27.</param>
        /// <param name="arg28">Argument 28.</param>
        /// <param name="arg29">Argument 29.</param>
        /// <param name="arg30">Argument 30.</param>
        /// <param name="arg31">Argument 31.</param>
        /// <param name="arg32">Argument 32.</param>
        /// <param name="arg33">Argument 33.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29, T30 arg30, T31 arg31, T32 arg32, T33 arg33)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27, arg28, arg29, arg30, arg31, arg32, arg33 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    /// <typeparam name="T21">The type of argument 21.</typeparam>
    /// <typeparam name="T22">The type of argument 22.</typeparam>
    /// <typeparam name="T23">The type of argument 23.</typeparam>
    /// <typeparam name="T24">The type of argument 24.</typeparam>
    /// <typeparam name="T25">The type of argument 25.</typeparam>
    /// <typeparam name="T26">The type of argument 26.</typeparam>
    /// <typeparam name="T27">The type of argument 27.</typeparam>
    /// <typeparam name="T28">The type of argument 28.</typeparam>
    /// <typeparam name="T29">The type of argument 29.</typeparam>
    /// <typeparam name="T30">The type of argument 30.</typeparam>
    /// <typeparam name="T31">The type of argument 31.</typeparam>
    /// <typeparam name="T32">The type of argument 32.</typeparam>
    /// <typeparam name="T33">The type of argument 33.</typeparam>
    /// <typeparam name="T34">The type of argument 34.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19], (T21)a[20], (T22)a[21], (T23)a[22], (T24)a[23], (T25)a[24], (T26)a[25], (T27)a[26], (T28)a[27], (T29)a[28], (T30)a[29], (T31)a[30], (T32)a[31], (T33)a[32], (T34)a[33])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        /// <param name="arg21">Argument 21.</param>
        /// <param name="arg22">Argument 22.</param>
        /// <param name="arg23">Argument 23.</param>
        /// <param name="arg24">Argument 24.</param>
        /// <param name="arg25">Argument 25.</param>
        /// <param name="arg26">Argument 26.</param>
        /// <param name="arg27">Argument 27.</param>
        /// <param name="arg28">Argument 28.</param>
        /// <param name="arg29">Argument 29.</param>
        /// <param name="arg30">Argument 30.</param>
        /// <param name="arg31">Argument 31.</param>
        /// <param name="arg32">Argument 32.</param>
        /// <param name="arg33">Argument 33.</param>
        /// <param name="arg34">Argument 34.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29, T30 arg30, T31 arg31, T32 arg32, T33 arg33, T34 arg34)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27, arg28, arg29, arg30, arg31, arg32, arg33, arg34 });
        }
    }


    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    /// <typeparam name="T9">The type of argument 9.</typeparam>
    /// <typeparam name="T10">The type of argument 10.</typeparam>
    /// <typeparam name="T11">The type of argument 11.</typeparam>
    /// <typeparam name="T12">The type of argument 12.</typeparam>
    /// <typeparam name="T13">The type of argument 13.</typeparam>
    /// <typeparam name="T14">The type of argument 14.</typeparam>
    /// <typeparam name="T15">The type of argument 15.</typeparam>
    /// <typeparam name="T16">The type of argument 16.</typeparam>
    /// <typeparam name="T17">The type of argument 17.</typeparam>
    /// <typeparam name="T18">The type of argument 18.</typeparam>
    /// <typeparam name="T19">The type of argument 19.</typeparam>
    /// <typeparam name="T20">The type of argument 20.</typeparam>
    /// <typeparam name="T21">The type of argument 21.</typeparam>
    /// <typeparam name="T22">The type of argument 22.</typeparam>
    /// <typeparam name="T23">The type of argument 23.</typeparam>
    /// <typeparam name="T24">The type of argument 24.</typeparam>
    /// <typeparam name="T25">The type of argument 25.</typeparam>
    /// <typeparam name="T26">The type of argument 26.</typeparam>
    /// <typeparam name="T27">The type of argument 27.</typeparam>
    /// <typeparam name="T28">The type of argument 28.</typeparam>
    /// <typeparam name="T29">The type of argument 29.</typeparam>
    /// <typeparam name="T30">The type of argument 30.</typeparam>
    /// <typeparam name="T31">The type of argument 31.</typeparam>
    /// <typeparam name="T32">The type of argument 32.</typeparam>
    /// <typeparam name="T33">The type of argument 33.</typeparam>
    /// <typeparam name="T34">The type of argument 34.</typeparam>
    /// <typeparam name="T35">The type of argument 35.</typeparam>
    [PublicAPI]
    public class BufferedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> : BufferedAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>> action,
            Duration duration)
            : this(action, (long) duration.TotalMilliseconds())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>> action,
            TimeSpan duration)
            : this(action, (long) duration.TotalMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is less than or equal to zero.</exception>
        // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        public BufferedAction(
            [NotNull] Action<IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>> action,
            long duration)
            : base(args => action(args.Select(a => ExtendedTuple.Create((T1)a[0], (T2)a[1], (T3)a[2], (T4)a[3], (T5)a[4], (T6)a[5], (T7)a[6], (T8)a[7], (T9)a[8], (T10)a[9], (T11)a[10], (T12)a[11], (T13)a[12], (T14)a[13], (T15)a[14], (T16)a[15], (T17)a[16], (T18)a[17], (T19)a[18], (T20)a[19], (T21)a[20], (T22)a[21], (T23)a[22], (T24)a[23], (T25)a[24], (T26)a[25], (T27)a[26], (T28)a[27], (T29)a[28], (T30)a[29], (T31)a[30], (T32)a[31], (T33)a[32], (T34)a[33], (T35)a[34])).AsTupleEnumerable()), duration)
        { }
        // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute, EventExceptionNotDocumented
        
        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        /// <param name="arg9">Argument 9.</param>
        /// <param name="arg10">Argument 10.</param>
        /// <param name="arg11">Argument 11.</param>
        /// <param name="arg12">Argument 12.</param>
        /// <param name="arg13">Argument 13.</param>
        /// <param name="arg14">Argument 14.</param>
        /// <param name="arg15">Argument 15.</param>
        /// <param name="arg16">Argument 16.</param>
        /// <param name="arg17">Argument 17.</param>
        /// <param name="arg18">Argument 18.</param>
        /// <param name="arg19">Argument 19.</param>
        /// <param name="arg20">Argument 20.</param>
        /// <param name="arg21">Argument 21.</param>
        /// <param name="arg22">Argument 22.</param>
        /// <param name="arg23">Argument 23.</param>
        /// <param name="arg24">Argument 24.</param>
        /// <param name="arg25">Argument 25.</param>
        /// <param name="arg26">Argument 26.</param>
        /// <param name="arg27">Argument 27.</param>
        /// <param name="arg28">Argument 28.</param>
        /// <param name="arg29">Argument 29.</param>
        /// <param name="arg30">Argument 30.</param>
        /// <param name="arg31">Argument 31.</param>
        /// <param name="arg32">Argument 32.</param>
        /// <param name="arg33">Argument 33.</param>
        /// <param name="arg34">Argument 34.</param>
        /// <param name="arg35">Argument 35.</param>
        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29, T30 arg30, T31 arg31, T32 arg32, T33 arg33, T34 arg34, T35 arg35)
        {
            Run(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27, arg28, arg29, arg30, arg31, arg32, arg33, arg34, arg35 });
        }
    }

}
 
