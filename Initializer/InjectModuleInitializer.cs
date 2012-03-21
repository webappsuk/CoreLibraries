using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;

namespace WebApplications.Utilities.Initializer
{
    /// <summary>
    /// Build task for injecting a module initializer.
    /// </summary>
    /// <remarks></remarks>
    public class InjectModuleInitializer : Task
    {
        /// <summary>
        /// Gets or sets the assembly file.
        /// </summary>
        /// <value>The assembly file.</value>
        /// <remarks></remarks>
        [Required]
        public string AssemblyFile { get; set; }

        /// <summary>
        /// Gets or sets the module initializer's type name (Defaults to 'ModuleIntiliazer').
        /// </summary>
        /// <value>The module initializer's type name.</value>
        /// <remarks></remarks>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the module initializer's method name (Defaults to 'Initialize').
        /// </summary>
        /// <value>The module initializer's method name.</value>
        /// <remarks></remarks>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the instrumentation should be done in the current <see cref="AppDomain"/>.
        /// </summary>
        /// <value><c>true</c> if a new <see cref="AppDomain"/> should be used; otherwise, <c>false</c> to perform the injection in the current <see cref="AppDomain"/>.</value>
        /// <remarks></remarks>
        public bool UseIsolatedAppDomain { get; set; }

        /// <summary>
        /// Gets or sets the strong name key pair (if any).
        /// </summary>
        /// <value>The strong name key pair.</value>
        /// <remarks></remarks>
        public string StrongNameKeyPair { get; set; }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>true if the task successfully executed; otherwise, false.</returns>
        /// <remarks></remarks>
        public override bool Execute()
        {
            Contract.Assert(Log != null);
            bool hasErrors = false;
            foreach (Output output in Injector.Inject(AssemblyFile, TypeName, MethodName, StrongNameKeyPair, UseIsolatedAppDomain))
            {
                switch (output.Importance)
                {
                    case OutputImportance.Error:
                        Log.LogError(output.Message);
                        break;
                    case OutputImportance.Warning:
                        Log.LogWarning(output.Message);
                        break;
                    case OutputImportance.MessageHigh:
                        Log.LogMessage(MessageImportance.High, output.Message);
                        break;
                    case OutputImportance.MessageNormal:
                        Log.LogMessage(MessageImportance.Normal, output.Message);
                        break;
                    case OutputImportance.MessageLow:
                        Log.LogMessage(MessageImportance.Low, output.Message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return !hasErrors;
        }
    }
}
