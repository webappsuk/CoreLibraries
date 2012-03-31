using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Wraps the method information with accessors for retrieving parameters.
    /// </summary>
    public class Method
    {
        /// <summary>
        /// The extended type.
        /// </summary>
        [NotNull]
        public readonly ExtendedType ExtendedType;

        /// <summary>
        ///   The method info.
        /// </summary>
        [NotNull]
        public readonly MethodInfo Info;

        /// <summary>
        ///   The parameters.
        /// </summary>
        [NotNull]
        public readonly IEnumerable<ParameterInfo> Parameters;

        /// <summary>
        /// Gets the type of the return.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        public Type ReturnType { get { return Info.ReturnType; }}

        /// <summary>
        ///   Initializes the <see cref="Method"/> class.
        /// </summary>
        /// <param name="extendedType">The extended type.</param>
        /// <param name="info">The method.</param>
        /// <param name="parameters">The parameters.</param>
        internal Method([NotNull]ExtendedType extendedType, [NotNull]MethodInfo info, [NotNull]IEnumerable<ParameterInfo> parameters)
        {
            ExtendedType = extendedType;
            Info = info;
            Parameters = parameters;
        }
    }
}