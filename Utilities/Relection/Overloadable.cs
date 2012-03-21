using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Holds information for a group of overloaded methods.
    /// </summary>
    /// <typeparam name="T">The overloadable type.</typeparam>
    public abstract class Overloadable<T> where T : MethodBase
    {
        /// <summary>
        ///   It is possible to have overloads with ref/out parameters,
        ///   hence type position is not always enough to disambiguate.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, List<Information>> _methods = new Dictionary<string,List<Information>>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="Overloadable&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="method">The method to add.</param>
        internal Overloadable([NotNull]T method)
        {
            Add(method);
        }

        /// <summary>
        ///   Adds the specified method.
        /// </summary>
        /// <param name="method">The method to add.</param>
        internal void Add([NotNull]T method)
        {
            List<ParameterInfo> parameters = method.GetParameters().ToList();
            string key = CreateKey(parameters.Select(p => p.ParameterType).ToArray());
            List<Information> methods;
            if (!_methods.TryGetValue(key, out methods))
            {
                methods = new List<Information>();
                _methods.Add(key, methods);
            }
            methods.Add(new Information(method, parameters));
        }

        /// <summary>
        ///   Creates a key for a signature.
        /// </summary>
        /// <param name="types">The parameter types.</param>
        /// <remarks>
        ///   We prepend the <paramref name="types"/> <see cref="System.Array.Length">count</see>
        ///   to the key as this will cause string comparison failures more quickly.
        /// </remarks>
        [NotNull]
        private string CreateKey(params Type[] types)
        {
            int l;
            // Most common case, use empty string to indicate no parameters.
            if ((types == null) || ((l = types.Length) < 1))
                return String.Empty;

            // For performance, this is a very common case, no need for stringbuilder.
            if (l == 1)
                return "1|" + types[0].FullName;

            StringBuilder sb = new StringBuilder();
            sb.Append(types.Length);
            foreach(Type t in types)
            {
                sb.Append('|');
                sb.Append(t.FullName);
            }
            return sb.ToString();
        }

        /// <summary>
        ///   Wraps the method information with accessors for retrieving parameters.
        /// </summary>
        public class Information
        {
            /// <summary>
            ///   The method info.
            /// </summary>
            public readonly T Info;

            /// <summary>
            ///   The parameters.
            /// </summary>
            public readonly IEnumerable<ParameterInfo> Parameters;

            /// <summary>
            ///   Initializes the <see cref="Information"/> class.
            /// </summary>
            /// <param name="info">The method.</param>
            /// <param name="parameters">The parameters.</param>
            internal Information(T info, IEnumerable<ParameterInfo> parameters)
            {
                Info = info;
                Parameters = parameters;
            }
        }
    }
}