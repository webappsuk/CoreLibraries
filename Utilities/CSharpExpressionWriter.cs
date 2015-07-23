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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Writes an <see cref="Expression"/> tree to C# like code.
    /// </summary>
    [PublicAPI]
    public class CSharpExpressionWriter : ExpressionVisitor
    {
        private const int Tab = 4;
        private const char TabChar = ' ';

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the <paramref name="expression"/> given.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="fullTypeNames">if set to <see langword="true" /> type names will be fully qualified.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents the <paramref name="expression"/>.
        /// </returns>
        [NotNull]
        public static string ToString([NotNull] Expression expression, bool fullTypeNames = false)
        {
            PreVisitor preVisitor = new PreVisitor(fullTypeNames);
            preVisitor.Visit(expression);
            CSharpExpressionWriter visitor = new CSharpExpressionWriter(preVisitor, expression is LambdaExpression);
            visitor.Visit(expression);
            return visitor.ToString();
        }

        [NotNull]
        private StringBuilder _builder = new StringBuilder();

        [NotNull]
        private readonly Dictionary<LambdaExpression, string> _lambdaDefinitions =
            new Dictionary<LambdaExpression, string>();

        private bool _lambdaIsRoot;

        [NotNull]
        private readonly PreVisitor _preVisitor;

        private int _indent;
        private bool _newLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpExpressionWriter"/> class.
        /// </summary>
        /// <param name="preVisitor">The pre visitor.</param>
        /// <param name="rootIsLambda">if set to <see langword="true" /> [root is lambda].</param>
        private CSharpExpressionWriter([NotNull] PreVisitor preVisitor, bool rootIsLambda)
        {
            _preVisitor = preVisitor;
            _lambdaIsRoot = rootIsLambda;
        }

        /// <summary>
        /// Visitor that gets the names of all the labels, parameters and lambdas, as well as all the types used by an expression.
        /// </summary>
        private class PreVisitor : ExpressionVisitor
        {
            [NotNull]
            private static readonly IReadOnlyDictionary<Type, string> _keywordTypes = new Dictionary<Type, string>
            {
                { typeof(object), "object" },
                { typeof(byte), "byte" },
                { typeof(sbyte), "sbyte" },
                { typeof(short), "short" },
                { typeof(ushort), "ushort" },
                { typeof(int), "int" },
                { typeof(uint), "uint" },
                { typeof(long), "long" },
                { typeof(ulong), "ulong" },
                { typeof(double), "double" },
                { typeof(float), "float" },
                { typeof(decimal), "decimal" },
                { typeof(string), "string" },
                { typeof(char), "char" },
                { typeof(bool), "bool" },
                { typeof(void), "void" },
            };

            [NotNull]
            private readonly Dictionary<LabelTarget, string> _labelNames =
                new Dictionary<LabelTarget, string>(ReferenceComparer<LabelTarget>.Default);

            [NotNull]
            private readonly Dictionary<LambdaExpression, string> _lambdaNames =
                new Dictionary<LambdaExpression, string>(ReferenceComparer<LambdaExpression>.Default);

            [NotNull]
            private readonly Dictionary<ParameterExpression, string> _parameterNames =
                new Dictionary<ParameterExpression, string>(ReferenceComparer<ParameterExpression>.Default);

            [NotNull]
            private readonly DependencyGraph<Type> _types = new DependencyGraph<Type>();

            [NotNull]
            private readonly Lazy<Dictionary<Type, string>> _typeNames;

            private int _labelId = 1;
            private int _lambdaId = 1;
            private int _paramId = 1;

            /// <summary>
            /// Whether namespace qualified names will be used instead of shortened ones.
            /// </summary>
            public readonly bool FullTypeNames;

            /// <summary>
            /// Initializes a new instance of the <see cref="PreVisitor"/> class.
            /// </summary>
            /// <param name="fullTypeNames">if set to <see langword="true" /> [full type names].</param>
            public PreVisitor(bool fullTypeNames)
            {
                _typeNames = new Lazy<Dictionary<Type, string>>(GetTypeNames);
                _types.Add(typeof(Math));
                FullTypeNames = fullTypeNames;
            }

            /// <summary>
            /// Adds the type to the collection.
            /// </summary>
            /// <param name="type">The type.</param>
            private void AddType([NotNull] Type type)
            {
                AddTypes(type);
            }

            /// <summary>
            /// Adds the types to the collection.
            /// </summary>
            /// <param name="types">The types.</param>
            private void AddTypes([NotNull] params Type[] types)
            {
                Stack<Type> stack = new Stack<Type>(types);

                Type type;
                while (stack.TryPop(out type))
                {
                    Debug.Assert(type != null);

                    _types.Add(type);

                    if (type.IsGenericTypeDefinition)
                        continue;

                    // ReSharper disable once ConstantNullCoalescingCondition - This can actually return null for certain types.
                    Type[] typeArgs = type.GetGenericArguments() ?? Array<Type>.Empty;
                    foreach (Type t in typeArgs)
                    {
                        Debug.Assert(t != null);

                        stack.Push(t);
                        _types.Add(type, t);
                    }
                }
            }

            /// <summary>
            /// Gets the type names.
            /// </summary>
            /// <returns></returns>
            [NotNull]
            private Dictionary<Type, string> GetTypeNames()
            {
                Dictionary<string, Type> uniqueTypes =
                    new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

                Lookup<string, Type> ambiguousTypes =
                    new Lookup<string, Type>(0, StringComparer.InvariantCultureIgnoreCase);

                foreach (Type type in _types.AllTopDown)
                {
                    Debug.Assert(type != null);

                    string name;
                    if (FullTypeNames || !_keywordTypes.TryGetValue(type, out name))
                        name = type.FullName;
                    Debug.Assert(name != null);

                    uniqueTypes[name] = type;

                    if (!FullTypeNames)
                    {
                        // Calculate each part of the path up to the name
                        string part = name;
                        int index;
                        while ((index = part.IndexOf('.')) > 0 &&
                               index < unchecked((uint)part.IndexOf('`')))
                        {
                            part = part.Substring(index + 1);
                            ambiguousTypes.Add(part, type);
                        }
                    }
                }

                // Each part of the component paths that only appeared once can be added to the unique components
                foreach (IGrouping<string, Type> group in ambiguousTypes)
                {
                    Debug.Assert(group != null);
                    Debug.Assert(group.Key != null);

                    if (!uniqueTypes.ContainsKey(group.Key) &&
                        group.HasExact(1))
                        // ReSharper disable once AssignNullToNotNullAttribute
                        uniqueTypes[group.Key] = group.First();
                }

                Dictionary<Type, string> dict = uniqueTypes.GroupBy(kvp => kvp.Value, kvp => kvp.Key)
                    // ReSharper disable PossibleNullReferenceException
                    .ToDictionary(g => g.Key, g => g.MinBy(p => p.Split('.').Length).Replace('+', '.'));
                // ReSharper restore PossibleNullReferenceException

                // ReSharper disable once PossibleNullReferenceException
                foreach (Type type in _types.AllTopDown.Where(t => t.IsGenericType))
                {
                    string name = dict[type];
                    Debug.Assert(name != null);

                    if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        // ReSharper disable once AssignNullToNotNullAttribute
                        name = dict[type.GetGenericArguments()[0]] + "?";
                    else
                    {
                        int genIndex = name.IndexOf('`');
                        if (genIndex > 0)
                            name = name.Substring(0, genIndex);

                        IEnumerable<string> args = type.GetGenericArguments()
                            // ReSharper disable PossibleNullReferenceException
                            .Select(t => t.IsGenericParameter ? string.Empty : dict[t]);
                        // ReSharper restore PossibleNullReferenceException

                        name = name + "<" + string.Join(", ", args) + ">";
                    }

                    dict[type] = name;
                }

                return dict;
            }

            /// <summary>
            /// Dispatches the expression to one of the more specialized visit methods in this class.
            /// </summary>
            /// <param name="node">The expression to visit.</param>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            [CanBeNull]
            public override Expression Visit([CanBeNull] Expression node)
            {
                if (node != null)
                    AddType(node.Type);
                return base.Visit(node);
            }

            /// <summary>
            /// Visits the <see cref="T:System.Linq.Expressions.ConstantExpression" />.
            /// </summary>
            /// <param name="node">The expression to visit.</param>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            protected override Expression VisitConstant(ConstantExpression node)
            {
                Type type = node.Value as Type;
                if (type != null)
                    AddType(type);

                MethodInfo method = node.Value as MethodInfo;
                if (method == null)
                {
                    Delegate @delegate = node.Value as Delegate;
                    if (@delegate != null)
                    {
                        if (@delegate.Target != null)
                            AddType(@delegate.Target.GetType());

                        method = @delegate.Method;
                    }
                }

                if (method != null)
                {
                    if (method.DeclaringType != null) // ReSharper disable once AssignNullToNotNullAttribute
                        AddType(method.DeclaringType);

                    AddTypes(method.GetGenericArguments());
                }

                LambdaExpression lambda = node.Value as LambdaExpression;
                if (lambda != null)
                    Visit(lambda);

                return base.VisitConstant(node);
            }

            /// <summary>
            /// Visits the <see cref="T:System.Linq.Expressions.LabelTarget" />.
            /// </summary>
            /// <param name="node">The expression to visit.</param>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            [CanBeNull]
            protected override LabelTarget VisitLabelTarget([CanBeNull] LabelTarget node)
            {
                if (node != null)
                {
                    if (node.Type != null)
                        AddType(node.Type);
                    if (!string.IsNullOrWhiteSpace(node.Name))
                        _labelNames[node] = node.Name;
                }

                return base.VisitLabelTarget(node);
            }

            /// <summary>
            /// Visits the children of the <see cref="T:System.Linq.Expressions.Expression`1" />.
            /// </summary>
            /// <typeparam name="T">The type of the delegate.</typeparam>
            /// <param name="node">The expression to visit.</param>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                if (node.ReturnType != null)
                    AddType(node.ReturnType);
                if (!string.IsNullOrWhiteSpace(node.Name))
                    _lambdaNames[node] = node.Name;

                return base.VisitLambda(node);
            }

            /// <summary>
            /// Visits the <see cref="T:System.Linq.Expressions.ParameterExpression" />.
            /// </summary>
            /// <param name="node">The expression to visit.</param>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (!string.IsNullOrWhiteSpace(node.Name))
                    _parameterNames[node] = node.Name;

                return base.VisitParameter(node);
            }

            /// <summary>
            /// Visits the children of the <see cref="T:System.Linq.Expressions.DynamicExpression" />.
            /// </summary>
            /// <param name="node">The expression to visit.</param>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            protected override Expression VisitDynamic(DynamicExpression node)
            {
                if (node.DelegateType != null)
                    AddType(node.DelegateType);

                ConvertBinder convert = node.Binder as ConvertBinder;
                if (convert != null)
                {
                    Debug.Assert(convert.Type != null);
                    AddType(convert.Type);
                }

                return base.VisitDynamic(node);
            }

            /// <summary>
            /// Visits the children of the <see cref="T:System.Linq.Expressions.TypeBinaryExpression" />.
            /// </summary>
            /// <param name="node">The expression to visit.</param>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            protected override Expression VisitTypeBinary(TypeBinaryExpression node)
            {
                AddType(node.TypeOperand);
                return base.VisitTypeBinary(node);
            }

            /// <summary>
            /// Visits the children of the <see cref="T:System.Linq.Expressions.CatchBlock" />.
            /// </summary>
            /// <param name="node">The expression to visit.</param>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                if (node.Test != null)
                    AddType(node.Test);
                return base.VisitCatchBlock(node);
            }

            /// <summary>
            /// Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression" />.
            /// </summary>
            /// <param name="node">The expression to visit.</param>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Object == null) // ReSharper disable once AssignNullToNotNullAttribute
                    AddType(node.Method.DeclaringType);

                AddTypes(node.Method.GetGenericArguments());

                return base.VisitMethodCall(node);
            }

            /// <summary>
            /// Visits the children of the <see cref="T:System.Linq.Expressions.NewArrayExpression" />.
            /// </summary>
            /// <param name="node">The expression to visit.</param>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            protected override Expression VisitNewArray(NewArrayExpression node)
            {
                Type elementType = node.Type.GetElementType();

                Debug.Assert(elementType != null);

                AddType(elementType);
                AddTypes(elementType.GetGenericArguments());

                return base.VisitNewArray(node);
            }

            /// <summary>
            /// Visits the children of the <see cref="T:System.Linq.Expressions.NewExpression" />.
            /// </summary>
            /// <param name="node">The expression to visit.</param>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            protected override Expression VisitNew(NewExpression node)
            {
                AddTypes(node.Type.GetGenericArguments());

                return base.VisitNew(node);
            }

            /// <summary>
            /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberExpression" />.
            /// </summary>
            /// <param name="node">The expression to visit.</param>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression == null) // ReSharper disable once AssignNullToNotNullAttribute
                    AddType(node.Member.DeclaringType);

                return base.VisitMember(node);
            }

            /// <summary>
            /// Gets the name of the label.
            /// </summary>
            /// <param name="label">The label.</param>
            /// <returns></returns>
            [NotNull]
            public string GetLabelName([NotNull] LabelTarget label)
            {
                string name;
                if (_labelNames.TryGetValue(label, out name))
                {
                    Debug.Assert(name != null);
                    return name;
                }

                while (_labelNames.ContainsValue(name = "label" + _labelId++))
                {
                }
                _labelNames[label] = name;
                return name;
            }

            /// <summary>
            /// Gets the name of the lambda.
            /// </summary>
            /// <param name="lambda">The lambda.</param>
            /// <returns></returns>
            [NotNull]
            public string GetLambdaName([NotNull] LambdaExpression lambda)
            {
                string name;
                if (_lambdaNames.TryGetValue(lambda, out name))
                {
                    Debug.Assert(name != null);
                    return name;
                }

                while (_lambdaNames.ContainsValue(name = "Lambda" + _lambdaId++))
                {
                }
                _lambdaNames[lambda] = name;
                return name;
            }

            /// <summary>
            /// Gets the name of the parameter.
            /// </summary>
            /// <param name="parameter">The parameter.</param>
            /// <returns></returns>
            [NotNull]
            public string GetParameterName([NotNull] ParameterExpression parameter)
            {
                string name;
                if (_parameterNames.TryGetValue(parameter, out name))
                {
                    Debug.Assert(name != null);
                    return name;
                }

                while (_parameterNames.ContainsValue(name = "var" + _paramId++))
                {
                }
                _parameterNames[parameter] = name;
                return name;
            }

            /// <summary>
            /// Gets the name of the type.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns></returns>
            [NotNull]
            public string GetTypeName([NotNull] Type type)
            {
                string name;
                Debug.Assert(_typeNames.Value != null);
                if (_typeNames.Value.TryGetValue(type, out name))
                {
                    Debug.Assert(name != null);
                    return name;
                }

                if (type.IsGenericType)
                {
                    name = type.FullName;
                    // ReSharper disable once PossibleNullReferenceException
                    name = name.Substring(0, name.IndexOf('`'));
                    return name + "<" + string.Join(", ", type.GetGenericArguments().Select(GetTypeName)) + ">";
                }

                // ReSharper disable once AssignNullToNotNullAttribute
                return type.FullName;
            }
        }

        /// <summary>
        /// Gets the name of the label.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns></returns>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetLabelName([NotNull] LabelTarget label)
        {
            return _preVisitor.GetLabelName(label);
        }

        /// <summary>
        /// Gets the name of the lambda.
        /// </summary>
        /// <param name="lambda">The lambda.</param>
        /// <returns></returns>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetLambdaName([NotNull] LambdaExpression lambda)
        {
            return _preVisitor.GetLambdaName(lambda);
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetParameterName([NotNull] ParameterExpression parameter)
        {
            return _preVisitor.GetParameterName(parameter);
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetTypeName([NotNull] Type type)
        {
            return _preVisitor.GetTypeName(type);
        }

        /// <summary>
        /// Indents the output.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Indent()
        {
            _indent += Tab;
        }

        /// <summary>
        /// Deindents this output.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once IdentifierTypo
        private void Deindent()
        {
            _indent -= Tab;
        }

        /// <summary>
        /// Outputs a string to the string builder.
        /// </summary>
        /// <param name="s">The s.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Out([NotNull] string s)
        {
            if (_newLine)
                _builder.Append(TabChar, _indent);
            _newLine = false;
            _builder.Append(s);
        }

        /// <summary>
        /// Outputs a string followed by a new line to the string builder.
        /// </summary>
        /// <param name="s">The string.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OutLine([NotNull] string s)
        {
            if (_newLine)
                _builder.Append(TabChar, _indent);
            _builder.AppendLine(s);
            _newLine = true;
        }

        /// <summary>
        /// Outputs a new line to the string builder.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void NewLine()
        {
            _builder.AppendLine();
            _newLine = true;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.BinaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.ArrayIndex)
            {
                Visit(node.Left);
                Out("[");
                Visit(node.Right);
                Out("]");
            }
            else if (node.NodeType == ExpressionType.Power)
            {
                Out(GetTypeName(typeof(Math)) + ".Pow(");
                Visit(node.Left);
                Out(", ");
                Visit(node.Right);
                Out(")");
            }
            else if (node.NodeType == ExpressionType.PowerAssign)
            {
                Visit(node.Left);
                Out(" = " + GetTypeName(typeof(Math)) + ".Pow(");
                Visit(node.Left);
                Out(", ");
                Visit(node.Right);
                Out(")");
            }
            else
            {
                bool bracketLeft = NeedsParentheses(node, node.Left);
                bool bracketRight = NeedsParentheses(node, node.Right);

                string op;
                bool isChecked = false;
                switch (node.NodeType)
                {
                    case ExpressionType.Add:
                        op = "+";
                        break;
                    case ExpressionType.AddChecked:
                        op = "+";
                        isChecked = true;
                        break;
                    case ExpressionType.And:
                        op = "&";
                        break;
                    case ExpressionType.AndAlso:
                        op = "&&";
                        break;
                    case ExpressionType.Coalesce:
                        op = "??";
                        break;
                    case ExpressionType.Divide:
                        op = "/";
                        break;
                    case ExpressionType.Equal:
                        op = "==";
                        break;
                    case ExpressionType.ExclusiveOr:
                        op = "^";
                        break;
                    case ExpressionType.GreaterThan:
                        op = ">";
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        op = ">=";
                        break;
                    case ExpressionType.LeftShift:
                        op = "<<";
                        break;
                    case ExpressionType.LessThan:
                        op = "<";
                        break;
                    case ExpressionType.LessThanOrEqual:
                        op = "<=";
                        break;
                    case ExpressionType.Modulo:
                        op = "%";
                        break;
                    case ExpressionType.Multiply:
                        op = "*";
                        break;
                    case ExpressionType.MultiplyChecked:
                        op = "*";
                        isChecked = true;
                        break;
                    case ExpressionType.NotEqual:
                        op = "!=";
                        break;
                    case ExpressionType.Or:
                        op = "|";
                        break;
                    case ExpressionType.OrElse:
                        op = "||";
                        break;
                    case ExpressionType.RightShift:
                        op = ">>";
                        break;
                    case ExpressionType.Subtract:
                        op = "-";
                        break;
                    case ExpressionType.SubtractChecked:
                        op = "-";
                        isChecked = true;
                        break;
                    case ExpressionType.TypeAs:
                        op = "as";
                        break;
                    case ExpressionType.TypeIs:
                        op = "is";
                        break;
                    case ExpressionType.Assign:
                        op = "=";
                        break;
                    case ExpressionType.AddAssign:
                        op = "+=";
                        break;
                    case ExpressionType.AndAssign:
                        op = "&=";
                        break;
                    case ExpressionType.DivideAssign:
                        op = "/=";
                        break;
                    case ExpressionType.ExclusiveOrAssign:
                        op = "^=";
                        break;
                    case ExpressionType.LeftShiftAssign:
                        op = "<<=";
                        break;
                    case ExpressionType.ModuloAssign:
                        op = "%=";
                        break;
                    case ExpressionType.MultiplyAssign:
                        op = "*=";
                        break;
                    case ExpressionType.OrAssign:
                        op = "|=";
                        break;
                    case ExpressionType.RightShiftAssign:
                        op = ">>=";
                        break;
                    case ExpressionType.SubtractAssign:
                        op = "-=";
                        break;
                    case ExpressionType.AddAssignChecked:
                        op = "+=";
                        isChecked = true;
                        break;
                    case ExpressionType.MultiplyAssignChecked:
                        op = "*=";
                        isChecked = true;
                        break;
                    case ExpressionType.SubtractAssignChecked:
                        op = "-=";
                        isChecked = true;
                        break;
                    default:
                        op = node.NodeType.ToString();
                        break;
                }

                if (isChecked)
                    Out("checked");

                if (bracketLeft)
                    Out("(");
                Visit(node.Left);
                if (bracketLeft)
                    Out(")");

                Out(" " + op + " ");

                if (bracketRight)
                    Out("(");
                Visit(node.Right);
                if (bracketRight)
                    Out(")");
            }
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.BlockExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        [NotNull]
        protected override Expression VisitBlock(BlockExpression node)
        {
            OutLine("{");
            Indent();

            // ReSharper disable once PossibleNullReferenceException
            foreach (ParameterExpression variable in node.Variables)
            {
                Debug.Assert(variable != null);
                OutLine(GetTypeName(variable.Type) + " " + GetParameterName(variable) + ";");
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            VisitStatements(node.Expressions);

            Deindent();
            Out("}");

            return node;
        }

        /// <summary>
        /// Visits the statements of a block.
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        private void VisitStatements([NotNull] ReadOnlyCollection<Expression> expressions)
        {
            Visit(expressions, VisitStatement);
        }

        /// <summary>
        /// Visits a statement inside a block.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        [NotNull]
        private Expression VisitStatement([NotNull] Expression node)
        {
            if (node.NodeType == ExpressionType.Default &&
                node.Type == typeof(void))
            {
                NewLine();
                return node;
            }

            Visit(node);

            try
            {
                if (node is BlockExpression ||
                    node is SwitchExpression ||
                    node is TryExpression ||
                    node is LabelExpression ||
                    node is LoopExpression ||
                    (node is ConditionalExpression && node.Type == typeof(void)))
                    return node;

                Out(";");
                return node;
            }
            finally
            {
                NewLine();
            }
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.ConditionalExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            if (node.Type == typeof(void))
            {
                Out("if (");
                Visit(node.Test);
                OutLine(")");
                if (node.IfTrue is BlockExpression)
                    Visit(node.IfTrue);
                else
                {
                    OutLine("{");
                    Indent();
                    VisitStatement(node.IfTrue);
                    Deindent();
                    Out("}");
                }

                if (!(node.IfFalse is DefaultExpression) ||
                    node.IfFalse.Type != typeof(void))
                {
                    NewLine();
                    if (node.IfFalse is BlockExpression)
                    {
                        OutLine("else");
                        Visit(node.IfFalse);
                    }
                    else if (node.IfFalse is ConditionalExpression &&
                             node.IfFalse.Type == typeof(void))
                    {
                        Out("else ");
                        VisitStatement(node.IfFalse);
                    }
                    else
                    {
                        OutLine("else");
                        OutLine("{");
                        Indent();
                        VisitStatement(node.IfFalse);
                        Deindent();
                        Out("}");
                    }
                }
            }
            else
            {
                Out("(");
                Visit(node.Test);
                NewLine();
                Indent();
                Out("? ");
                Visit(node.IfTrue);
                NewLine();
                Out(": ");
                Visit(node.IfFalse);
                Out(")");
                Deindent();
            }

            return node;
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.ConstantExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            object value = node.Value;
            Out(value == null ? "null" : ConstantString(value));

            return node;
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.DebugInfoExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            Debug.Assert(node.Document != null);

            Out(
                string.Format(
                    "/* {0}: [{1}, {2}] - [{3}, {4}] */",
                    node.Document.FileName,
                    node.StartLine,
                    node.StartColumn,
                    node.EndLine,
                    node.EndColumn)
                );
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.DynamicExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        [NotNull]
        protected override Expression VisitDynamic(DynamicExpression node)
        {
            ConvertBinder convert;
            GetMemberBinder getMember;
            SetMemberBinder setMember;
            DeleteMemberBinder deleteMember;
            InvokeMemberBinder call;
            UnaryOperationBinder unary;
            BinaryOperationBinder binary;

            Out("/* dynamic */ ");

            CallSiteBinder binder = node.Binder;

            Debug.Assert(node.Arguments != null);
            Debug.Assert(binder != null);

            if ((convert = binder as ConvertBinder) != null)
            {
                Debug.Assert(convert.Type != null);

                Out("((" + GetTypeName(convert.Type) + ")");
                Visit(node.Arguments[0]);
                Out(")");
            }
            else if ((getMember = binder as GetMemberBinder) != null)
            {
                Debug.Assert(getMember.Name != null);

                Visit(node.Arguments[0]);
                Out(".");
                Out(getMember.Name);
            }
            else if ((setMember = binder as SetMemberBinder) != null)
            {
                Debug.Assert(setMember.Name != null);

                Visit(node.Arguments[0]);
                Out(".");
                Out(setMember.Name);
                Out(" = ");
                Visit(node.Arguments[1]);
                Out(")");

            }
            else if ((deleteMember = binder as DeleteMemberBinder) != null)
            {
                Debug.Assert(deleteMember.Name != null);

                Out("(delete ");
                Visit(node.Arguments[0]);
                Out(".");
                Out(deleteMember.Name);
                Out(")");
            }
            else if ((binder as GetIndexBinder) != null)
            {
                Visit(node.Arguments[0]);
                VisitMethod(
                    string.Empty,
                    Array<Type>.Empty,
                    node.Arguments.Skip(1).ToArray(),
                    "[",
                    "]",
                    Array<ParameterInfo>.Empty);
            }
            else if ((binder as SetIndexBinder) != null)
            {
                Out("(");
                Visit(node.Arguments[0]);
                VisitMethod(
                    string.Empty,
                    Array<Type>.Empty,
                    node.Arguments.Skip(1).Take(node.Arguments.Count - 2).ToArray(),
                    "[",
                    "]",
                    Array<ParameterInfo>.Empty);
                Out(" = ");
                Visit(node.Arguments[node.Arguments.Count - 1]);
                Out(")");
            }
            else if ((binder as DeleteIndexBinder) != null)
            {
                Out("(delete ");
                Visit(node.Arguments[0]); 
                VisitMethod(
                     string.Empty,
                     Array<Type>.Empty,
                     node.Arguments.Skip(1).ToArray(),
                     "[",
                     "]",
                     Array<ParameterInfo>.Empty);
                Out(")");
            }
            else if ((call = binder as InvokeMemberBinder) != null)
            {
                Debug.Assert(call.Name != null);

                Visit(node.Arguments[0]);
                Out(".");
                VisitMethod(
                     call.Name,
                     Array<Type>.Empty,
                     node.Arguments.Skip(1).ToArray(),
                     "(",
                     ")",
                     Array<ParameterInfo>.Empty);
            }
            else if ((binder as InvokeBinder) != null)
            {
                Visit(node.Arguments[0]);
                VisitMethod(
                     String.Empty, 
                     Array<Type>.Empty,
                     node.Arguments.Skip(1).ToArray(),
                     "(",
                     ")",
                     Array<ParameterInfo>.Empty);
            }
            else if ((binder as CreateInstanceBinder) != null)
            {
                Out("new ");
                Visit(node.Arguments[0]);
                VisitMethod(
                     String.Empty,
                     Array<Type>.Empty,
                     node.Arguments.Skip(1).ToArray(),
                     "(",
                     ")",
                     Array<ParameterInfo>.Empty);
            }
            else if ((unary = binder as UnaryOperationBinder) != null)
            {
                // ReSharper disable AssignNullToNotNullAttribute
                UnaryExpression unaryExp = Expression.MakeUnary(unary.Operation, node.Arguments[0], null);
                // ReSharper restore AssignNullToNotNullAttribute

                VisitUnary(unaryExp);
            }
            else if ((binary = binder as BinaryOperationBinder) != null)
            {
                // ReSharper disable AssignNullToNotNullAttribute
                BinaryExpression binaryExp = Expression.MakeBinary(binary.Operation, node.Arguments[0], node.Arguments[1]);
                // ReSharper restore AssignNullToNotNullAttribute

                VisitBinary(binaryExp);
            }
            else
            {
                Out(binder.ToString());
                Visit(node.Arguments);
            }

            return node;
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.DefaultExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitDefault(DefaultExpression node)
        {
            if (node.Type != typeof(void))
                Out(string.Format("default({0})", GetTypeName(node.Type)));
            return node;
        }

        /// <summary>
        /// Visits the children of the extension expression.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        [NotNull]
        protected override Expression VisitExtension(Expression node)
        {
            if (node.CanReduce)
                Visit(node.Reduce());
            else
                throw new InvalidOperationException();

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.GotoExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        /// <exception cref="System.NotSupportedException"></exception>
        protected override Expression VisitGoto(GotoExpression node)
        {
            switch (node.Kind)
            {
                case GotoExpressionKind.Goto:
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Out("goto " + GetLabelName(node.Target));
                    break;
                case GotoExpressionKind.Return:
                    Out("return");
                    break;
                case GotoExpressionKind.Break:
                    Out("break");
                    break;
                case GotoExpressionKind.Continue:
                    Out("continue");
                    break;
                default:
                    throw new NotSupportedException();
            }
            if (node.Value != null)
            {
                Out(" ");
                Visit(node.Value);
            }

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.InvocationExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        [NotNull]
        protected override Expression VisitInvocation([NotNull] InvocationExpression node)
        {
            Visit(node.Expression);

            ParameterInfo[] parameters = Array<ParameterInfo>.Empty;
            MethodInfo invokeMethod = node.Type.GetMethod("Invoke");
            if (invokeMethod != null)
                parameters = invokeMethod.GetParameters();

            VisitMethod(".Invoke", Array<Type>.Empty, node.Arguments, "(", ")", parameters);

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.LabelExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitLabel(LabelExpression node)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Out(GetLabelName(node.Target));

            if (node.Type != typeof(void))
            {
                Out(" ");
                Visit(node.DefaultValue);
            }

            Out(":");
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.Expression`1" />.
        /// </summary>
        /// <typeparam name="T">The type of the delegate.</typeparam>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            // If the lambda does not have a name, is not a block, and all the parameters are not byref, output the lambda inline
            if (node.Name == null &&
                !(node.Body is BlockExpression) &&
                // ReSharper disable once PossibleNullReferenceException
                node.Parameters.All(p => !p.IsByRef))
            {
                if (node.Parameters.Count == 1)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Out(GetParameterName(node.Parameters.First()));
                }
                else
                {
                    Out("(");
                    bool first = true;
                    foreach (ParameterExpression parameter in node.Parameters)
                    {
                        Debug.Assert(parameter != null);
                        if (first) first = false;
                        else Out(", ");

                        Out(GetParameterName(parameter));
                    }
                    Out(")");
                }

                Out(" => ");

                Visit(node.Body);

                return node;
            }

            bool isRoot = _lambdaIsRoot;
            StringBuilder builder = _builder;
            int indent = _indent;
            _lambdaIsRoot = false;

            string name = GetLambdaName(node);

            if (!_lambdaDefinitions.ContainsKey(node))
            {
                if (!isRoot)
                {
                    Out(name);

                    _builder = new StringBuilder();
                    _indent = 0;
                }

                // ReSharper disable once AssignNullToNotNullAttribute
                Out(GetTypeName(node.ReturnType) + " " + name);

                Out("(");
                for (int index = 0; index < node.Parameters.Count; index++)
                {
                    if (index > 0) Out(", ");

                    ParameterExpression parameter = node.Parameters[index];
                    Debug.Assert(parameter != null);
                    if (parameter.Type.IsByRef)
                        Out("ref ");
                    Out(GetTypeName(parameter.Type) + " " + GetParameterName(parameter));
                }
                OutLine(")");

                if (node.Body is BlockExpression)
                    Visit(node.Body);
                else
                {
                    OutLine("{");
                    Indent();
                    VisitStatement(node.Body);
                    Deindent();
                    OutLine("}");
                }

                if (!isRoot)
                {
                    _lambdaDefinitions.Add(node, _builder.ToString());

                    _builder = builder;
                    _indent = indent;
                    _newLine = false;
                }
            }
            else if (!isRoot)
                Out(name);

            _lambdaIsRoot = isRoot;

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.LoopExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitLoop(LoopExpression node)
        {
            Out("loop");
            if (node.ContinueLabel != null)
                Out(" " + GetLabelName(node.ContinueLabel) + ":");
            NewLine();

            if (node.Body is BlockExpression)
                Visit(node.Body);
            else
            {
                OutLine("{");
                Indent();
                // ReSharper disable once AssignNullToNotNullAttribute
                VisitStatement(node.Body);
                Deindent();
                Out("}");
            }

            if (node.BreakLabel != null)
                Out(" " + GetLabelName(node.BreakLabel) + ":");

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null)
                Visit(node.Expression);
            else // ReSharper disable once AssignNullToNotNullAttribute
                Out(GetTypeName(node.Member.DeclaringType));
            Out("." + node.Member.Name);

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.IndexExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        [NotNull]
        protected override Expression VisitIndex([NotNull] IndexExpression node)
        {
            Visit(node.Object);
            Out("[");

            bool first = true;
            // ReSharper disable once PossibleNullReferenceException
            foreach (Expression index in node.Arguments)
            {
                if (first) first = false;
                else Out(", ");

                Visit(index);
            }

            Out("]");

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        [NotNull]
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!_preVisitor.FullTypeNames &&
                node.Object == null &&
                node.Method.GetCustomAttribute<ExtensionAttribute>() != null)
            {

                Visit(node.Arguments[0]);
                Out(".");
                VisitMethod(
                    node.Method.Name,
                    node.Method.GetGenericArguments(),
                    node.Arguments.Skip(1).ToArray(),
                    "(",
                    ")",
                    node.Method.GetParameters().Skip(1).ToArray());

                return node;
            }

            if (node.Object == null) // ReSharper disable once AssignNullToNotNullAttribute
                Out(GetTypeName(node.Method.DeclaringType));
            else
                Visit(node.Object);

            Out(".");
            VisitMethod(
                node.Method.Name,
                node.Method.GetGenericArguments(),
                node.Arguments,
                "(",
                ")",
                node.Method.GetParameters());

            return node;
        }

        /// <summary>
        /// Visits the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="typeArgs">The type arguments.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="openBracket">The open bracket.</param>
        /// <param name="closeBracket">The close bracket.</param>
        /// <param name="parameters">The parameters.</param>
        private void VisitMethod(
            [NotNull] string name,
            [NotNull] [ItemNotNull] IReadOnlyCollection<Type> typeArgs,
            [NotNull] IReadOnlyList<Expression> args,
            [NotNull] string openBracket,
            [NotNull] string closeBracket,
            [NotNull] IReadOnlyList<ParameterInfo> parameters)
        {
            Out(name);

            bool first;
            if (typeArgs.Count > 0)
            {
                Out("<");

                first = true;
                foreach (Type type in typeArgs)
                {
                    if (first) first = false;
                    else Out(", ");

                    Out(GetTypeName(type));
                }

                Out(">");
            }

            Out(openBracket);

            first = true;
            for (int i = 0; i < args.Count; i++)
            {
                if (first) first = false;
                else Out(", ");

                if (i < parameters.Count)
                {
                    ParameterInfo parameter = parameters[i];
                    Debug.Assert(parameter != null);
                    if (parameter.IsOut)
                        Out("out ");
                    else if (parameter.ParameterType.IsByRef)
                        Out("ref ");
                    else if (i == args.Count - 1 &&
                             parameter.ParameterType.IsArray &&
                             parameter.GetCustomAttribute<ParamArrayAttribute>() != null)
                    {
                        NewArrayExpression arrayExp = args[i] as NewArrayExpression;
                        ConstantExpression constExp;
                        if (arrayExp != null)
                        {
                            if (arrayExp.NodeType == ExpressionType.NewArrayInit)
                            {
                                args = arrayExp.Expressions;
                                parameters = Array<ParameterInfo>.Empty;
                                first = true;
                                i = -1;
                                continue;
                            }
                        }
                        else if ((constExp = args[i] as ConstantExpression) != null && constExp.Value != null)
                        {
                            Array array = (Array)constExp.Value;
                            List<Expression> arrayExps = new List<Expression>();
                            for (int j = array.GetLowerBound(0); j <= array.GetUpperBound(0); j++)
                                arrayExps.Add(Expression.Constant(array.GetValue(j)));

                            args = arrayExps;
                            parameters = Array<ParameterInfo>.Empty;
                            first = true;
                            i = -1;
                            continue;
                        }
                    }
                }

                Visit(args[i]);
            }

            Out(closeBracket);
        }
        
        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.NewArrayExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            Out("new ");
            Type elementType = node.Type.GetElementType();
            Debug.Assert(elementType != null);

            if (node.NodeType == ExpressionType.NewArrayBounds)
            {
                VisitMethod(
                    GetTypeName(elementType),
                    Array<Type>.Empty,
                    node.Expressions,
                    "[",
                    "]",
                    Array<ParameterInfo>.Empty);
            }
            else
            {
                Debug.Assert(node.NodeType == ExpressionType.NewArrayInit);
                VisitMethod(
                    GetTypeName(elementType) + "[]",
                    Array<Type>.Empty,
                    node.Expressions,
                    "{ ",
                    " }",
                    Array<ParameterInfo>.Empty);
            }

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.NewExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitNew(NewExpression node)
        {
            Out("new ");

            VisitMethod(
                GetTypeName(node.Type),
                Array<Type>.Empty,
                node.Arguments,
                "(",
                ")",
                node.Constructor.GetParameters());

            return node;
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.ParameterExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            Out(GetParameterName(node));

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.RuntimeVariablesExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            // TODO Implement properly
            Out(node.GetDebugView());
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.SwitchCase" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            Visit(
                // ReSharper disable once AssignNullToNotNullAttribute
                node.TestValues,
                e =>
                {
                    Out("case ");
                    Visit(e);
                    OutLine(":");
                    return e;
                });

            Indent();

            bool needsBreak = true;

            BlockExpression block = node.Body as BlockExpression;
            if (block != null)
            {
                // ReSharper disable once PossibleNullReferenceException
                if (block.Variables.Count < 1)
                    // ReSharper disable once AssignNullToNotNullAttribute
                    VisitStatements(block.Expressions);
                else
                    Visit(node.Body);

                // ReSharper disable once AssignNullToNotNullAttribute
                if (block.Expressions.Last() is GotoExpression)
                    needsBreak = false;
            }
            else
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                VisitStatement(node.Body);
                if (node.Body is GotoExpression)
                    needsBreak = false;
            }

            if (needsBreak)
                OutLine("break;");
            Deindent();

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.SwitchExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitSwitch(SwitchExpression node)
        {
            Out("switch (");
            Visit(node.SwitchValue);
            OutLine(")");
            OutLine("{");
            Indent();

            // ReSharper disable once AssignNullToNotNullAttribute
            Visit(node.Cases, VisitSwitchCase);

            if (node.DefaultBody != null)
            {
                OutLine("default:");
                Indent();

                bool needsBreak = true;

                BlockExpression block = node.DefaultBody as BlockExpression;
                if (block != null)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    if (block.Variables.Count < 1)
                        // ReSharper disable once AssignNullToNotNullAttribute
                        VisitStatements(block.Expressions);
                    else
                        Visit(block);

                    // ReSharper disable once AssignNullToNotNullAttribute
                    if (block.Expressions.Last() is GotoExpression)
                        needsBreak = false;
                }
                else
                {
                    VisitStatement(node.DefaultBody);
                    if (node.DefaultBody is GotoExpression)
                        needsBreak = false;
                }

                if (needsBreak)
                    OutLine("break;");
                Deindent();
            }

            Deindent();
            Out("}");

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.TryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitTry(TryExpression node)
        {
            OutLine("try");

            if (node.Body is BlockExpression)
                Visit(node.Body);
            else
            {
                OutLine("{");
                Indent();
                // ReSharper disable once AssignNullToNotNullAttribute
                VisitStatement(node.Body);
                Deindent();
                OutLine("}");
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            Visit(node.Handlers, VisitCatchBlock);

            if (node.Fault != null)
            {
                OutLine("fault");
                if (node.Fault is BlockExpression)
                    Visit(node.Fault);
                else
                {
                    OutLine("{");
                    Indent();
                    VisitStatement(node.Fault);
                    Deindent();
                    OutLine("}");
                }
            }
            if (node.Finally != null)
            {
                OutLine("finally");
                if (node.Finally is BlockExpression)
                    Visit(node.Finally);
                else
                {
                    OutLine("{");
                    Indent();
                    VisitStatement(node.Finally);
                    Deindent();
                    OutLine("}");
                }
            }

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.CatchBlock" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            Out("catch");

            if (node.Test != null)
            {
                Out("(");
                Out(GetTypeName(node.Test));

                if (node.Variable != null)
                {
                    Out(" ");
                    Out(GetParameterName(node.Variable));
                }

                Out(")");
            }
            NewLine();

            if (node.Filter != null)
            {
                Indent();
                Out("if (");
                Visit(node.Filter);
                OutLine(")");
                Deindent();
            }

            if (node.Body is BlockExpression)
                Visit(node.Body);
            else
            {
                OutLine("{");
                Indent();
                // ReSharper disable once AssignNullToNotNullAttribute
                VisitStatement(node.Body);
                Deindent();
                OutLine("}");
            }

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.TypeBinaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            if (node.NodeType == ExpressionType.TypeIs)
            {
                Visit(node.Expression);
                Out(" is ");
                Out(GetTypeName(node.TypeOperand));
                return node;
            }

            Visit(node.Expression);

            if (node.Expression.Type.IsNullableType())
            {
                // C#6 Null-conditional operator
                Out("?");
            }

            Out(".GetType() == typeof(");
            Out(GetTypeName(node.TypeOperand));
            Out(")");

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.UnaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Increment:
                    Out("(");
                    Visit(node.Operand);
                    Out(" + 1)");
                    break;
                case ExpressionType.Throw:
                    // Operand can actually be null
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (node.Operand == null)
                        // ReSharper disable once HeuristicUnreachableCode
                        Out("throw");
                    else
                    {
                        Out("throw ");
                        Visit(node.Operand);
                    }
                    break;
                case ExpressionType.ConvertChecked:
                    Out("checked((" + GetTypeName(node.Type) + ")");
                    Visit(node.Operand);
                    Out(")");
                    break;
                case ExpressionType.Convert:
                case ExpressionType.Unbox:
                    Out("((" + GetTypeName(node.Type) + ")");
                    Visit(node.Operand);
                    Out(")");
                    break;
                case ExpressionType.PreIncrementAssign:
                    Out("++");
                    Visit(node.Operand);
                    break;
                case ExpressionType.PreDecrementAssign:
                    Out("--");
                    Visit(node.Operand);
                    break;
                case ExpressionType.OnesComplement:
                    Out("~");
                    Visit(node.Operand);
                    break;
                case ExpressionType.IsTrue:
                case ExpressionType.IsFalse:
                    Visit(node.Operand);
                    break;
                case ExpressionType.Decrement:
                    Out("(");
                    Visit(node.Operand);
                    Out(" - 1)");
                    break;
                case ExpressionType.Negate:
                    Out("-");
                    Visit(node.Operand);
                    break;
                case ExpressionType.UnaryPlus:
                    Out("+");
                    Visit(node.Operand);
                    break;
                case ExpressionType.NegateChecked:
                    Out("checked(-");
                    Visit(node.Operand);
                    Out(")");
                    break;
                case ExpressionType.Not:
                    Out(node.Type == typeof(bool) ? "!" : "~");
                    Visit(node.Operand);
                    break;
                case ExpressionType.Quote:
                    Visit(node.Operand);
                    break;
                case ExpressionType.ArrayLength:
                    Visit(node.Operand);
                    Out(".Length");
                    break;
                case ExpressionType.TypeAs:
                    Visit(node.Operand);
                    Out(" as " + GetTypeName(node.Type));
                    break;
                case ExpressionType.PostIncrementAssign:
                    Visit(node.Operand);
                    Out("++");
                    break;
                case ExpressionType.PostDecrementAssign:
                    Visit(node.Operand);
                    Out("--");
                    break;
            }

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberInitExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            Visit(node.NewExpression);
            NewLine();

            OutLine("{");
            Indent();

            bool first = true;
            foreach (MemberBinding binding in node.Bindings)
            {
                if (first) first = false;
                else OutLine(",");

                Debug.Assert(binding != null);
                VisitMemberBinding(binding);
            }

            NewLine();
            Deindent();
            Out("}");

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.ListInitExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitListInit(ListInitExpression node)
        {
            Visit(node.NewExpression);
            NewLine();

            OutLine("{");
            Indent();

            bool first = true;
            foreach (ElementInit init in node.Initializers)
            {
                if (first) first = false;
                else OutLine(",");

                Debug.Assert(init != null);
                VisitElementInit(init);
            }

            NewLine();
            Deindent();
            Out("}");

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.ElementInit" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override ElementInit VisitElementInit(ElementInit node)
        {
            if (node.Arguments.Count > 1)
                Out("{ ");

            bool first = true;
            foreach (Expression arg in node.Arguments)
            {
                if (first) first = false;
                else Out(", ");

                Visit(arg);
            }
            if (node.Arguments.Count > 1)
                Out(" }");

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberAssignment" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            Out(node.Member.Name);
            Out(" = ");
            Visit(node.Expression);

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberMemberBinding" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            Out(node.Member.Name);
            OutLine(" = ");
            OutLine("{");
            Indent();

            bool first = true;
            foreach (MemberBinding binding in node.Bindings)
            {
                if (first) first = false;
                else OutLine(",");

                Debug.Assert(binding != null);
                VisitMemberBinding(binding);
            }

            Deindent();
            Out("}");

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberListBinding" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            Out(node.Member.Name);
            OutLine(" = ");
            OutLine("{");
            Indent();

            bool first = true;
            foreach (ElementInit init in node.Initializers)
            {
                if (first) first = false;
                else OutLine(",");

                Debug.Assert(init != null);
                VisitElementInit(init);
            }

            Deindent();
            Out("}");

            return node;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            foreach (string lambda in _lambdaDefinitions
                .OrderBy(kvp => GetLambdaName(kvp.Key))
                .Select(kvp => kvp.Value))
            {
                NewLine();
                NewLine();
                Debug.Assert(lambda != null);
                Out(lambda);
            }

            return _builder.ToString();
        }

        /// <summary>
        /// Gets the constant string for a value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [NotNull]
        private string ConstantString([NotNull] object value)
        {
            Type type = value.GetType();
            if (type == typeof(object)) return "new object()";
            if (type == typeof(string)) return "\"" + ((string)value).Escape() + "\"";
            if (type == typeof(sbyte)) return ((sbyte)value).ToString("D", CultureInfo.InvariantCulture);
            if (type == typeof(byte)) return ((byte)value).ToString("D", CultureInfo.InvariantCulture);
            if (type == typeof(short)) return ((short)value).ToString("D", CultureInfo.InvariantCulture);
            if (type == typeof(ushort)) return ((ushort)value).ToString("D", CultureInfo.InvariantCulture);
            if (type == typeof(int)) return ((int)value).ToString("D", CultureInfo.InvariantCulture);
            if (type == typeof(uint)) return ((uint)value).ToString("D", CultureInfo.InvariantCulture) + "U";
            if (type == typeof(long)) return ((long)value).ToString("D", CultureInfo.InvariantCulture) + "L";
            if (type == typeof(ulong)) return ((ulong)value).ToString("D", CultureInfo.InvariantCulture) + "UL";
            if (type == typeof(decimal)) return ((decimal)value).ToString("G", CultureInfo.InvariantCulture) + "M";
            if (type == typeof(float)) return ((float)value).ToString("R", CultureInfo.InvariantCulture) + "F";
            if (type == typeof(double)) return ((double)value).ToString("R", CultureInfo.InvariantCulture) + "D";
            if (type == typeof(bool)) return (bool)value ? "true" : "false";
            if (type == typeof(char)) return "'" + Char.ToString((char)value).Escape() + "'";
            if (type.DescendsFrom<Type>()) return "typeof(" + GetTypeName((Type)value) + ")";
            if (type.DescendsFrom<Enum>())
            {
                Enum e = (Enum)value;
                string val = e.ToString();
                string enumName = GetTypeName(e.GetType());

                int i;
                return int.TryParse(val, out i)
                    ? "(" + enumName + ")" + (i < 0 ? "(" + val + ")" : val)
                    : enumName + "." + val.Replace(", ", " | " + enumName + ".");
            }
            if (type.DescendsFrom<LambdaExpression>()) return ConstantString((LambdaExpression)value, type);
            if (type.DescendsFrom<Delegate>()) return ConstantString((Delegate)value, type);

            string str = value.ToString();

            // If the ToString method is not overridden, it just returns the type name
            // If its not overridden, don't bother returning the ToString value
            if (str == type.FullName)
                str = "...";

            return string.Format("constant<{0}>({1})", GetTypeName(type), str);
        }

        /// <summary>
        /// Gets the constant string for a <see cref="LambdaExpression" />.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [NotNull]
        private string ConstantString([NotNull] LambdaExpression node, [NotNull] Type type)
        {
            // If the lambda does not have a name, is not a block, and all the parameters are not byref, output the lambda inline
            if (node.Name == null &&
                !(node.Body is BlockExpression) &&
                // ReSharper disable once PossibleNullReferenceException
                node.Parameters.All(p => !p.IsByRef))
            {
                bool isRoot = _lambdaIsRoot;
                StringBuilder builder = _builder;
                _builder = new StringBuilder();
                int indent = _indent;
                _lambdaIsRoot = false;

                if (node.Parameters.Count == 1)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Out(GetParameterName(node.Parameters.First()));
                }
                else
                {
                    Out("(");
                    bool first = true;
                    foreach (ParameterExpression parameter in node.Parameters)
                    {
                        Debug.Assert(parameter != null);
                        if (first) first = false;
                        else Out(", ");

                        Out(GetParameterName(parameter));
                    }
                    Out(")");
                }

                Out(" => ");

                Visit(node.Body);

                string lambda = _builder.ToString();

                _builder = builder;
                _indent = indent;
                _newLine = false;
                _lambdaIsRoot = isRoot;

                return lambda;
            }

            // TODO Do something different here
            return string.Format("constant<{0}>(...)", GetTypeName(type));
        }

        /// <summary>
        /// Gets the constant string for a <see cref="Delegate" />.
        /// </summary>
        /// <param name="delegate">The delegate.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [NotNull]
        private string ConstantString([NotNull] Delegate @delegate, [NotNull] Type type)
        {
            if (@delegate.Method == null || @delegate.Method.DeclaringType == null)
                return string.Format("constant<{0}>(...)", GetTypeName(type));

            StringBuilder sb = new StringBuilder();

            sb.Append("new ");
            sb.Append(GetTypeName(type));
            sb.Append("(");
            if (@delegate.Target != null)
                sb.Append(ConstantString(@delegate.Target));
            else if (@delegate.Method.IsStatic)
                sb.Append(GetTypeName(@delegate.Method.DeclaringType));
            else
                return string.Format("constant<{0}>(...)", GetTypeName(type));

            sb.Append(".");
            sb.Append(@delegate.Method.Name);

            if (@delegate.Method.IsGenericMethod)
            {
                sb.Append("<");
                bool first = true;
                foreach (Type typeArg in @delegate.Method.GetGenericArguments())
                {
                    Debug.Assert(typeArg != null);
                    if (first) first = false;
                    else sb.Append(", ");
                    sb.Append(GetTypeName(typeArg));
                }
                sb.Append(">");
            }
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// Determines whether the <paramref name="child"/> needs parantheses.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="child">The child.</param>
        /// <returns></returns>
        private static bool NeedsParentheses([NotNull] Expression parent, [CanBeNull] Expression child)
        {
            if (child == null)
                return false;

            switch (parent.NodeType)
            {
                case ExpressionType.Increment:
                case ExpressionType.Decrement:
                case ExpressionType.IsTrue:
                case ExpressionType.IsFalse:
                case ExpressionType.Power:
                    return false;
                case ExpressionType.Unbox:
                    return true;
            }

            int childOpPrec = GetOperatorPrecedence(child);
            int parentOpPrec = GetOperatorPrecedence(parent);

            if (childOpPrec == parentOpPrec)
            {
                // When parent op and child op has the same precedence,
                // we want to be a little conservative to have more clarity.
                // Parentheses are not needed if
                // 1) Both ops are &&, ||, &, |, or ^, all of them are the only
                // op that has the precedence.
                // 2) Parent op is + or *, e.g. x + (y - z) can be simplified to
                // x + y - z.
                // 3) Parent op is -, / or %, and the child is the left operand.
                // In this case, if left and right operand are the same, we don't
                // remove parenthesis, e.g. (x + y) - (x + y)
                // 
                switch (parent.NodeType)
                {
                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                    case ExpressionType.And:
                    case ExpressionType.Or:
                    case ExpressionType.ExclusiveOr:
                        // Since these ops are the only ones on their precedence,
                        // the child op must be the same.
                        Debug.Assert(child.NodeType == parent.NodeType);
                        // We remove the parenthesis, e.g. x && y && z
                        return false;
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                        return false;
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                    case ExpressionType.Divide:
                    case ExpressionType.Modulo:
                        BinaryExpression binary = parent as BinaryExpression;
                        Debug.Assert(binary != null);
                        // Need to have parenthesis for the right operand.
                        return child == binary.Right;
                }
                return true;
            }

            // Special case: negate of a constant needs parentheses, to
            // disambiguate it from a negative constant.
            if (child.NodeType == ExpressionType.Constant &&
                (parent.NodeType == ExpressionType.Negate || parent.NodeType == ExpressionType.NegateChecked))
                return true;

            // If the parent op has higher precedence, need parentheses for the child.
            return childOpPrec < parentOpPrec;
        }

        /// <summary>
        /// Gets the operator precedence for the node given.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private static int GetOperatorPrecedence([NotNull] Expression node)
        {
            // Roughly matches C# operator precedence, with some additional
            // operators. Also things which are not binary/unary expressions,
            // such as conditional and type testing, don't use this mechanism.
            switch (node.NodeType)
            {
                // Assignment
                case ExpressionType.Assign:
                case ExpressionType.ExclusiveOrAssign:
                case ExpressionType.AddAssign:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.SubtractAssign:
                case ExpressionType.SubtractAssignChecked:
                case ExpressionType.DivideAssign:
                case ExpressionType.ModuloAssign:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.LeftShiftAssign:
                case ExpressionType.RightShiftAssign:
                case ExpressionType.AndAssign:
                case ExpressionType.OrAssign:
                case ExpressionType.PowerAssign:
                case ExpressionType.Coalesce:
                    return 1;

                // Conditional (?:) would go here

                // Conditional OR
                case ExpressionType.OrElse:
                    return 2;

                // Conditional AND
                case ExpressionType.AndAlso:
                    return 3;

                // Logical OR
                case ExpressionType.Or:
                    return 4;

                // Logical XOR
                case ExpressionType.ExclusiveOr:
                    return 5;

                // Logical AND
                case ExpressionType.And:
                    return 6;

                // Equality
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return 7;

                // Relational, type testing
                case ExpressionType.GreaterThan:
                case ExpressionType.LessThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.TypeAs:
                case ExpressionType.TypeIs:
                case ExpressionType.TypeEqual:
                    return 8;

                // Shift
                case ExpressionType.LeftShift:
                case ExpressionType.RightShift:
                    return 9;

                // Additive
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return 10;

                // Multiplicative
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return 11;

                // Unary
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.UnaryPlus:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.PreDecrementAssign:
                case ExpressionType.OnesComplement:
                case ExpressionType.Increment:
                case ExpressionType.Decrement:
                case ExpressionType.IsTrue:
                case ExpressionType.IsFalse:
                case ExpressionType.Unbox:
                case ExpressionType.Throw:
                    return 12;

                // Power, which is not in C#
                // But VB/Python/Ruby put it here, above unary.
                case ExpressionType.Power:
                    return 13;

                // Primary, which includes all other node types:
                //   member access, calls, indexing, new.
                // ReSharper disable RedundantCaseLabel
                case ExpressionType.PostIncrementAssign:
                case ExpressionType.PostDecrementAssign:
                // ReSharper restore RedundantCaseLabel
                default:
                    return 14;

                // These aren't expressions, so never need parentheses:
                //   constants, variables
                case ExpressionType.Constant:
                case ExpressionType.Parameter:
                    return 15;
            }
        }
    }
}