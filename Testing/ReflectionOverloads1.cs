 
 
#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: ReflectionOverloads.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Testing
{
    ///<summary>
    /// Extensions to the reflection namespace.
    ///</summary>
    public static partial class Reflection
    {		
		#region 0 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<TResult> Func<TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<TResult>) GetFunc(methodBase, checkParameterAssignability, typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<TResult> ConstructorFunc<TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action Action(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action) GetAction(methodInfo, checkParameterAssignability);
        }
		#endregion
		
		#region 1 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, TResult> Func<T1, TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, TResult>) GetFunc(methodBase, checkParameterAssignability, typeof(T1), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, TResult> ConstructorFunc<T1, TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof(T1), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action<T1> Action<T1>(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action<T1>) GetAction(methodInfo, checkParameterAssignability, typeof(T1));
        }
		#endregion
		
		#region 2 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, TResult> Func<T1, T2, TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, TResult>) GetFunc(methodBase, checkParameterAssignability, typeof(T1), typeof(T2), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, TResult> ConstructorFunc<T1, T2, TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof(T1), typeof(T2), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action<T1, T2> Action<T1, T2>(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action<T1, T2>) GetAction(methodInfo, checkParameterAssignability, typeof(T1), typeof(T2));
        }
		#endregion
		
		#region 3 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, TResult> Func<T1, T2, T3, TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, TResult>) GetFunc(methodBase, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, TResult> ConstructorFunc<T1, T2, T3, TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action<T1, T2, T3> Action<T1, T2, T3>(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action<T1, T2, T3>) GetAction(methodInfo, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3));
        }
		#endregion
		
		#region 4 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, TResult> Func<T1, T2, T3, T4, TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, TResult>) GetFunc(methodBase, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, TResult> ConstructorFunc<T1, T2, T3, T4, TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action<T1, T2, T3, T4> Action<T1, T2, T3, T4>(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action<T1, T2, T3, T4>) GetAction(methodInfo, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }
		#endregion
		
		#region 5 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, TResult> Func<T1, T2, T3, T4, T5, TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, TResult>) GetFunc(methodBase, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, TResult> ConstructorFunc<T1, T2, T3, T4, T5, TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action<T1, T2, T3, T4, T5> Action<T1, T2, T3, T4, T5>(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action<T1, T2, T3, T4, T5>) GetAction(methodInfo, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        }
		#endregion
		
		#region 6 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, TResult> Func<T1, T2, T3, T4, T5, T6, TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, TResult>) GetFunc(methodBase, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, TResult> ConstructorFunc<T1, T2, T3, T4, T5, T6, TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action<T1, T2, T3, T4, T5, T6> Action<T1, T2, T3, T4, T5, T6>(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action<T1, T2, T3, T4, T5, T6>) GetAction(methodInfo, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        }
		#endregion
		
		#region 7 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> Func<T1, T2, T3, T4, T5, T6, T7, TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, TResult>) GetFunc(methodBase, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> ConstructorFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action<T1, T2, T3, T4, T5, T6, T7> Action<T1, T2, T3, T4, T5, T6, T7>(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7>) GetAction(methodInfo, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
        }
		#endregion
		
		#region 8 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>) GetFunc(methodBase, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> ConstructorFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8> Action<T1, T2, T3, T4, T5, T6, T7, T8>(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8>) GetAction(methodInfo, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
        }
		#endregion
		
		#region 9 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
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
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>) GetFunc(methodBase, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
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
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> ConstructorFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
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
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>) GetAction(methodInfo, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9));
        }
		#endregion
		
		#region 10 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
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
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>) GetFunc(methodBase, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
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
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> ConstructorFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
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
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>) GetAction(methodInfo, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10));
        }
		#endregion
		
		#region 11 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
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
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>) GetFunc(methodBase, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
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
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> ConstructorFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
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
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>) GetAction(methodInfo, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11));
        }
		#endregion
		
		#region 12 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
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
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>) GetFunc(methodBase, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
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
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> ConstructorFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
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
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>) GetAction(methodInfo, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12));
        }
		#endregion
		
		#region 13 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
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
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>) GetFunc(methodBase, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
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
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> ConstructorFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
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
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>) GetAction(methodInfo, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13));
        }
		#endregion
		
		#region 14 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
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
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>) GetFunc(methodBase, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
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
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> ConstructorFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
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
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>) GetAction(methodInfo, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14));
        }
		#endregion
		
		#region 15 parameters.
        /// <summary>
        ///   Gets the lambda functional equivalent of a method base, for much better runtime performance than an invocation.
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
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name = "methodBase">The method base.</param>
        /// <param name = "checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method base.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
            this MethodBase methodBase,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>) GetFunc(methodBase, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a type, for much better runtime performance than an invocation.
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
        /// <typeparam name = "TResult">The type of the result.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a type constructor.
        /// </returns>
		[NotNull]
		[UsedImplicitly]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> ConstructorFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
            this Type type,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>) GetConstructorFunc(type, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof (TResult));
        }
		
        /// <summary>
        ///   Gets the lambda functional equivalent of a method info, for much better runtime performance than an invocation.
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
        /// <param name="methodInfo">The method info.</param>
        /// <param name="checkParameterAssignability">if set to <see langword="true"/> check's function parameters can be assigned to method safely.</param>
        /// <returns>
        ///   A functional equivalent of a method info.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            this MethodInfo methodInfo,
            bool checkParameterAssignability =
#if DEBUG
                true
#else
                false
#endif
            )
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>) GetAction(methodInfo, checkParameterAssignability, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15));
        }
		#endregion
	}
}
 
