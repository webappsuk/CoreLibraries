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

using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Expression visitor that gets all the <see cref="ParameterExpression">parameters</see> that are actually used within an expression.
    /// </summary>
    [PublicAPI]
    public class ParameterUsageVisitor : ExpressionVisitor, IReadOnlyCollection<ParameterExpression>
    {
        [NotNull]
        private readonly HashSet<ParameterExpression> _visitedParameters = new HashSet<ParameterExpression>();

        /// <summary>
        /// Prevents a default instance of the <see cref="ParameterUsageVisitor"/> class from being created.
        /// </summary>
        private ParameterUsageVisitor()
        {
        }

        /// <summary>
        /// Gets whether the parameter given is used within the visited expression.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        [PublicAPI]
        public bool Contains([NotNull] ParameterExpression parameter)
        {
            return _visitedParameters.Contains(parameter);
        }

        /// <summary>
        /// Determines the parameters used within the given expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public static ParameterUsageVisitor Create([NotNull] Expression expression)
        {
            ParameterUsageVisitor visitor = new ParameterUsageVisitor();
            visitor.Visit(expression);
            return visitor;
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
            _visitedParameters.Add(node);
            return base.VisitParameter(node);
        }

        /// <summary>
        /// Visits the lambda.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            // Overridden to skip visiting the parameters to the lambda, as we only care if the parameters are used.
            Visit(node.Body);
            return node;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ParameterExpression> GetEnumerator()
        {
            return _visitedParameters.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the number of parameters used in the expression.
        /// </summary>
        /// <returns>The number of parameters used in the expression. </returns>
        public int Count
        {
            get { return _visitedParameters.Count; }
        }
    }
}