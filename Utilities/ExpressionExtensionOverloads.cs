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
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    public static partial class ExpressionExtensions
    {
        #region 0 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<TResult>> GetFuncExpression<TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 0);
            
            Expression body = expression;

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<TResult>>(body);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action> GetActionExpression([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 0);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            return Expression.Lambda<Action>(expression);
        }
        #endregion

        #region 1 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, TResult>> GetFuncExpression<T1, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 1);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            Expression body = expression.Inline(i1);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, TResult>>(body, p1);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1>> GetActionExpression<T1>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 1);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            return Expression.Lambda<Action<T1>>(expression.Inline(i1), p1);
        }
        #endregion

        #region 2 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, T2, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, TResult>> GetFuncExpression<T1, T2, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 2);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            Expression body = expression.Inline(i1, i2);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, T2, TResult>>(body, p1, p2);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1, T2}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2>> GetActionExpression<T1, T2>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 2);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            return Expression.Lambda<Action<T1, T2>>(expression.Inline(i1, i2), p1, p2);
        }
        #endregion

        #region 3 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, T2, T3, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, TResult>> GetFuncExpression<T1, T2, T3, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 3);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            Expression body = expression.Inline(i1, i2, i3);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, T2, T3, TResult>>(body, p1, p2, p3);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1, T2, T3}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3>> GetActionExpression<T1, T2, T3>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 3);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            return Expression.Lambda<Action<T1, T2, T3>>(expression.Inline(i1, i2, i3), p1, p2, p3);
        }
        #endregion

        #region 4 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, T2, T3, T4, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, TResult>> GetFuncExpression<T1, T2, T3, T4, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 4);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            Expression body = expression.Inline(i1, i2, i3, i4);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, T2, T3, T4, TResult>>(body, p1, p2, p3, p4);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1, T2, T3, T4}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4>> GetActionExpression<T1, T2, T3, T4>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 4);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            return Expression.Lambda<Action<T1, T2, T3, T4>>(expression.Inline(i1, i2, i3, i4), p1, p2, p3, p4);
        }
        #endregion

        #region 5 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, T2, T3, T4, T5, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 5);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            Expression body = expression.Inline(i1, i2, i3, i4, i5);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, T2, T3, T4, T5, TResult>>(body, p1, p2, p3, p4, p5);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1, T2, T3, T4, T5}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5>> GetActionExpression<T1, T2, T3, T4, T5>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 5);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            return Expression.Lambda<Action<T1, T2, T3, T4, T5>>(expression.Inline(i1, i2, i3, i4, i5), p1, p2, p3, p4, p5);
        }
        #endregion

        #region 6 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, T2, T3, T4, T5, T6, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, T6, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 6);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            Expression body = expression.Inline(i1, i2, i3, i4, i5, i6);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, TResult>>(body, p1, p2, p3, p4, p5, p6);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1, T2, T3, T4, T5, T6}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5, T6>> GetActionExpression<T1, T2, T3, T4, T5, T6>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 6);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            return Expression.Lambda<Action<T1, T2, T3, T4, T5, T6>>(expression.Inline(i1, i2, i3, i4, i5, i6), p1, p2, p3, p4, p5, p6);
        }
        #endregion

        #region 7 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, T2, T3, T4, T5, T6, T7, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, T6, T7, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 7);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            Expression body = expression.Inline(i1, i2, i3, i4, i5, i6, i7);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>(body, p1, p2, p3, p4, p5, p6, p7);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1, T2, T3, T4, T5, T6, T7}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7>> GetActionExpression<T1, T2, T3, T4, T5, T6, T7>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 7);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            return Expression.Lambda<Action<T1, T2, T3, T4, T5, T6, T7>>(expression.Inline(i1, i2, i3, i4, i5, i6, i7), p1, p2, p3, p4, p5, p6, p7);
        }
        #endregion

        #region 8 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, T2, T3, T4, T5, T6, T7, T8, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, T6, T7, T8, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 8);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            Expression body = expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>(body, p1, p2, p3, p4, p5, p6, p7, p8);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1, T2, T3, T4, T5, T6, T7, T8}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8>> GetActionExpression<T1, T2, T3, T4, T5, T6, T7, T8>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 8);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            return Expression.Lambda<Action<T1, T2, T3, T4, T5, T6, T7, T8>>(expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8), p1, p2, p3, p4, p5, p6, p7, p8);
        }
        #endregion

        #region 9 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 9);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            Expression body = expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>>(body, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1, T2, T3, T4, T5, T6, T7, T8, T9}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>> GetActionExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 9);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            return Expression.Lambda<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>>(expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9), p1, p2, p3, p4, p5, p6, p7, p8, p9);
        }
        #endregion

        #region 10 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 10);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            ParameterExpression p10 = Expression.Parameter(typeof(T10));
            Expression i10 = p10.Type != typeof(T10)
                ? Expression.Convert(p10, typeof(T10))
                : (Expression)p10;

            Expression body = expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9, i10);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>>(body, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> GetActionExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 10);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            ParameterExpression p10 = Expression.Parameter(typeof(T10));
            Expression i10 = p10.Type != typeof(T10)
                ? Expression.Convert(p10, typeof(T10))
                : (Expression)p10;

            return Expression.Lambda<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9, i10), p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        }
        #endregion

        #region 11 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 11);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            ParameterExpression p10 = Expression.Parameter(typeof(T10));
            Expression i10 = p10.Type != typeof(T10)
                ? Expression.Convert(p10, typeof(T10))
                : (Expression)p10;

            ParameterExpression p11 = Expression.Parameter(typeof(T11));
            Expression i11 = p11.Type != typeof(T11)
                ? Expression.Convert(p11, typeof(T11))
                : (Expression)p11;

            Expression body = expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>>(body, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> GetActionExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 11);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            ParameterExpression p10 = Expression.Parameter(typeof(T10));
            Expression i10 = p10.Type != typeof(T10)
                ? Expression.Convert(p10, typeof(T10))
                : (Expression)p10;

            ParameterExpression p11 = Expression.Parameter(typeof(T11));
            Expression i11 = p11.Type != typeof(T11)
                ? Expression.Convert(p11, typeof(T11))
                : (Expression)p11;

            return Expression.Lambda<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11), p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        }
        #endregion

        #region 12 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 12);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            ParameterExpression p10 = Expression.Parameter(typeof(T10));
            Expression i10 = p10.Type != typeof(T10)
                ? Expression.Convert(p10, typeof(T10))
                : (Expression)p10;

            ParameterExpression p11 = Expression.Parameter(typeof(T11));
            Expression i11 = p11.Type != typeof(T11)
                ? Expression.Convert(p11, typeof(T11))
                : (Expression)p11;

            ParameterExpression p12 = Expression.Parameter(typeof(T12));
            Expression i12 = p12.Type != typeof(T12)
                ? Expression.Convert(p12, typeof(T12))
                : (Expression)p12;

            Expression body = expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>>(body, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> GetActionExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 12);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            ParameterExpression p10 = Expression.Parameter(typeof(T10));
            Expression i10 = p10.Type != typeof(T10)
                ? Expression.Convert(p10, typeof(T10))
                : (Expression)p10;

            ParameterExpression p11 = Expression.Parameter(typeof(T11));
            Expression i11 = p11.Type != typeof(T11)
                ? Expression.Convert(p11, typeof(T11))
                : (Expression)p11;

            ParameterExpression p12 = Expression.Parameter(typeof(T12));
            Expression i12 = p12.Type != typeof(T12)
                ? Expression.Convert(p12, typeof(T12))
                : (Expression)p12;

            return Expression.Lambda<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12), p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12);
        }
        #endregion

        #region 13 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 13);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            ParameterExpression p10 = Expression.Parameter(typeof(T10));
            Expression i10 = p10.Type != typeof(T10)
                ? Expression.Convert(p10, typeof(T10))
                : (Expression)p10;

            ParameterExpression p11 = Expression.Parameter(typeof(T11));
            Expression i11 = p11.Type != typeof(T11)
                ? Expression.Convert(p11, typeof(T11))
                : (Expression)p11;

            ParameterExpression p12 = Expression.Parameter(typeof(T12));
            Expression i12 = p12.Type != typeof(T12)
                ? Expression.Convert(p12, typeof(T12))
                : (Expression)p12;

            ParameterExpression p13 = Expression.Parameter(typeof(T13));
            Expression i13 = p13.Type != typeof(T13)
                ? Expression.Convert(p13, typeof(T13))
                : (Expression)p13;

            Expression body = expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>>(body, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> GetActionExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 13);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            ParameterExpression p10 = Expression.Parameter(typeof(T10));
            Expression i10 = p10.Type != typeof(T10)
                ? Expression.Convert(p10, typeof(T10))
                : (Expression)p10;

            ParameterExpression p11 = Expression.Parameter(typeof(T11));
            Expression i11 = p11.Type != typeof(T11)
                ? Expression.Convert(p11, typeof(T11))
                : (Expression)p11;

            ParameterExpression p12 = Expression.Parameter(typeof(T12));
            Expression i12 = p12.Type != typeof(T12)
                ? Expression.Convert(p12, typeof(T12))
                : (Expression)p12;

            ParameterExpression p13 = Expression.Parameter(typeof(T13));
            Expression i13 = p13.Type != typeof(T13)
                ? Expression.Convert(p13, typeof(T13))
                : (Expression)p13;

            return Expression.Lambda<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>(expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13), p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13);
        }
        #endregion

        #region 14 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="T14">The type of parameter 14.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 14);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            ParameterExpression p10 = Expression.Parameter(typeof(T10));
            Expression i10 = p10.Type != typeof(T10)
                ? Expression.Convert(p10, typeof(T10))
                : (Expression)p10;

            ParameterExpression p11 = Expression.Parameter(typeof(T11));
            Expression i11 = p11.Type != typeof(T11)
                ? Expression.Convert(p11, typeof(T11))
                : (Expression)p11;

            ParameterExpression p12 = Expression.Parameter(typeof(T12));
            Expression i12 = p12.Type != typeof(T12)
                ? Expression.Convert(p12, typeof(T12))
                : (Expression)p12;

            ParameterExpression p13 = Expression.Parameter(typeof(T13));
            Expression i13 = p13.Type != typeof(T13)
                ? Expression.Convert(p13, typeof(T13))
                : (Expression)p13;

            ParameterExpression p14 = Expression.Parameter(typeof(T14));
            Expression i14 = p14.Type != typeof(T14)
                ? Expression.Convert(p14, typeof(T14))
                : (Expression)p14;

            Expression body = expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>>(body, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="T14">The type of parameter 14.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> GetActionExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 14);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            ParameterExpression p10 = Expression.Parameter(typeof(T10));
            Expression i10 = p10.Type != typeof(T10)
                ? Expression.Convert(p10, typeof(T10))
                : (Expression)p10;

            ParameterExpression p11 = Expression.Parameter(typeof(T11));
            Expression i11 = p11.Type != typeof(T11)
                ? Expression.Convert(p11, typeof(T11))
                : (Expression)p11;

            ParameterExpression p12 = Expression.Parameter(typeof(T12));
            Expression i12 = p12.Type != typeof(T12)
                ? Expression.Convert(p12, typeof(T12))
                : (Expression)p12;

            ParameterExpression p13 = Expression.Parameter(typeof(T13));
            Expression i13 = p13.Type != typeof(T13)
                ? Expression.Convert(p13, typeof(T13))
                : (Expression)p13;

            ParameterExpression p14 = Expression.Parameter(typeof(T14));
            Expression i14 = p14.Type != typeof(T14)
                ? Expression.Convert(p14, typeof(T14))
                : (Expression)p14;

            return Expression.Lambda<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>(expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14), p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14);
        }
        #endregion

        #region 15 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="T14">The type of parameter 14.</typeparam>
        /// <typeparam name="T15">The type of parameter 15.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 15);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            ParameterExpression p10 = Expression.Parameter(typeof(T10));
            Expression i10 = p10.Type != typeof(T10)
                ? Expression.Convert(p10, typeof(T10))
                : (Expression)p10;

            ParameterExpression p11 = Expression.Parameter(typeof(T11));
            Expression i11 = p11.Type != typeof(T11)
                ? Expression.Convert(p11, typeof(T11))
                : (Expression)p11;

            ParameterExpression p12 = Expression.Parameter(typeof(T12));
            Expression i12 = p12.Type != typeof(T12)
                ? Expression.Convert(p12, typeof(T12))
                : (Expression)p12;

            ParameterExpression p13 = Expression.Parameter(typeof(T13));
            Expression i13 = p13.Type != typeof(T13)
                ? Expression.Convert(p13, typeof(T13))
                : (Expression)p13;

            ParameterExpression p14 = Expression.Parameter(typeof(T14));
            Expression i14 = p14.Type != typeof(T14)
                ? Expression.Convert(p14, typeof(T14))
                : (Expression)p14;

            ParameterExpression p15 = Expression.Parameter(typeof(T15));
            Expression i15 = p15.Type != typeof(T15)
                ? Expression.Convert(p15, typeof(T15))
                : (Expression)p15;

            Expression body = expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14, i15);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>>(body, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="T14">The type of parameter 14.</typeparam>
        /// <typeparam name="T15">The type of parameter 15.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> GetActionExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 15);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            ParameterExpression p10 = Expression.Parameter(typeof(T10));
            Expression i10 = p10.Type != typeof(T10)
                ? Expression.Convert(p10, typeof(T10))
                : (Expression)p10;

            ParameterExpression p11 = Expression.Parameter(typeof(T11));
            Expression i11 = p11.Type != typeof(T11)
                ? Expression.Convert(p11, typeof(T11))
                : (Expression)p11;

            ParameterExpression p12 = Expression.Parameter(typeof(T12));
            Expression i12 = p12.Type != typeof(T12)
                ? Expression.Convert(p12, typeof(T12))
                : (Expression)p12;

            ParameterExpression p13 = Expression.Parameter(typeof(T13));
            Expression i13 = p13.Type != typeof(T13)
                ? Expression.Convert(p13, typeof(T13))
                : (Expression)p13;

            ParameterExpression p14 = Expression.Parameter(typeof(T14));
            Expression i14 = p14.Type != typeof(T14)
                ? Expression.Convert(p14, typeof(T14))
                : (Expression)p14;

            ParameterExpression p15 = Expression.Parameter(typeof(T15));
            Expression i15 = p15.Type != typeof(T15)
                ? Expression.Convert(p15, typeof(T15))
                : (Expression)p15;

            return Expression.Lambda<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>(expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14, i15), p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15);
        }
        #endregion

        #region 16 parameters
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="T14">The type of parameter 14.</typeparam>
        /// <typeparam name="T15">The type of parameter 15.</typeparam>
        /// <typeparam name="T16">The type of parameter 16.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 16);
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            ParameterExpression p10 = Expression.Parameter(typeof(T10));
            Expression i10 = p10.Type != typeof(T10)
                ? Expression.Convert(p10, typeof(T10))
                : (Expression)p10;

            ParameterExpression p11 = Expression.Parameter(typeof(T11));
            Expression i11 = p11.Type != typeof(T11)
                ? Expression.Convert(p11, typeof(T11))
                : (Expression)p11;

            ParameterExpression p12 = Expression.Parameter(typeof(T12));
            Expression i12 = p12.Type != typeof(T12)
                ? Expression.Convert(p12, typeof(T12))
                : (Expression)p12;

            ParameterExpression p13 = Expression.Parameter(typeof(T13));
            Expression i13 = p13.Type != typeof(T13)
                ? Expression.Convert(p13, typeof(T13))
                : (Expression)p13;

            ParameterExpression p14 = Expression.Parameter(typeof(T14));
            Expression i14 = p14.Type != typeof(T14)
                ? Expression.Convert(p14, typeof(T14))
                : (Expression)p14;

            ParameterExpression p15 = Expression.Parameter(typeof(T15));
            Expression i15 = p15.Type != typeof(T15)
                ? Expression.Convert(p15, typeof(T15))
                : (Expression)p15;

            ParameterExpression p16 = Expression.Parameter(typeof(T16));
            Expression i16 = p16.Type != typeof(T16)
                ? Expression.Convert(p16, typeof(T16))
                : (Expression)p16;

            Expression body = expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14, i15, i16);

            if (expression.ReturnType != typeof(TResult))
                body = Expression.Convert(body, typeof(TResult));

            return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>>(body, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="T14">The type of parameter 14.</typeparam>
        /// <typeparam name="T15">The type of parameter 15.</typeparam>
        /// <typeparam name="T16">The type of parameter 16.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// <see cref="Expression{Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}}"/>.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> GetActionExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression != null);
            Contract.Requires(expression.Parameters.Count == 16);
            Contract.Requires(expression.ReturnType == typeof(void));
            
            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            ParameterExpression p2 = Expression.Parameter(typeof(T2));
            Expression i2 = p2.Type != typeof(T2)
                ? Expression.Convert(p2, typeof(T2))
                : (Expression)p2;

            ParameterExpression p3 = Expression.Parameter(typeof(T3));
            Expression i3 = p3.Type != typeof(T3)
                ? Expression.Convert(p3, typeof(T3))
                : (Expression)p3;

            ParameterExpression p4 = Expression.Parameter(typeof(T4));
            Expression i4 = p4.Type != typeof(T4)
                ? Expression.Convert(p4, typeof(T4))
                : (Expression)p4;

            ParameterExpression p5 = Expression.Parameter(typeof(T5));
            Expression i5 = p5.Type != typeof(T5)
                ? Expression.Convert(p5, typeof(T5))
                : (Expression)p5;

            ParameterExpression p6 = Expression.Parameter(typeof(T6));
            Expression i6 = p6.Type != typeof(T6)
                ? Expression.Convert(p6, typeof(T6))
                : (Expression)p6;

            ParameterExpression p7 = Expression.Parameter(typeof(T7));
            Expression i7 = p7.Type != typeof(T7)
                ? Expression.Convert(p7, typeof(T7))
                : (Expression)p7;

            ParameterExpression p8 = Expression.Parameter(typeof(T8));
            Expression i8 = p8.Type != typeof(T8)
                ? Expression.Convert(p8, typeof(T8))
                : (Expression)p8;

            ParameterExpression p9 = Expression.Parameter(typeof(T9));
            Expression i9 = p9.Type != typeof(T9)
                ? Expression.Convert(p9, typeof(T9))
                : (Expression)p9;

            ParameterExpression p10 = Expression.Parameter(typeof(T10));
            Expression i10 = p10.Type != typeof(T10)
                ? Expression.Convert(p10, typeof(T10))
                : (Expression)p10;

            ParameterExpression p11 = Expression.Parameter(typeof(T11));
            Expression i11 = p11.Type != typeof(T11)
                ? Expression.Convert(p11, typeof(T11))
                : (Expression)p11;

            ParameterExpression p12 = Expression.Parameter(typeof(T12));
            Expression i12 = p12.Type != typeof(T12)
                ? Expression.Convert(p12, typeof(T12))
                : (Expression)p12;

            ParameterExpression p13 = Expression.Parameter(typeof(T13));
            Expression i13 = p13.Type != typeof(T13)
                ? Expression.Convert(p13, typeof(T13))
                : (Expression)p13;

            ParameterExpression p14 = Expression.Parameter(typeof(T14));
            Expression i14 = p14.Type != typeof(T14)
                ? Expression.Convert(p14, typeof(T14))
                : (Expression)p14;

            ParameterExpression p15 = Expression.Parameter(typeof(T15));
            Expression i15 = p15.Type != typeof(T15)
                ? Expression.Convert(p15, typeof(T15))
                : (Expression)p15;

            ParameterExpression p16 = Expression.Parameter(typeof(T16));
            Expression i16 = p16.Type != typeof(T16)
                ? Expression.Convert(p16, typeof(T16))
                : (Expression)p16;

            return Expression.Lambda<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>(expression.Inline(i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14, i15, i16), p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16);
        }
        #endregion

    }
}