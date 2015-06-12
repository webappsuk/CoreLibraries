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
using System.Linq.Expressions;
using System.Reflection;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Reflect
{
    /// <summary>
    /// Helper class for getting <see cref="MemberInfo"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <threadsafety static="false" instance="false" />
    [PublicAPI]
    public static class InfoHelper<T>
    {
        /// <summary>
        /// A temporary field that can be used to help get <see cref="MemberInfo"/> for a method with reference or output parameters.
        /// </summary>
        [PublicAPI]
        public static T RefOrOut;

        /// <summary>
        /// A secondary temporary field that can be used to help get a <see cref="ParameterInfo"/> of a reference/output parameter for a method with multiple reference/ouput parameters.
        /// </summary>
        [PublicAPI]
        public static T Parameter;

        /// <summary>
        /// The <see cref="FieldInfo"/> for <see cref="RefOrOut"/>.
        /// </summary>
        [NotNull]
        // ReSharper disable once StaticFieldInGenericType
        internal static readonly FieldInfo RefOrOutFieldInfo;

        /// <summary>
        /// The <see cref="FieldInfo"/> for <see cref="Parameter"/>.
        /// </summary>
        [NotNull]
        // ReSharper disable once StaticFieldInGenericType
        internal static readonly FieldInfo ParameterFieldInfo;

        static InfoHelper()
        {
            RefOrOutFieldInfo = InfoHelper.GetFieldInfo(() => RefOrOut, true);
            ParameterFieldInfo = InfoHelper.GetFieldInfo(() => Parameter, true);
        }
    }

    /// <summary>
    /// Helper class for getting <see cref="MemberInfo"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false" />
    [PublicAPI]
    public static class InfoHelper
    {
        /// <summary>
        /// Gets the method information.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the method was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>
        /// Usage:
        /// <code>InfoHelper.GetMethodInfo(() =&gt; TypeOrInstance.Method(parameters));</code>
        /// </remarks>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static MethodInfo GetMethodInfo([NotNull] Expression<Action> exp, bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            MethodCallExpression body = exp.Body as MethodCallExpression;
            if (body != null) return body.Method;
            if (throwIfNotFound) throw new ArgumentException("MethodInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the method information.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the method was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>InfoHelper.GetMethodInfo&lt;Type&gt;(i => i.Method(parameters));</code>
        /// </remarks>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static MethodInfo GetMethodInfo<TInstance>(
            [NotNull] Expression<Action<TInstance>> exp,
            bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            MethodCallExpression body = exp.Body as MethodCallExpression;
            if (body != null) return body.Method;
            if (throwIfNotFound) throw new ArgumentException("MethodInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the method information.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the method was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>instance.GetMethodInfo(i => i.Method(parameters));</code>
        /// </remarks>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static MethodInfo GetMethodInfo<TInstance>(
            this TInstance instance,
            [NotNull] Expression<Action<TInstance>> exp,
            bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");

            MethodCallExpression body = exp.Body as MethodCallExpression;
            if (body != null) return body.Method;
            if (throwIfNotFound) throw new ArgumentException("MethodInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the method information.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the method was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>InfoHelper.GetMethodInfo(() => TypeOrInstance.Method(parameters));</code>
        /// </remarks>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static MethodInfo GetMethodInfo([NotNull] Expression<Func<object>> exp, bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            Expression body = exp.Body.NodeType == ExpressionType.Convert
                ? ((UnaryExpression)exp.Body).Operand
                : exp.Body;

            MethodCallExpression expression = body as MethodCallExpression;
            if (expression != null) return expression.Method;
            if (throwIfNotFound) throw new ArgumentException("MethodInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the method information.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the method was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>InfoHelper.GetMethodInfo&lt;Type&gt;(i => i.Method(parameters));</code>
        /// </remarks>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static MethodInfo GetMethodInfo<TInstance>(
            [NotNull] Expression<Func<TInstance, object>> exp,
            bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            Expression body = exp.Body.NodeType == ExpressionType.Convert
                ? ((UnaryExpression)exp.Body).Operand
                : exp.Body;

            MethodCallExpression expression = body as MethodCallExpression;
            if (expression != null) return expression.Method;
            if (throwIfNotFound) throw new ArgumentException("MethodInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the method information.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the method was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>instance.GetMethodInfo(i => i.Method(parameters));</code>
        /// </remarks>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static MethodInfo GetMethodInfo<TInstance>(
            this TInstance instance,
            [NotNull] Expression<Func<TInstance, object>> exp,
            bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            Expression body = exp.Body.NodeType == ExpressionType.Convert
                ? ((UnaryExpression)exp.Body).Operand
                : exp.Body;

            MethodCallExpression expression = body as MethodCallExpression;
            if (expression != null) return expression.Method;
            if (throwIfNotFound) throw new ArgumentException("MethodInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the constructor information.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the constructor was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>InfoHelper.GetConstructorInfo(() => new Type(parameter));</code>
        /// </remarks>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static ConstructorInfo GetConstructorInfo<TInstance>(
            [NotNull] Expression<Func<TInstance>> exp,
            bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            NewExpression body = exp.Body as NewExpression;
            if (body != null) return body.Constructor;
            if (throwIfNotFound) throw new ArgumentException("ConstructorInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the property was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>InfoHelper.GetPropertyInfo&lt;Type&gt;(i => i.Property);</code>
        /// </remarks>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static PropertyInfo GetPropertyInfo<TInstance>(
            [NotNull] Expression<Func<TInstance, object>> exp,
            bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            Expression body = exp.Body.NodeType == ExpressionType.Convert
                ? ((UnaryExpression)exp.Body).Operand
                : exp.Body;

            MemberExpression expression = body as MemberExpression;
            if (expression != null) return expression.Member as PropertyInfo;
            if (throwIfNotFound) throw new ArgumentException("PropertyInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the property was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>InfoHelper.GetPropertyInfo(() => TypeOrInstance.Property);</code>
        /// </remarks>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static PropertyInfo GetPropertyInfo<TResult>(
            [NotNull] Expression<Func<TResult>> exp,
            bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            MemberExpression body = exp.Body as MemberExpression;
            if (body != null) return body.Member as PropertyInfo;
            if (throwIfNotFound) throw new ArgumentException("PropertyInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the property was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>InfoHelper.GetPropertyInfo&lt;Type,PropertyType&gt;(i => i.Property);</code>
        /// </remarks>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static PropertyInfo GetPropertyInfo<TInstance, TResult>(
            [NotNull] Expression<Func<TInstance, TResult>> exp,
            bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            MemberExpression body = exp.Body as MemberExpression;
            if (body != null) return body.Member as PropertyInfo;
            if (throwIfNotFound) throw new ArgumentException("PropertyInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the property was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>instance.GetPropertyInfo(i => i.Property);</code>
        /// </remarks>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static PropertyInfo GetPropertyInfo<TInstance, TResult>(
            this TInstance instance,
            [NotNull] Expression<Func<TInstance, TResult>> exp,
            bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            MemberExpression body = exp.Body as MemberExpression;
            if (body != null) return body.Member as PropertyInfo;
            if (throwIfNotFound) throw new ArgumentException("PropertyInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the field was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>InfoHelper.GetFieldInfo&lt;Type&gt;(i => i.Field);</code>
        /// </remarks>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static FieldInfo GetFieldInfo<TInstance>(
            [NotNull] Expression<Func<TInstance, object>> exp,
            bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            Expression memExp = exp.Body.NodeType == ExpressionType.Convert
                ? ((UnaryExpression)exp.Body).Operand
                : exp.Body;

            MemberExpression expression = memExp as MemberExpression;
            if (expression != null) return expression.Member as FieldInfo;
            if (throwIfNotFound) throw new ArgumentException("FieldInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the field was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>InfoHelper.GetFieldInfo(() => TypeOrInstance.Field);</code>
        /// </remarks>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static FieldInfo GetFieldInfo<TResult>(
            [NotNull] Expression<Func<TResult>> exp,
            bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            MemberExpression body = exp.Body as MemberExpression;
            if (body != null) return body.Member as FieldInfo;
            if (throwIfNotFound) throw new ArgumentException("FieldInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the field was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>InfoHelper.GetFieldInfo&lt;Type,FieldType&gt;(i => i.Field);</code>
        /// </remarks>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static FieldInfo GetFieldInfo<TInstance, TResult>(
            [NotNull] Expression<Func<TInstance, TResult>> exp,
            bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            MemberExpression body = exp.Body as MemberExpression;
            if (body != null) return body.Member as FieldInfo;
            if (throwIfNotFound) throw new ArgumentException("FieldInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the field was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>instance.GetFieldInfo(i => i.Field);</code>
        /// </remarks>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static FieldInfo GetFieldInfo<TInstance, TResult>(
            this TInstance instance,
            [NotNull] Expression<Func<TInstance, TResult>> exp,
            bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            MemberExpression body = exp.Body as MemberExpression;
            if (body != null) return body.Member as FieldInfo;
            if (throwIfNotFound) throw new ArgumentException("FieldInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the parameter information for the parameter indicated by the expressions parameter.
        /// </summary>
        /// <typeparam name="TParam">The type of the parameter.</typeparam>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the parameter was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>InfoHelper.GetParameterInfo&lt;ParameterType&gt;(p => TypeOrInstance.Method(..., p, ...));</code>
        /// or
        /// <code>InfoHelper.GetParameterInfo&lt;ParameterType&gt;(p => TypeOrInstance.Method(..., ref InfoHelper&lt;ParameterType&gt;.Parameter, ...));</code>
        /// If both <c>p</c> and <see cref="InfoHelper{T}.Parameter"/> are used, then the parameter <c>p</c> is passed to will be used.
        /// </remarks>
        /// <exception cref="System.ArgumentException">Multiple parameters specified</exception>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static ParameterInfo GetParameterInfo<TParam>(
            [NotNull] Expression<Action<TParam>> exp,
            bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            MethodCallExpression body = exp.Body as MethodCallExpression;
            if (body != null)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                ParameterInfo parameterInfo = GetParameterInfo<TParam>(body, exp.Parameters[0]);
                if (parameterInfo != null) return parameterInfo;
            }

            if (throwIfNotFound) throw new ArgumentException("ParameterInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the parameter information.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <typeparam name="TParam">The type of the parameter.</typeparam>
        /// <param name="exp">The expression.</param>
        /// <param name="throwIfNotFound">if set to <see langword="true" /> and the parameter was not found, an exception will be throw;
        /// otherwise, <see langword="null"/> will be returned.</param>
        /// <returns></returns>
        /// <remarks>Usage:
        /// <code>InfoHelper.GetParameterInfo&lt;Type,ParameterType&gt;(p => TypeOrInstance.Method(..., p, ...));</code>
        /// or
        /// <code>InfoHelper.GetParameterInfo&lt;Type,ParameterType&gt;(p => TypeOrInstance.Method(..., ref InfoHelper&lt;ParameterType&gt;.Parameter, ...));</code>
        /// If both <c>p</c> and <see cref="InfoHelper{T}.Parameter"/> are used, then the parameter <c>p</c> is passed to will be used.
        /// </remarks>
        /// <exception cref="System.ArgumentException">Multiple parameters specified</exception>
        [ContractAnnotation("throwIfNotFound:true => notnull")]
        [PublicAPI]
        public static ParameterInfo GetParameterInfo<TInstance, TParam>(
            [NotNull] Expression<Action<TInstance, TParam>> exp,
            bool throwIfNotFound = false)
        {
            if (exp == null) throw new ArgumentNullException("exp");
            MethodCallExpression body = exp.Body as MethodCallExpression;
            if (body != null)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                ParameterInfo parameterInfo = GetParameterInfo<TParam>(body, exp.Parameters[1]);
                if (parameterInfo != null) return parameterInfo;
            }

            if (throwIfNotFound) throw new ArgumentException("ParameterInfo not found in expression");
            return null;
        }

        /// <summary>
        /// Gets the parameter information.
        /// </summary>
        /// <typeparam name="TParam">The type of the parameter.</typeparam>
        /// <param name="body">The body.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Multiple parameters specified</exception>
        [CanBeNull]
        private static ParameterInfo GetParameterInfo<TParam>(
            [NotNull] MethodCallExpression body,
            [NotNull] ParameterExpression parameter)
        {
            Expression paramArg = null;
            int index = 0;
            bool fromParam = false;
            bool fromTemp = false;
            bool multipleTemp = false;
            bool multipleRefOut = false;

            for (int i = 0; i < body.Arguments.Count; i++)
            {
                Expression arg = body.Arguments[i];

                if (ReferenceEquals(arg, parameter))
                {
                    if (paramArg != null && fromParam)
                        throw new ArgumentException("Multiple parameters specified");

                    paramArg = arg;
                    index = i;
                    fromParam = true;
                    multipleTemp = false;
                }
                else if (!fromParam &&
                         arg is MemberExpression)
                {
                    MemberExpression field = (MemberExpression)arg;

                    if (field.Member == InfoHelper<TParam>.ParameterFieldInfo)
                    {
                        if (paramArg != null && fromTemp)
                            multipleTemp = true;

                        paramArg = arg;
                        index = i;
                        fromTemp = true;
                        multipleRefOut = false;
                    }
                    else if (!fromTemp &&
                             field.Member == InfoHelper<TParam>.RefOrOutFieldInfo)
                    {
                        if (paramArg != null)
                            multipleRefOut = true;

                        paramArg = arg;
                        index = i;
                    }
                }
            }

            if (multipleTemp || multipleRefOut)
                throw new ArgumentException("Multiple parameters specified");

            return paramArg == null
                ? null
                : body.Method.GetParameters()[index];
        }
    }
}