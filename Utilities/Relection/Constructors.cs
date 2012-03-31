using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Holds information about all constructors with a given name.
    /// </summary>
    public class Constructors
    {
        /// <summary>
        /// The extended type.
        /// </summary>
        [NotNull]
        public readonly ExtendedType ExtendedType;

        /// <summary>
        ///   It is possible to have overloads with ref/out parameters,
        ///   hence type position is not always enough to disambiguate.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, Constructor> _constructors = new Dictionary<string,Constructor>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="Overloadable&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="extendedType">The extended type.</param>
        /// <param name="constructor">The constructor to add.</param>
        internal Constructors([NotNull]ExtendedType extendedType, [NotNull]ConstructorInfo constructor)
        {
            ExtendedType = extendedType;
            Add(constructor);
        }

        /// <summary>
        ///   Adds the specified constructor.
        /// </summary>
        /// <param name="constructor">The constructor to add.</param>
        internal void Add([NotNull]ConstructorInfo constructor)
        {
            // Build type array.
            ParameterInfo[] paramters = constructor.GetParameters();
            int pCount = paramters.Length;
            Type[] types = new Type[pCount];
            for (int a = 0; a < pCount; a++)
                types[a] = paramters[a].ParameterType;

            // Add constructor
            _constructors.Add(CreateKey(types), new Constructor(ExtendedType, constructor, paramters));
        }

        /// <summary>
        /// Gets the overloads for this constructor.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        public IEnumerable<Constructor> Overloads { get { return _constructors.Values; } }

        /// <summary>
        ///   Creates a key for a signature.
        /// </summary>
        /// <param name="types">The parameter types.</param>
        /// <remarks>
        ///   We prepend the <paramref name="types"/> <see cref="System.Array.Length">count</see>
        ///   to the key as this will cause string comparison failures more quickly.
        /// </remarks>
        [NotNull]
        private string CreateKey([NotNull]params Type[] types)
        {
            int l = types.Length;
            // Most common case, use empty string to indicate no parameters.
            if (l < 1)
                return String.Empty;

            // For performance, this is a very common case, no need for stringbuilder.
            if (l == 1)
                return "1|" + types[0].FullName ?? types[0].Name;

            StringBuilder sb = new StringBuilder();
            sb.Append(types.Length);
            foreach(Type t in types)
            {
                sb.Append('|');
                sb.Append(t.FullName ?? t.Name);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the <see cref="Constructor"/> matching the parameter types.
        /// </summary>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Constructor"/>; otherwise <see langword="null"/> if not found.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public Constructor GetOverload([NotNull]params Type[] types)
        {
            Constructor constructor;
            return _constructors.TryGetValue(CreateKey(types), out constructor) ? constructor : null;
        }
    }
}