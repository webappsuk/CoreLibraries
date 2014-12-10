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
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Writes an <see cref="Expression"/> tree to C# like code.
    /// </summary>
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
        [PublicAPI]
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
        private readonly Dictionary<LambdaExpression, string> _lambdaDefinitions = new Dictionary<LambdaExpression, string>();

        private bool _lambdaIsRoot;

        [NotNull]
        private readonly PreVisitor _preVisitor;

        private int _indent;
        private bool _newLine;

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
            public readonly bool FullTypeNames;

            public PreVisitor(bool fullTypeNames)
            {
                _typeNames = new Lazy<Dictionary<Type, string>>(GetTypeNames);
                _types.Add(typeof(Math));
                FullTypeNames = fullTypeNames;
            }

            private void AddType([NotNull] Type type)
            {
                AddTypes(type);
            }

            private void AddTypes([NotNull] params Type[] types)
            {
                Stack<Type> stack = new Stack<Type>(types);

                Type type;
                while (stack.TryPop(out type))
                {
                    _types.Add(type);

                    if (type.IsGenericTypeDefinition)
                        continue;

                    Type[] typeArgs = type.GetGenericArguments();
                    foreach (Type t in typeArgs)
                    {
                        stack.Push(t);
                        _types.Add(type, t);
                    }
                }
            }

            [NotNull]
            private Dictionary<Type, string> GetTypeNames()
            {
                Dictionary<string, Type> uniqueTypes =
                    new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

                Lookup<string, Type> ambiguousTypes =
                    new Lookup<string, Type>(0, StringComparer.InvariantCultureIgnoreCase);

                foreach (Type type in _types.AllTopDown)
                {
                    string name;
                    if (FullTypeNames || !_keywordTypes.TryGetValue(type, out name))
                        name = type.FullName;

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
                    if (!uniqueTypes.ContainsKey(group.Key) &&
                        group.HasExact(1))
                        uniqueTypes[group.Key] = group.First();
                }

                Dictionary<Type, string> dict = uniqueTypes.GroupBy(kvp => kvp.Value, kvp => kvp.Key)
                    // ReSharper disable once PossibleNullReferenceException
                    .ToDictionary(g => g.Key, g => g.MinBy(p => p.Split('.').Length).Replace('+', '.'));

                foreach (Type type in _types.AllTopDown.Where(t => t.IsGenericType))
                {
                    string name = dict[type];

                    if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        name = dict[type.GetGenericArguments()[0]] + "?";
                    else
                    {
                        int genIndex = name.IndexOf('`');
                        if (genIndex > 0)
                            name = name.Substring(0, genIndex);

                        name = name + "<" +
                               string.Join(
                                   ", ",
                                   type.GetGenericArguments().Select(t => t.IsGenericParameter ? string.Empty : dict[t])) +
                               ">";
                    }

                    dict[type] = name;
                }

                return dict;
            }

            [CanBeNull]
            public override Expression Visit([CanBeNull] Expression node)
            {
                if (node != null)
                    AddType(node.Type);
                return base.Visit(node);
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                Type type = node.Value as Type;
                if (type != null)
                    AddType(type);

                return base.VisitConstant(node);
            }

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

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                if (node.ReturnType != null)
                    AddType(node.ReturnType);
                if (!string.IsNullOrWhiteSpace(node.Name))
                    _lambdaNames[node] = node.Name;

                return base.VisitLambda(node);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (!string.IsNullOrWhiteSpace(node.Name))
                    _parameterNames[node] = node.Name;

                return base.VisitParameter(node);
            }

            [NotNull]
            protected override Expression VisitDynamic(DynamicExpression node)
            {
                if (node.DelegateType != null)
                    AddType(node.DelegateType);
                return base.VisitDynamic(node);
            }

            protected override Expression VisitTypeBinary(TypeBinaryExpression node)
            {
                AddType(node.TypeOperand);
                return base.VisitTypeBinary(node);
            }

            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                if (node.Test != null)
                    AddType(node.Test);
                return base.VisitCatchBlock(node);
            }

            [NotNull]
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Object == null) // ReSharper disable once AssignNullToNotNullAttribute
                    AddType(node.Method.DeclaringType);

                AddTypes(node.Method.GetGenericArguments());

                return base.VisitMethodCall(node);
            }

            protected override Expression VisitNewArray(NewArrayExpression node)
            {
                AddType(node.Type.GetElementType());

                AddTypes(node.Type.GetElementType().GetGenericArguments());

                return base.VisitNewArray(node);
            }

            protected override Expression VisitNew(NewExpression node)
            {
                AddTypes(node.Type.GetGenericArguments());

                return base.VisitNew(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression == null) // ReSharper disable once AssignNullToNotNullAttribute
                    AddType(node.Member.DeclaringType);

                return base.VisitMember(node);
            }

            [NotNull]
            public string GetLabelName([NotNull] LabelTarget label)
            {
                string name;
                if (_labelNames.TryGetValue(label, out name))
                    return name;

                while (_labelNames.ContainsValue(name = "label" + _labelId++))
                {
                }
                _labelNames[label] = name;
                return name;
            }

            [NotNull]
            public string GetLambdaName([NotNull] LambdaExpression lambda)
            {
                string name;
                if (_lambdaNames.TryGetValue(lambda, out name))
                    return name;

                while (_lambdaNames.ContainsValue(name = "Lambda" + _lambdaId++))
                {
                }
                _lambdaNames[lambda] = name;
                return name;
            }

            [NotNull]
            public string GetParameterName([NotNull] ParameterExpression parameter)
            {
                string name;
                if (_parameterNames.TryGetValue(parameter, out name))
                    return name;

                while (_parameterNames.ContainsValue(name = "var" + _paramId++))
                {
                }
                _parameterNames[parameter] = name;
                return name;
            }

            [NotNull]
            public string GetTypeName([NotNull] Type type)
            {
                string name;
                if (_typeNames.Value.TryGetValue(type, out name))
                    return name;

                if (type.IsGenericType)
                {
                    name = type.FullName;
                    name = name.Substring(0, name.IndexOf('`'));
                    return name + "<" + string.Join(", ", type.GetGenericArguments().Select(GetTypeName)) + ">";
                }

                return type.FullName;
            }
        }

        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetLabelName([NotNull] LabelTarget label)
        {
            return _preVisitor.GetLabelName(label);
        }

        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetLambdaName([NotNull] LambdaExpression lambda)
        {
            return _preVisitor.GetLambdaName(lambda);
        }

        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetParameterName([NotNull] ParameterExpression parameter)
        {
            return _preVisitor.GetParameterName(parameter);
        }

        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetTypeName([NotNull] Type type)
        {
            return _preVisitor.GetTypeName(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Indent()
        {
            _indent += Tab;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once IdentifierTypo
        private void Deindent()
        {
            _indent -= Tab;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Out([NotNull] object s)
        {
            if (_newLine)
                _builder.Append(TabChar, _indent);
            _newLine = false;
            _builder.Append(s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Out([NotNull] string s)
        {
            if (_newLine)
                _builder.Append(TabChar, _indent);
            _newLine = false;
            _builder.Append(s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Out(char c)
        {
            if (_newLine)
                _builder.Append(TabChar, _indent);
            _newLine = false;
            _builder.Append(c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OutLine([NotNull] string s)
        {
            if (_newLine)
                _builder.Append(TabChar, _indent);
            _builder.AppendLine(s);
            _newLine = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void NewLine()
        {
            _builder.AppendLine();
            _newLine = true;
        }

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
                        throw new NotImplementedException();
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

        [NotNull]
        protected override Expression VisitBlock(BlockExpression node)
        {
            OutLine("{");
            Indent();

            foreach (ParameterExpression variable in node.Variables)
                OutLine(GetTypeName(variable.Type) + " " + GetParameterName(variable) + ";");

            VisitStatements(node.Expressions);

            Deindent();
            Out("}");

            return node;
        }

        private void VisitStatements([NotNull] ReadOnlyCollection<Expression> expressions)
        {
            Visit(expressions, VisitStatement);
        }

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

        protected override Expression VisitConstant(ConstantExpression node)
        {
            object value = node.Value;
            Out(value == null ? "null" : ConstantString(value));

            return node;
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            // TODO Implement properly
            Out(node.ToString());
            return node;
        }

        [NotNull]
        protected override Expression VisitDynamic(DynamicExpression node)
        {
            // TODO Implement properly
            Out(node.ToString());
            return node;
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            if (node.Type != typeof(void))
                Out(string.Format("default({0})", GetTypeName(node.Type)));
            return node;
        }

        [NotNull]
        protected override Expression VisitExtension(Expression node)
        {
            if (node.CanReduce)
                Visit(node.Reduce());
            else
                throw new InvalidOperationException();

            return node;
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            switch (node.Kind)
            {
                case GotoExpressionKind.Goto:
                    Out("goto " + GetLabelName(node.Target));
                    if (node.Type != typeof(void))
                    {
                        Out(" ");
                        Visit(node.Value);
                    }
                    break;
                case GotoExpressionKind.Return:
                    Out("return");
                    if (node.Type != typeof(void))
                    {
                        Out(" ");
                        Visit(node.Value);
                    }
                    break;
                case GotoExpressionKind.Break:
                    Out("break");
                    if (node.Type != typeof(void))
                    {
                        Out(" ");
                        Visit(node.Value);
                    }
                    break;
                case GotoExpressionKind.Continue:
                    Out("continue");
                    if (node.Type != typeof(void))
                    {
                        Out(" ");
                        Visit(node.Value);
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }

            return node;
        }

        [NotNull]
        protected override Expression VisitInvocation([NotNull] InvocationExpression node)
        {
            Visit(node.Expression);
            VisitMethod(".Invoke", Array<Type>.Empty, node.Arguments.ToArray(), "(", ")");

            return node;
        }

        protected override Expression VisitLabel(LabelExpression node)
        {
            Out(GetLabelName(node.Target));

            if (node.Type != typeof(void))
            {
                Out(" ");
                Visit(node.DefaultValue);
            }

            Out(":");
            return node;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            bool isRoot = _lambdaIsRoot;
            StringBuilder builder = _builder;
            int indent = _indent;
            bool newLine = _newLine;
            _lambdaIsRoot = false;

            string name = GetLambdaName(node);

            if (!_lambdaDefinitions.ContainsKey(node))
            {
                if (!isRoot)
                {
                    Out(name);

                    _builder = new StringBuilder();
                    _indent = 0;
                    newLine = false;
                }

                Out(GetTypeName(node.ReturnType) + " " + name);

                Out("(");
                for (int index = 0; index < node.Parameters.Count; index++)
                {
                    if (index > 0) Out(", ");

                    ParameterExpression parameter = node.Parameters[index];
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
                    _newLine = newLine;
                }
            }
            else if (!isRoot)
            {
                Out(name);
            }

            return node;
        }

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
                VisitStatement(node.Body);
                Deindent();
                Out("}");
            }

            if (node.BreakLabel != null)
                Out(" " + GetLabelName(node.BreakLabel) + ":");

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null)
                Visit(node.Expression);
            else // ReSharper disable once AssignNullToNotNullAttribute
                Out(GetTypeName(node.Member.DeclaringType));
            Out("." + node.Member.Name);

            return node;
        }

        [NotNull]
        protected override Expression VisitIndex([NotNull] IndexExpression node)
        {
            Visit(node.Object);
            Out("[");

            bool first = true;
            foreach (Expression index in node.Arguments)
            {
                if (first) first = false;
                else Out(", ");

                Visit(index);
            }

            Out("]");

            return node;
        }

        [NotNull]
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!_preVisitor.FullTypeNames && node.Method.GetCustomAttribute<ExtensionAttribute>() != null)
            {
                Visit(node.Arguments[0]);
                Out(".");
                VisitMethod(node.Method.Name, node.Method.GetGenericArguments(), node.Arguments.Skip(1).ToArray(), "(", ")");

                return node;
            }

            if (node.Object == null) // ReSharper disable once AssignNullToNotNullAttribute
                Out(GetTypeName(node.Method.DeclaringType));
            else
                Visit(node.Object);

            Out(".");
            VisitMethod(node.Method.Name, node.Method.GetGenericArguments(), node.Arguments.ToArray(), "(", ")");

            return node;
        }

        private void VisitMethod(
            [NotNull] string name,
            [NotNull] Type[] typeArgs,
            [NotNull] IEnumerable<Expression> args,
            [NotNull] string openBracket,
            [NotNull] string closeBracket)
        {
            Out(name);

            bool first;
            if (typeArgs.Length > 0)
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
            foreach (Expression index in args)
            {
                if (first) first = false;
                else Out(", ");

                // TODO Add "ref " for by ref parameters

                Visit(index);
            }

            Out(closeBracket);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            Out("new ");
            Type elementType = node.Type.GetElementType();

            if (node.NodeType == ExpressionType.NewArrayBounds)
                VisitMethod(GetTypeName(elementType), Array<Type>.Empty, node.Expressions.ToArray(), "[", "]");
            else
            {
                Contract.Assert(node.NodeType == ExpressionType.NewArrayInit);
                VisitMethod(GetTypeName(elementType) + "[]", Array<Type>.Empty, node.Expressions.ToArray(), "{ ", " }");
            }

            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            Out("new ");
            VisitMethod(GetTypeName(node.Type), Array<Type>.Empty, node.Arguments.ToArray(), "(", ")");

            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            Out(GetParameterName(node));

            return node;
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            // TODO Implement properly
            Out(node.GetDebugView());
            return node;
        }

        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            Visit(
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
                if (block.Variables.Count < 1)
                    VisitStatements(block.Expressions);
                else
                    Visit(node.Body);

                if (block.Expressions.Last() is GotoExpression)
                    needsBreak = false;
            }
            else
                VisitStatement(node.Body);

            if (needsBreak)
                OutLine("break;");
            Deindent();

            return node;
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            Out("switch (");
            Visit(node.SwitchValue);
            OutLine(")");
            OutLine("{");
            Indent();

            Visit(node.Cases, VisitSwitchCase);

            if (node.DefaultBody != null)
            {
                OutLine("default:");
                Indent();

                bool needsBreak = true;

                BlockExpression block = node.DefaultBody as BlockExpression;
                if (block != null)
                {
                    if (block.Variables.Count < 1)
                        VisitStatements(block.Expressions);
                    else
                        Visit(block);

                    if (block.Expressions.Last() is GotoExpression)
                        needsBreak = false;
                }
                else
                    VisitStatement(node.DefaultBody);

                if (needsBreak)
                    OutLine("break;");
                Deindent();
            }

            Deindent();
            Out("}");

            return node;
        }

        protected override Expression VisitTry(TryExpression node)
        {
            OutLine("try");

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
                VisitStatement(node.Body);
                Deindent();
                OutLine("}");
            }

            return node;
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            if (node.NodeType == ExpressionType.TypeIs)
            {
                Visit(node.Expression);
                Out(" is ");
                Out(GetTypeName(node.TypeOperand));
                return node;
            }

            if (node.Expression.Type.IsNullableType())
            {
                // TODO Something...
            }

            Visit(node.Expression);
            Out(".GetType() == ");
            Out(GetTypeName(node.TypeOperand));

            return node;
        }

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

                        // TODO If operand is not of type exception, wrap in new RuntimeWrappedException?
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

                VisitMemberBinding(binding);
            }

            NewLine();
            Deindent();
            Out("}");

            return node;
        }

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

                VisitElementInit(init);
            }

            NewLine();
            Deindent();
            Out("}");

            return node;
        }

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

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            Out(node.Member.Name);
            Out(" = ");
            Visit(node.Expression);

            return node;
        }

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

                VisitMemberBinding(binding);
            }

            Deindent();
            Out("}");

            return node;
        }

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

                VisitElementInit(init);
            }

            Deindent();
            Out("}");

            return node;
        }

        public override string ToString()
        {
            foreach (string lambda in _lambdaDefinitions.OrderBy(kvp => GetLambdaName(kvp.Key)).Select(kvp => kvp.Value))
            {
                NewLine();
                NewLine();
                Out(lambda);
            }

            return _builder.ToString();
        }

        [UsedImplicitly]
        private string ConstantString([NotNull] object value)
        {
            Type type = value.GetType();
            if (type == typeof(object)) return "new object()";
            if (type == typeof(string)) return ConstantString((string)value);
            if (type == typeof(sbyte)) return ConstantString((sbyte)value);
            if (type == typeof(byte)) return ConstantString((byte)value);
            if (type == typeof(short)) return ConstantString((short)value);
            if (type == typeof(ushort)) return ConstantString((ushort)value);
            if (type == typeof(int)) return ConstantString((int)value);
            if (type == typeof(uint)) return ConstantString((uint)value);
            if (type == typeof(long)) return ConstantString((long)value);
            if (type == typeof(ulong)) return ConstantString((ulong)value);
            if (type == typeof(decimal)) return ConstantString((decimal)value);
            if (type == typeof(float)) return ConstantString((float)value);
            if (type == typeof(double)) return ConstantString((double)value);
            if (type == typeof(bool)) return ConstantString((bool)value);
            if (type == typeof(char)) return ConstantString((char)value);
            if (type.DescendsFrom<Type>()) ConstantString((Type)value);
            if (type.DescendsFrom<Enum>()) return ConstantString((Enum)value);

            string str = value.ToString();

            // If the ToString method is not overridden, it just returns the type name
            // If its not overridden, don't bother returning the ToString value
            if (str == type.FullName)
                str = "...";

            return string.Format("constant<{0}>({1})", GetTypeName(type), str);
        }

        // ReSharper disable CodeAnnotationAnalyzer
        [UsedImplicitly]
        private string ConstantString(string value)
        {
            return "\"" + value.Escape() + "\"";
        }

        [UsedImplicitly]
        private string ConstantString(bool value)
        {
            return value ? "true" : "false";
        }

        [UsedImplicitly]
        private string ConstantString(char value)
        {
            return "'" + Char.ToString(value).Escape() + "'";
        }

        [UsedImplicitly]
        private string ConstantString(Enum value)
        {
            string val = value.ToString();
            string enumName = GetTypeName(value.GetType());

            int i;
            return int.TryParse(val, out i)
                ? "(" + enumName + ")" + (i < 0 ? "(" + val + ")" : val)
                : enumName + "." + val.Replace(", ", " | " + enumName + ".");
        }

        [UsedImplicitly]
        private string ConstantString(int value)
        {
            return value.ToString("D");
        }

        [UsedImplicitly]
        private string ConstantString(uint value)
        {
            return value.ToString("D") + "U";
        }

        [UsedImplicitly]
        private string ConstantString(long value)
        {
            return value.ToString("D") + "L";
        }

        [UsedImplicitly]
        private string ConstantString(ulong value)
        {
            return value.ToString("D") + "UL";
        }

        [UsedImplicitly]
        private string ConstantString(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture) + "M";
        }

        [UsedImplicitly]
        private string ConstantString(float value)
        {
            return value.ToString("R") + "F";
        }

        [UsedImplicitly]
        private string ConstantString(double value)
        {
            return value.ToString("R") + "D";
        }

        [UsedImplicitly]
        private string ConstantString(Type value)
        {
            return "typeof(" + GetTypeName(value) + ")";
        }

        // ReSharper restore CodeAnnotationAnalyzer

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
                        Contract.Assert(child.NodeType == parent.NodeType);
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
                        Contract.Assert(binary != null);
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