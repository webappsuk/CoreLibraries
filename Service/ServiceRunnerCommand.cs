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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Implements a command that can be executed by the <see cref="ServiceRunner"/>.
    /// </summary>
    public class ServiceRunnerCommand
    {
#if DEBUG
        /// <summary>
        /// Grabs the debug view for an expression.
        /// </summary>
        [NotNull]
        private static readonly Func<Expression, string> _expressionDebugView = typeof(Expression).GetProperty(
            "DebugView",
            BindingFlags.NonPublic | BindingFlags.Instance)
            .GetGetMethod(true)
            .Func<Expression, string>();

        /// <summary>
        /// The debug view
        /// </summary>
        [PublicAPI]
        public readonly string DebugView;
#endif

        /// <summary>
        /// The <see cref="MethodInfo"/> for <see cref="string.Split(char[])"/>.
        /// </summary>
        [NotNull]
        private static readonly MethodInfo _stringSplit = typeof(string).GetMethod("Split", new[] { typeof(char[]), typeof(StringSplitOptions) });

        /// <summary>
        /// The <see cref="MethodInfo"/> for <see cref="string.IsNullOrEmpty(string)"/>.
        /// </summary>
        [NotNull]
        private static readonly MethodInfo _stringIsNullOrEmpty = typeof(string).GetMethod(
            "IsNullOrEmpty",
            new[] { typeof(string) });

        /// <summary>
        /// The <see cref="MethodInfo"/> for <see cref="Rebase"/>.
        /// </summary>
        [NotNull]
        private static readonly MethodInfo _rebase = typeof(ServiceRunnerCommand).GetMethod(
            "Rebase",
            BindingFlags.Static | BindingFlags.NonPublic,
            null,
            new[] { typeof(string[]), typeof(int) },
            null);

        [NotNull]
        private static readonly string[] _emptyStringArray = new string[0];

        /// <summary>
        /// The <see cref="MethodInfo"/> for <see cref="Enum.Parse(Type, string, bool)"/>.
        /// </summary>
        [NotNull]
        private static readonly MethodInfo _enumParse = typeof(Enum)
            .GetMethod("Parse", new[] { typeof(Type), typeof(string), typeof(bool) });

        [NotNull]
        private readonly string[] _names;

        [NotNull]
        private readonly Func<object, string, bool> _execute;

        /// <summary>
        /// Gets the primary name.
        /// </summary>
        /// <value>The name.</value>
        [NotNull]
        [PublicAPI]
        public string Name
        {
            get
            {
                string name = _names[0];
                Contract.Assert(name != null);
                return name;
            }
        }

        /// <summary>
        /// The instance type.
        /// </summary>
        [NotNull]
        public readonly Type InstanceType;

        /// <summary>
        /// Gets all names that can be used to run the command.
        /// </summary>
        /// <value>The aliases.</value>
        [NotNull]
        public IEnumerable<string> AllNames
        {
            get { return _names; }
        }

        /// <summary>
        /// The description.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        [NotNull]
        [PublicAPI]
        public IEnumerable<ParameterInfo> Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// The minimum arguments.
        /// </summary>
        [PublicAPI]
        public readonly int MinimumArguments;

        /// <summary>
        /// The maximum arguments.
        /// </summary>
        [PublicAPI]
        public readonly int MaximumArguments;

        [NotNull]
        private readonly string _description;

        [NotNull]
        private readonly IEnumerable<ParameterInfo> _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRunnerCommand"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="attribute">The attribute.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        internal ServiceRunnerCommand([NotNull] MethodInfo method, [NotNull] ServiceRunnerCommandAttribute attribute)
        {
            Contract.Requires<RequiredContractException>(method != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(attribute != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(method.DeclaringType != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(!method.IsGenericMethod, "Method_Generic");

            InstanceType = method.DeclaringType;

            ParameterExpression instanceParameterExpression = Expression.Parameter(typeof(object), "instance");
            ParameterExpression argsParameterExpression = Expression.Parameter(typeof(string), "arguments");

            Expression instance = InstanceType == typeof(object)
                ? (Expression)instanceParameterExpression
                : Expression.Convert(instanceParameterExpression, InstanceType);

            ParameterInfo[] parameters = method.GetParameters();
            _parameters = parameters;
            int minimumArguments;
            int maximumArguments;
            Expression[] inputs = new Expression[parameters.Length];
            List<ParameterExpression> locals = new List<ParameterExpression>();
            List<Expression> body = new List<Expression>();
            ParameterExpression splitArgs = null;

            if (attribute.ConsumeLine)
            {
                if ((parameters.Length != 1) ||
                    (parameters[0] == null) ||
                    (parameters[0].ParameterType != typeof (string)))
                    throw new ServiceException(() => ServiceResources.Err_ServiceCommand_Invalid_Command_Method, method);

                minimumArguments = parameters[0].IsOptional ? 0 : 1;
                maximumArguments = 1;

                // Pass the line straight in.
                inputs[0] = argsParameterExpression;
            }
            else if (parameters.Length < 1)
            {
                minimumArguments = 0;
                maximumArguments = 0;
            }
            else
            {
                minimumArguments =
                    parameters.Select((p, i) => p.IsOptional ? i : int.MaxValue)
                        .Union(new[] { parameters.Length })
                        .Min();

                ParameterInfo lastParam = parameters.Last();
                Contract.Assert(lastParam != null);

                // If the last parameter is a string[], and there are no optional parameters it can accept a parameters collection.
                bool hasParams =
                    (minimumArguments == parameters.Length) &&
                    (lastParam.ParameterType == typeof(string[]));

                if (!hasParams)
                    maximumArguments = parameters.Length;
                else
                {
                    minimumArguments--;
                    if (attribute.MinimumArguments > minimumArguments)
                        minimumArguments = attribute.MinimumArguments;
                    maximumArguments = int.MaxValue;
                }

                // Split the arguments
                splitArgs = Expression.Variable(typeof(string[]), "splitArguments");

                // Assign split arguments to body
                locals.Add(splitArgs);
                body.Add(
                    Expression.Assign(
                        splitArgs,
                        Expression.Call(
                            argsParameterExpression,
                            _stringSplit,
                            Expression.Constant(null, typeof(char[])),
                            Expression.Constant(StringSplitOptions.RemoveEmptyEntries))));

                // Map args to inputs
                for (int p = 0; p < parameters.Length; p++)
                {
                    ParameterInfo parameter = parameters[p];
                    Contract.Assert(parameter != null);

                    Expression argExpression = Expression.ArrayIndex(splitArgs, Expression.Constant(p));
                    Type parameterType = parameter.ParameterType;
                    Expression input;

                    // We can pass strings straight through
                    if (parameterType == typeof(string))
                        input = argExpression;
                    else if (hasParams &&
                             (p == (parameters.Length - 1)) &&
                             (parameterType == typeof(string[])))
                    {
                        // This is the final parameters argument
                        input = splitArgs;
                        if (p > 0)
                            input = Expression.Call(_rebase, input, Expression.Constant(p));
                    }
                    else
                    {
                        // We need to parse the string

                        // Check for nullable
                        bool isNullable = parameterType.IsGenericType &&
                                          parameterType.GetGenericTypeDefinition() == typeof(Nullable<>);

                        if (isNullable)
                            parameterType = parameterType.GetGenericArguments()[0];

                        Contract.Assert(parameterType != null);
                        MethodInfo parseMethod;
                        if (parameterType.IsEnum)
                        {
                            input = Expression.Convert(
                                Expression.Call(
                                    _enumParse,
                                    Expression.Constant(parameterType),
                                    argExpression,
                                    Expression.Constant(true)),
                                parameterType);
                        }
                        else if ((parseMethod = parameterType.GetMethod("Parse", new[] { typeof(string) })) != null)
                        {
                            input = Expression.Call(parseMethod, argExpression);
                        }
                        else if (!argExpression.TryConvert(parameterType, out input))
                            throw new LoggingException(
                                () => ServiceResources.Err_ServiceCommand_Parameter_Conversion_Unsupported,
                                parameterType,
                                parameter.Name,
                                method);

                        // Convert parsed output to nullable type
                        if (isNullable)
                            input = Expression.Convert(input, parameter.ParameterType);
                    }

                    if ((p >= minimumArguments) &&
                        parameter.IsOptional)
                    {
                        // Argument is optional
                        input = Expression.Condition(
                            Expression.LessThan(
                                Expression.ArrayLength(splitArgs),
                                Expression.Constant(p + 1)),
                            Expression.Constant(parameter.DefaultValue, parameter.ParameterType),
                            input);
                    }

                    inputs[p] = input;
                }
            }

            // Call the method passing in the inputs
            Expression methodCall = Expression.Call(instance, method, inputs);

            // If the method doesn't return a bool, return true
            if (method.ReturnType != typeof(bool))
                methodCall = Expression.Block(methodCall, Expression.Constant(true, typeof(bool)));

            // Validate min & max arguments
            if (minimumArguments > 0)
            {
                methodCall = Expression.Condition(
                    splitArgs != null
                        ? (Expression)Expression.LessThan(Expression.ArrayLength(splitArgs), Expression.Constant(minimumArguments))
                        : Expression.Call(_stringIsNullOrEmpty, argsParameterExpression),
                    Expression.Constant(false, typeof(bool)),
                    methodCall);
            }
            if (maximumArguments == 0)
            {
                Contract.Assert(splitArgs == null);
                methodCall = Expression.Condition(
                    Expression.Not(Expression.Call(_stringIsNullOrEmpty, argsParameterExpression)),
                    Expression.Constant(false, typeof(bool)),
                    methodCall);
            }
            else if (!attribute.ConsumeLine &&
                (maximumArguments < int.MaxValue))
            {
                Contract.Assert(splitArgs != null);
                methodCall = Expression.Condition(
                    Expression.GreaterThan(Expression.ArrayLength(splitArgs), Expression.Constant(maximumArguments)),
                    Expression.Constant(false, typeof(bool)),
                    methodCall);
            }
            MinimumArguments = minimumArguments;
            MaximumArguments = maximumArguments;

            body.Add(methodCall);

            // Build 
            Expression<Func<object, string, bool>> lambda = Expression.Lambda<Func<object, string, bool>>(
                body.Blockify(locals),
                instanceParameterExpression,
                argsParameterExpression);

#if DEBUG
            // Update the DebugView
            DebugView = _expressionDebugView(lambda);
#endif

            _execute = lambda.Compile();

            // Get the names
            string methodName = method.Name;
            Translation names = Translation.Get(attribute.ResourceType, attribute.NamesProperty);
            if ((names == null) ||
                string.IsNullOrWhiteSpace(names.Message))
            {
                Log.Add(LoggingLevel.Warning, () => ServiceResources.Wrn_No_Names_For_Command, methodName);
                // Use the method name.
                _names = new[] { methodName };
            }
            else
                _names = names.Message
                    .Split(',')
                    .Select(n => n.Trim())
                    .Where(n => n.Length > 0)
                    .Distinct(StringComparer.CurrentCultureIgnoreCase)
                    .ToArray();

            Contract.Assert(_names.Length > 0);

            // Get the description.
            Translation description = Translation.Get(attribute.ResourceType, attribute.DescriptionProperty);
            if ((description == null) ||
                string.IsNullOrWhiteSpace(description.Message))
            {
                Log.Add(LoggingLevel.Warning, () => ServiceResources.Wrn_No_Description_For_Command, Name);
                // ReSharper disable once AssignNullToNotNullAttribute
                _description = ServiceResources.Cmd_Description;
            }
            else
                _description = description.Message;
        }

        /// <summary>
        /// Runs the command on the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns><see langword="true" /> if succeeded, <see langword="false" /> otherwise.</returns>
        public bool Run([NotNull] object instance, [NotNull] string arguments)
        {
            Contract.Requires<RequiredContractException>(instance != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(arguments != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(InstanceType.IsInstanceOfType(instance), "Bad_Instance");

            return _execute(instance, arguments);
        }

        /// <summary>
        /// Used to rebase an args array.
        /// </summary>  
        /// <param name="args">The arguments.</param>
        /// <param name="skip">The skip.</param>
        /// <returns>System.String[].</returns>
        [NotNull]
        [UsedImplicitly]
        private static string[] Rebase([NotNull] string[] args, int skip)
        {
            Contract.Requires<RequiredContractException>(args != null, "Parameter_Null");

            if (skip < 1)
                return args;
            if (skip >= args.Length)
                return _emptyStringArray;
            
            string[] newArgs = new string[args.Length - skip];
            Array.Copy(args, skip, newArgs, 0, newArgs.Length);
            return newArgs;
        }
    }
}