



 
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
using System.Collections.Generic;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2> : IEnumerable<Tuple<T1, T2>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3> : IEnumerable<Tuple<T1, T2, T3>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4> : IEnumerable<Tuple<T1, T2, T3, T4>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5> : IEnumerable<Tuple<T1, T2, T3, T4, T5>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28, Tuple&lt;T29&gt;&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    /// <typeparam name="T29">The type of item 29.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28, Tuple&lt;T29, T30&gt;&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    /// <typeparam name="T29">The type of item 29.</typeparam>
    /// <typeparam name="T30">The type of item 30.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28, Tuple&lt;T29, T30, T31&gt;&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    /// <typeparam name="T29">The type of item 29.</typeparam>
    /// <typeparam name="T30">The type of item 30.</typeparam>
    /// <typeparam name="T31">The type of item 31.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28, Tuple&lt;T29, T30, T31, T32&gt;&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    /// <typeparam name="T29">The type of item 29.</typeparam>
    /// <typeparam name="T30">The type of item 30.</typeparam>
    /// <typeparam name="T31">The type of item 31.</typeparam>
    /// <typeparam name="T32">The type of item 32.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28, Tuple&lt;T29, T30, T31, T32, T33&gt;&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    /// <typeparam name="T29">The type of item 29.</typeparam>
    /// <typeparam name="T30">The type of item 30.</typeparam>
    /// <typeparam name="T31">The type of item 31.</typeparam>
    /// <typeparam name="T32">The type of item 32.</typeparam>
    /// <typeparam name="T33">The type of item 33.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28, Tuple&lt;T29, T30, T31, T32, T33, T34&gt;&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    /// <typeparam name="T29">The type of item 29.</typeparam>
    /// <typeparam name="T30">The type of item 30.</typeparam>
    /// <typeparam name="T31">The type of item 31.</typeparam>
    /// <typeparam name="T32">The type of item 32.</typeparam>
    /// <typeparam name="T33">The type of item 33.</typeparam>
    /// <typeparam name="T34">The type of item 34.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>>
    {
    }

    /// <summary>
    /// An IEnumerable of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28, Tuple&lt;T29, T30, T31, T32, T33, T34, T35&gt;&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    /// <typeparam name="T29">The type of item 29.</typeparam>
    /// <typeparam name="T30">The type of item 30.</typeparam>
    /// <typeparam name="T31">The type of item 31.</typeparam>
    /// <typeparam name="T32">The type of item 32.</typeparam>
    /// <typeparam name="T33">The type of item 33.</typeparam>
    /// <typeparam name="T34">The type of item 34.</typeparam>
    /// <typeparam name="T35">The type of item 35.</typeparam>
    [PublicAPI]
    public interface IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> : IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>>
    {
    }

}
 
