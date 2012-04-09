using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Reflect
{
    /// <summary>
    /// A method that implements a cast operator.
    /// </summary>
    /// <remarks></remarks>
    public class CastMethod : Method
    {
        /// <summary>
        /// Whether the cast is an explicit cast.
        /// </summary>
        public readonly bool IsExplicit;

        /// <summary>
        /// The type this cast casts from.
        /// </summary>
        public readonly Type FromType;

        /// <summary>
        /// The type this cast casts to.
        /// </summary>
        public readonly Type ToType;

        /// <summary>
        /// Initializes a new instance of the <see cref="CastMethod" /> class.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="info">The info.</param>
        /// <param name="isExplicit">if set to <see langword="true" /> [is explicit].</param>
        /// <remarks></remarks>
        internal CastMethod([NotNull] ExtendedType extendedType, [NotNull] MethodInfo info, bool isExplicit)
                : base(extendedType, info)
        {
            Contract.Requires(info.Name=="op_Explicit"||info.Name=="op_Implicit");
            Contract.Requires(ParameterTypes.Count() == 1);
            this.IsExplicit = isExplicit;
            this.FromType = ParameterTypes.First();
            this.ToType = ReturnType;
            Contract.Assert(FromType == extendedType.Type || ToType == extendedType.Type);
        }
    }
}