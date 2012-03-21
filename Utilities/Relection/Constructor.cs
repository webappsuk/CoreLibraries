using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Holds information about a particular constructor.
    /// </summary>
    public class Constructor : Overloadable<ConstructorInfo>
    {
        internal Constructor([NotNull] ConstructorInfo method) : base(method)
        {
        }
    }
}