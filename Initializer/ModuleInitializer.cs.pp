using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace $rootnamespace$
{
    /// <summary>
    /// This type cannot be private, as it must be callable from the actual Module Initializer.
    /// </summary>
    /// <remarks><para>If you define the 'ModuleInitializer' constant as part of the build then the resulting assembly is rewritten so that
    /// the <see cref="ModuleInitializer"/> method is called when the enclosing module is first loaded.</para></remarks>
    internal static class ModuleInitializer
    {
        /// <summary>
        /// This method must not be private and must be static.  Any return value is ignored.
        /// </summary>
        /// <remarks>
        /// <para>Include initialization code here that will run when the library is first loaded,
        /// and before any element of the library is used.</para>
        /// </remarks>
        internal static void Initialize()
        {
        }
    }
}
