using System;
using System.Threading;
namespace WebApplications.Utilities.Initializer.TestApp
{
    /// <summary>
    /// This type cannot be private, as it must be callable from the actual Module Initializer.
    /// </summary>
    /// <remarks></remarks>
    internal static class ModuleInitializer
    {
        /// <summary>
        /// Set when initializer hit.
        /// </summary>
        internal static DateTime InitializerHit;

        internal static DateTime StaticConstructorHit;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        static ModuleInitializer()
        {
            Thread.Sleep(50);
            StaticConstructorHit = DateTime.Now;
            Thread.Sleep(50);
        }

        /// <summary>
        /// This method must not be private and must be static.  Any return value is ignored.
        /// </summary>
        /// <remarks>
        /// <para>Include initialization code here that will run when the library is first loaded,
        /// and before any element of the library is used.</para>
        /// </remarks>
        internal static void Initialize()
        {
            Thread.Sleep(50);
            InitializerHit = DateTime.Now;
            Thread.Sleep(50);
        }
    }
}
