using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   This type cannot be private, as it must be callable from the actual Module Initializer.
    /// </summary>
    [UsedImplicitly]
    internal static class ModuleInitializer
    {
        /// <summary>
        ///   This method must not be private and must be <see langword="static"/>. 
        ///   Any return value is ignored.
        /// </summary>
        /// <remarks>
        ///   Include initialization code here that will run when the library is first loaded,
        ///   and before any element of the library is used.
        /// </remarks>
        [UsedImplicitly]
        internal static void Initialize()
        {
            Log.LoadConfiguration();
        }
    }
}
