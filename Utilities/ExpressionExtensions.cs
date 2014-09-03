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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    public static class ExpressionExtensions
    {

        /// <summary>
        /// The <see cref="IEnumerator.MoveNext" /> method.
        /// </summary>
        [NotNull]
        private static readonly MethodInfo _enumeratorMoveNextMethod = typeof(IEnumerator).GetMethod(
            "MoveNext",
            BindingFlags.Public | BindingFlags.Instance);

        /// <summary>
        /// Takes an input source enumerable expression (must be of type 
        /// <see cref="IEnumerable{T}" />) and creates a foreach loop,
        /// where the body is generated using the 
        /// <see cref="getBody" /> function.
        /// </summary>
        /// <param name="sourceEnumerable">The source enumerable.</param>
        /// <param name="getBody">The get body function, where the input parameter is the current item in the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <returns>
        /// BlockExpression.
        /// </returns>
        /// <exception cref="System.ArgumentException">The source enumerable is not of an enumerable type;sourceEnumerable</exception>
        [NotNull]
        public static Expression ForEach(
            [NotNull] this Expression sourceEnumerable,
            [NotNull][InstantHandle] Func<Expression, Expression> getBody,
            [CanBeNull] LabelTarget @break = null,
            [CanBeNull] LabelTarget @continue = null)
        {
            return ForEach(sourceEnumerable, item => new[] { getBody(item) }, @break, @continue);
        }

        /// <summary>
        /// Takes an input source enumerable expression (must be of type 
        /// <see cref="IEnumerable{T}" />) and creates a foreach loop,
        /// where the body is generated using the 
        /// <see cref="getBody" /> function.
        /// </summary>
        /// <param name="sourceEnumerable">The source enumerable.</param>
        /// <param name="getBody">The get body function, where the input parameter is the current item in the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <returns>
        /// BlockExpression.
        /// </returns>
        /// <exception cref="System.ArgumentException">The source enumerable is not of an enumerable type;sourceEnumerable</exception>
        [NotNull]
        public static Expression ForEach(
            [NotNull] this Expression sourceEnumerable,
            [NotNull][InstantHandle] Func<Expression, IEnumerable<Expression>> getBody,
            [CanBeNull] LabelTarget @break = null,
            [CanBeNull] LabelTarget @continue = null)
        {
            Contract.Requires(sourceEnumerable != null);

            Type enumerableType = sourceEnumerable.Type;
            Type elementType;
            Type enumeratorType;
            if (enumerableType == typeof(IEnumerable))
            {
                elementType = typeof(object);
                enumeratorType = typeof(IEnumerator);
            }
            else if ((enumerableType.IsGenericType) &&
                     (enumerableType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                elementType = enumerableType.GetGenericArguments().Single();
                enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);
            }
            else
                throw new ArgumentException("The source enumerable is not of an enumerable type", "sourceEnumerable");
            //TODO Translate?

            MethodInfo getEnumeratorMethod = enumerableType.GetMethod(
                "GetEnumerator",
                BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo currentProperty = enumeratorType.GetProperty("Current", elementType);

            ParameterExpression enumerator = Expression.Variable(enumeratorType, "enumerator");
            if (@break == null)
                @break = Expression.Label();

            Expression[] expressions = getBody(Expression.Property(enumerator, currentProperty)).ToArray();
            if (expressions.Length < 1) return Expression.Empty();

            return Expression.Block(
                new[] { enumerator },
                Expression.Assign(enumerator, Expression.Call(sourceEnumerable, getEnumeratorMethod)),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Call(enumerator, _enumeratorMoveNextMethod),
                        expressions.Length > 1 ? Expression.Block(expressions) : expressions.First(),
                        Expression.Break(@break)),
                    @break,
                    @continue));
        }

        /// <summary>
        /// Takes an enumeration of expressions (and optional a set of locals), and returns the most compact single expression.
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        /// <param name="locals">The locals.</param>
        /// <returns>A single expression</returns>
        [NotNull]
        public static Expression Blockify(
            this IEnumerable<Expression> expressions,
            [NotNull] IEnumerable<ParameterExpression> locals)
        {
            Contract.Requires(locals != null);
            return expressions.Blockify(locals.ToArray());
        }

        /// <summary>
        /// Takes an enumeration of expressions (and optional a set of locals), and returns the most compact single expression.
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        /// <param name="locals">The locals.</param>
        /// <returns>A single expression</returns>
        [NotNull]
        public static Expression Blockify(
            this IEnumerable<Expression> expressions,
            params ParameterExpression[] locals)
        {
            Contract.Ensures(Contract.Result<Expression>() != null);

            Expression[] e = (expressions ?? Enumerable.Empty<Expression>()).ToArray();
            if ((locals != null) &&
                (locals.Length > 0))
                return e.Length > 0
                    ? (Expression)Expression.Block(locals, e)
                    : Expression.Empty();
            return e.Length > 1
                ? Expression.Block(e)
                : (e.Length > 0
                    ? e[0]
                    : Expression.Empty());
        }

        /// <summary>
        /// If the expression is a block that has no local variables, then it returns an enumeration of the inner
        /// expressions.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>IEnumerable{Expression}.</returns>
        [NotNull]
        public static IEnumerable<Expression> UnBlockify([NotNull] this Expression expression)
        {
            Contract.Requires(expression != null);
            Contract.Ensures(Contract.Result<IEnumerable<Expression>>() != null);

            BlockExpression block = expression as BlockExpression;
            // Check we have a block.
            if (block == null)
            {
                yield return expression;
                yield break;
            }

            Contract.Assert(block.Variables != null);
            Contract.Assert(block.Expressions != null);
            if (block.Variables.Count > 0)
            {
                // The block has local variables so we can't un-block it.
                yield return block;
                yield break;
            }
            foreach (Expression e in block.Expressions)
                yield return e;
        }

        /// <summary>
        /// If the expression is a block, then it returns an enumeration of the inner expressions and outputs any
        /// variables.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>IEnumerable{Expression}.</returns>
        [NotNull]
        public static IEnumerable<Expression> UnBlockify(
            [NotNull] this Expression expression,
            [NotNull] out IEnumerable<ParameterExpression> variables)
        {
            Contract.Requires(expression != null);
            Contract.Ensures(Contract.ValueAtReturn(out variables) != null);
            Contract.Ensures(Contract.Result<IEnumerable<Expression>>() != null);

            BlockExpression block = expression as BlockExpression;
            if (block == null)
            {
                // We don't have a block.
                variables = Enumerable.Empty<ParameterExpression>();
                return new[] { expression };
            }
            Contract.Assert(block.Variables != null);
            Contract.Assert(block.Expressions != null);
            variables = block.Variables;
            return block.Expressions;
        }

        /// <summary>
        /// Adds the variables to an existing block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="variables">The variables.</param>
        /// <returns>BlockExpression.</returns>
        [NotNull]
        public static Expression AddVariables(
            [NotNull] this Expression block,
            [NotNull] IEnumerable<ParameterExpression> variables)
        {
            Contract.Requires(block != null);
            Contract.Requires(variables != null);
            Contract.Ensures(Contract.Result<Expression>() != null);
            return AddVariables(block, variables.ToArray());
        }

        /// <summary>
        /// Adds the variables to an existing block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="variables">The variables.</param>
        /// <returns>BlockExpression.</returns>
        [NotNull]
        public static Expression AddVariables(
            [NotNull] this Expression block,
            [NotNull] params ParameterExpression[] variables)
        {
            Contract.Requires(block != null);
            Contract.Requires(variables != null);
            Contract.Ensures(Contract.Result<Expression>() != null);

            ParameterExpression[] v = variables.ToArray();
            if (v.Length < 1)
                return block;

            BlockExpression b = block as BlockExpression;
            return b == null
                ? Expression.Block(variables, block)
                : Expression.Block(b.Variables.Concat(v), b.Expressions);
        }

        /// <summary>
        /// Adds the expressions to an existing block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="expressions">The expressions.</param>
        /// <returns>BlockExpression.</returns>
        [NotNull]
        public static Expression AddExpressions(
            [NotNull] this Expression block,
            [NotNull] IEnumerable<Expression> expressions)
        {
            Contract.Requires(block != null);
            Contract.Requires(expressions != null);
            Contract.Ensures(Contract.Result<Expression>() != null);
            return block.AddExpressions(expressions.ToArray());
        }

        /// <summary>
        /// Adds the expressions to an existing block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="expressions">The expressions.</param>
        /// <returns>BlockExpression.</returns>
        [NotNull]
        public static Expression AddExpressions(
            [NotNull] this Expression block,
            [NotNull] params Expression[] expressions)
        {
            Contract.Requires(block != null);
            Contract.Requires(expressions != null);
            Contract.Ensures(Contract.Result<Expression>() != null);

            if (expressions.Length < 1)
                return block;

            BlockExpression b = block as BlockExpression;
            return b == null
                ? Expression.Block(new[] { block }.Concat(expressions))
                : Expression.Block(b.Variables, b.Expressions.Concat(expressions));
        }

        #region GetFuncExpression overloads
        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>Expression{Func{``0}}.</returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<TResult>> GetFuncExpression<TResult>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression.Parameters.Count == 0);
            if (expression.ReturnType == typeof(TResult))
                return (Expression<Func<TResult>>)expression;
            return Expression.Lambda<Func<TResult>>(
                Expression.Convert(expression.Body, typeof(TResult)),
                expression.Parameters);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>Expression{Func{``0``1}}.</returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, TResult>> GetFuncExpression<T1, TResult>([NotNull] this LambdaExpression expression)
        {
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
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// Expression{Func{``0``1``2}}.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, TResult>> GetFuncExpression<T1, T2, TResult>([NotNull] this LambdaExpression expression)
        {
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
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// Expression{Func{``0``1``2``3}}.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, TResult>> GetFuncExpression<T1, T2, T3, TResult>([NotNull] this LambdaExpression expression)
        {
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
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// Expression{Func{``0``1``2``3``4}}.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, TResult>> GetFuncExpression<T1, T2, T3, T4, TResult>([NotNull] this LambdaExpression expression)
        {
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
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// Expression{Func{``0``1``2``3``4``5}}.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, TResult>([NotNull] this LambdaExpression expression)
        {
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
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// Expression{Func{``0``1``2``3``4``5``6}}.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, T6, TResult>([NotNull] this LambdaExpression expression)
        {
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
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <typeparam name="T7">The type of the t7.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// Expression{Func{``0``1``2``3``4``5``6``7}}.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, T6, T7, TResult>([NotNull] this LambdaExpression expression)
        {
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
        /// Gets the lambda expression as a strongly typed function.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <typeparam name="T7">The type of the t7.</typeparam>
        /// <typeparam name="T8">The type of the t8.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>
        /// Expression{Func{``0``1``2``3``4``5``6``7``8}}.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> GetFuncExpression<T1, T2, T3, T4, T5, T6, T7, T8, TResult>([NotNull] this LambdaExpression expression)
        {
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
        #endregion

        #region GetActionExpression overloads
        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <param name="expression">The lambda expression.</param>
        [NotNull]
        [PublicAPI]
        public static Expression<Action> GetActionExpression([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression.Parameters.Count == 0);
            Contract.Requires(expression.ReturnType == typeof(void));

            return (Expression<Action>)expression;
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1>> GetActionExpression<T1>([NotNull] this LambdaExpression expression)
        {
            Contract.Requires(expression.Parameters.Count == 1);
            Contract.Requires(expression.ReturnType == typeof(void));

            ParameterExpression p1 = Expression.Parameter(typeof(T1));
            Expression i1 = p1.Type != typeof(T1)
                ? Expression.Convert(p1, typeof(T1))
                : (Expression)p1;

            return Expression.Lambda<Action<T1>>(expression.Inline(i1), p1);
        }

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2>> GetActionExpression<T1, T2>([NotNull] this LambdaExpression expression)
        {
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

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3>> GetActionExpression<T1, T2, T3>([NotNull] this LambdaExpression expression)
        {
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

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4>> GetActionExpression<T1, T2, T3, T4>([NotNull] this LambdaExpression expression)
        {
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

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5>> GetActionExpression<T1, T2, T3, T4, T5>([NotNull] this LambdaExpression expression)
        {
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

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5, T6>> GetActionExpression<T1, T2, T3, T4, T5, T6>([NotNull] this LambdaExpression expression)
        {
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

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <typeparam name="T7">The type of the t7.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7>> GetActionExpression<T1, T2, T3, T4, T5, T6, T7>([NotNull] this LambdaExpression expression)
        {
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

        /// <summary>
        /// Gets the lambda expression as a strongly typed action.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <typeparam name="T3">The type of the t3.</typeparam>
        /// <typeparam name="T4">The type of the t4.</typeparam>
        /// <typeparam name="T5">The type of the t5.</typeparam>
        /// <typeparam name="T6">The type of the t6.</typeparam>
        /// <typeparam name="T7">The type of the t7.</typeparam>
        /// <typeparam name="T8">The type of the t8.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        [NotNull]
        [PublicAPI]
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8>> GetActionExpression<T1, T2, T3, T4, T5, T6, T7, T8>([NotNull] this LambdaExpression expression)
        {
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

        /// <summary>
        /// Takes a lambda expression and returns new expression where the parameters have been replaced by the specified
        /// expressions, effectively inlining the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The replacement parameters.</param>
        /// <returns>Expression.</returns>
        [NotNull]
        [PublicAPI]
        public static Expression Inline(
            [NotNull] this LambdaExpression expression,
            [NotNull] params Expression[] parameters)
        {
            return new ParameterReplacerVisitor(expression, parameters).Visit();
        }

        /// <summary>
        /// Replaces the parameters of a lambda expression.
        /// </summary>
        private class ParameterReplacerVisitor : ExpressionVisitor
        {
            /// <summary>
            /// The lambda
            /// </summary>
            [NotNull]
            private readonly LambdaExpression _lambda;

            /// <summary>
            /// The replacements
            /// </summary>
            [NotNull]
            private readonly IReadOnlyDictionary<ParameterExpression, Expression> _replacements;

            /// <summary>
            /// Initializes a new instance of the <see cref="ParameterReplacerVisitor" /> class.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <param name="parameters">The parameters.</param>
            public ParameterReplacerVisitor(
                [NotNull] LambdaExpression expression,
                [NotNull] params Expression[] parameters)
            {
                Contract.Requires(expression != null, "Parameter_Null");
                Contract.Requires(parameters != null, "Parameter_Null");
                _lambda = expression;
                int pcount = _lambda.Parameters.Count;

                if (pcount != parameters.Length)
                {
                    throw new ArgumentOutOfRangeException(
                        "parameters",
                        string.Format(
                            "The number of parameter replacement expressions '{0}' does not match the number of parameters in the lambda expression '{1}'.",
                        //TODO Translate?
                            parameters.Length,
                            pcount));
                }

                Dictionary<ParameterExpression, Expression> replacements =
                    new Dictionary<ParameterExpression, Expression>(pcount);
                for (int i = 0; i < pcount; i++)
                {
                    ParameterExpression originalParameter = _lambda.Parameters[i];
                    Expression newParameter = parameters[i];
                    // ReSharper disable PossibleNullReferenceException
                    if (originalParameter.Type != newParameter.Type)
                        // ReSharper restore PossibleNullReferenceException
                        newParameter = newParameter.Convert(originalParameter.Type);

                    replacements.Add(originalParameter, newParameter);
                }
                _replacements = replacements;
            }

            /// <summary>
            /// Visits this instance.
            /// </summary>
            /// <returns>Expression.</returns>
            [NotNull]
            internal Expression Visit()
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                return Visit(_lambda.Body);
            }

            /// <summary>
            /// Visits the <see cref="T:System.Linq.Expressions.ParameterExpression" />.
            /// </summary>
            /// <param name="node">The expression to visit.</param>
            /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
            protected override Expression VisitParameter(ParameterExpression node)
            {
                Expression replacement;
                // ReSharper disable once AssignNullToNotNullAttribute
                return _replacements.TryGetValue(node, out replacement) ? replacement : node;
            }
        }
    }
}