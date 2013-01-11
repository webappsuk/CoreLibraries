#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Reflect
{
    /// <summary>
    /// Wrap <see cref="FieldInfo"/> with additional information.
    /// </summary>
    /// <remarks></remarks>
    [DebuggerDisplay("{Info} [Extended]")]
    public class Field
    {
        /// <summary>
        /// The extended type.
        /// </summary>
        [NotNull] public readonly ExtendedType ExtendedType;

        /// <summary>
        /// The underlying <see cref="FieldInfo"/>.
        /// </summary>
        [NotNull] public readonly FieldInfo Info;

        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="info">The info.</param>
        /// <remarks></remarks>
        public Field([NotNull] ExtendedType extendedType, [NotNull] FieldInfo info)
        {
            ExtendedType = extendedType;
            Info = info;
        }

        /// <summary>
        /// Gets the field type.
        /// </summary>
        /// <value>The field type.</value>
        /// <remarks></remarks>
        public Type ReturnType
        {
            get { return Info.FieldType; }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Field"/> to <see cref="System.Reflection.FieldInfo"/>.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator FieldInfo(Field field)
        {
            return field == null ? null : field.Info;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Reflection.FieldInfo"/> to <see cref="Field"/>.
        /// </summary>
        /// <param name="fieldInfo">The field info.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Field(FieldInfo fieldInfo)
        {
            return fieldInfo == null
                       ? null
                       : ((ExtendedType) fieldInfo.DeclaringType).GetField(fieldInfo);
        }

        /// <summary>
        /// Retrieves the lambda function equivalent of the specified static getter method.
        /// </summary>
        /// <typeparam name="TValue">The type of the value returned.</typeparam>	
        /// <returns>A function that takes an object of the type T and returns the value of the field.</returns>
        [UsedImplicitly]
        [CanBeNull]
        public Func<TValue> Getter<TValue>()
        {
            // Only valid for static fields.
            if (!Info.IsStatic)
                return null;

            Type fieldType = Info.FieldType;
            Type returnType = typeof (TValue);

            // Check the return type can be assigned from the member type
            if ((returnType != fieldType) &&
                (!returnType.IsAssignableFrom(fieldType)))
                return null;

            // Get a member access expression
            Expression expression = Expression.Field(null, Info);

            Contract.Assert(expression != null);

            // Cast return value if necessary
            if ((returnType != fieldType) &&
                !expression.TryConvert(returnType, out expression))
                return null;

            Contract.Assert(expression != null);

            // Create lambda and compile
            return Expression.Lambda<Func<TValue>>(expression).Compile();
        }
        
        /// <summary>
        /// Retrieves the lambda function equivalent of the specified instance getter method.
        /// </summary>
        /// <typeparam name="T">The type of the parameter the function encapsulates.</typeparam>	
        /// <typeparam name="TValue">The type of the value returned.</typeparam>	
        /// <returns>A function that takes an object of the type T and returns the value of the field.</returns>
        [UsedImplicitly]
        [CanBeNull]
        public Func<T, TValue> Getter<T, TValue>()
        {
            // Only valid for instance fields.
            if (Info.IsStatic)
                return null;

            Type fieldType = Info.FieldType;
            Type declaringType = ExtendedType.Type;
            Type parameterType = typeof (T);
            Type returnType = typeof (TValue);

            // Create input parameter expression
            ParameterExpression parameterExpression = Expression.Parameter(parameterType, "target");

            // Cast parameter if necessary
            Expression expression = parameterExpression;
            if ((parameterType != declaringType) &&
                !expression.TryConvert(declaringType, out expression))
                return null;

            // Get a member access expression
            expression = Expression.Field(expression, Info);

            Contract.Assert(expression != null);
            Contract.Assert(returnType != null);

            // Cast return value if necessary
            if ((returnType != fieldType) &&
                !expression.TryConvert(returnType, out expression))
                return null;

            Contract.Assert(expression != null);
            Contract.Assert(parameterExpression != null);

            // Create lambda and compile
            return Expression.Lambda<Func<T, TValue>>(expression, parameterExpression).Compile();
        }

        /// <summary>
        /// Retrieves the lambda action equivalent of the specified static setter method.
        /// </summary>
        /// <typeparam name="TValue">The type of the value returned.</typeparam>	
        /// <returns>An action that sets the value of the relevant static field.</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        [CanBeNull]
        public Action<TValue> Setter<TValue>()
        {
            // Only valid for static fields.
            if ((!Info.IsStatic) ||
                (Info.IsInitOnly))
                return null;

            Type fieldType = Info.FieldType;
            Type valueType = typeof (TValue);

            // Get a field access expression
            Expression expression = Expression.Field(null, Info);

            // Create value parameter expression
            ParameterExpression valueParameterExpression = Expression.Parameter(
                valueType, "value");
            Expression valueExpression = valueParameterExpression;
            
            // Cast value if necessary
            if ((valueType != fieldType) &&
                !valueExpression.TryConvert(fieldType, out valueExpression)) 
                return null;

            Contract.Assert(expression != null);
            Contract.Assert(valueExpression != null);

            // Create assignment
            expression = Expression.Assign(expression, valueExpression);

            Contract.Assert(expression != null);

            // Create lambda and compile
            return
                Expression.Lambda<Action<TValue>>(expression, valueParameterExpression).Compile();
        }

        /// <summary>
        /// Retrieves the lambda action equivalent of the specified instance setter method.
        /// </summary>
        /// <typeparam name="T">The type of the parameter the function encapsulates.</typeparam>	
        /// <typeparam name="TValue">The type of the value returned.</typeparam>	
        /// <returns>An action that takes an object of the type T and sets the value of the relevant field.</returns>
        [UsedImplicitly]
        [CanBeNull]
        public Action<T, TValue> Setter<T, TValue>()
        {
            // Only valid for instance fields.
            if ((Info.IsStatic) ||
                (Info.IsInitOnly))
                return null;

            Type declaringType = ExtendedType.Type;
            Type parameterType = typeof (T);
            Type fieldType = Info.FieldType;
            Type valueType = typeof (TValue);

            // Create input parameter expression
            ParameterExpression parameterExpression = Expression.Parameter(
                parameterType, "target");

            // Cast parameter if necessary
            Expression expression = parameterExpression;
            if ((parameterType != declaringType) &&
                !expression.TryConvert(declaringType, out expression))
                return null;

            // Get a member access expression
            expression = Expression.Field(expression, Info);

            // Create value parameter expression
            ParameterExpression valueParameterExpression = Expression.Parameter(
                valueType, "value");

            Expression valueExpression = valueParameterExpression;
            if ((valueType != fieldType) &&
                !valueExpression.TryConvert(fieldType, out valueExpression))
                return null;

            Contract.Assert(expression != null);
            Contract.Assert(valueExpression != null);

            // Create assignment
            expression = Expression.Assign(expression, valueExpression);

            Contract.Assert(expression != null);
            Contract.Assert(parameterExpression != null);

            // Create lambda and compile
            return
                Expression.Lambda<Action<T, TValue>>(expression, parameterExpression, valueParameterExpression).Compile();
        }
    }
}