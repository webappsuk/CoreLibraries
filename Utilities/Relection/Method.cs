using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Holds information about a particular method.
    /// </summary>
    public class Method : Overloadable<MethodInfo>
    {
        internal Method([NotNull] MethodInfo method) : base(method)
        {
        }
    }
}