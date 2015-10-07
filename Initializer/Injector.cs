#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using WebApplications.Utilities.Annotations;
using MethodAttributes = Mono.Cecil.MethodAttributes;

namespace WebApplications.Utilities.Initializer
{
    /// <summary>
    /// Used to inject a module initializer into an assembly.
    /// </summary>
    /// <remarks>
    /// <para>Creates a seperate domain in which to load and inject the assembly, to allow assembly unloading and prevent file locks.</para>
    /// </remarks>
    internal class Injector : MarshalByRefObject
    {
        /// <summary>
        /// The injector assembly.
        /// </summary>
        [NotNull]
        private static readonly string _assemblyFileName;

        /// <summary>
        /// The injector type.
        /// </summary>
        [NotNull]
        private static readonly string _typeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        static Injector()
        {
            Type injectorType = typeof(Injector);
            _assemblyFileName = injectorType.Assembly.Location;
            // ReSharper disable once AssignNullToNotNullAttribute
            _typeName = injectorType.FullName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.MarshalByRefObject"/> class.
        /// </summary>
        /// <remarks>This can only be done by the static <see cref="Inject"/> method.</remarks>
        private Injector()
        {
        }

        /// <summary>
        /// Injects the module initializer into the specified assembly file.
        /// </summary>
        /// <param name="assemblyFile">The assembly file.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="strongNameKeyPair">The strong name key pair.</param>
        /// <param name="useIsolatedAppDomain">if set to <c>true</c> uses a new <see cref="AppDomain"/>.</param>
        /// <returns>Any errors; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<Output> Inject(
            string assemblyFile,
            string typeName,
            string methodName,
            string strongNameKeyPair,
            bool useIsolatedAppDomain)
        {
            Injector injector;
            if (!useIsolatedAppDomain)
            {
                injector = new Injector();
                return injector.DoInject(assemblyFile, typeName, methodName, strongNameKeyPair);
            }

            AppDomain childDomain = null;
            try
            {
                // Create a child app domain.
                childDomain = AppDomain.CreateDomain("InjectionDomain");

                // Create an instance of the injector object in the new domain.
                injector = (Injector)childDomain.CreateInstanceFromAndUnwrap(
                    _assemblyFileName,
                    _typeName,
                    false,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                    null,
                    null,
                    null,
                    null);
                Debug.Assert(injector != null);

                // Call the DoInject method on the injector in the child domain.
                return injector.DoInject(assemblyFile, typeName, methodName, strongNameKeyPair);
            }
            finally
            {
                if (childDomain != null)
                    AppDomain.Unload(childDomain);
            }
        }

        /// <summary>
        /// Does the injection in an isolated app domain.
        /// </summary>
        /// <param name="assemblyFile">The assembly file.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="strongNameKeyPair">The strong name key pair.</param>
        /// <returns>Any errors; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        [SecuritySafeCritical]
        [NotNull]
        [ItemNotNull]
        private IEnumerable<Output> DoInject(
            string assemblyFile,
            string typeName,
            string methodName,
            string strongNameKeyPair)
        {
            OutputCollection outputCollection = new OutputCollection();
            try
            {
                if (String.IsNullOrWhiteSpace(assemblyFile))
                {
                    outputCollection.Add(OutputImportance.Error, "Must specify a valid assembly.");
                    return outputCollection;
                }

                if (String.IsNullOrWhiteSpace(typeName))
                    typeName = "ModuleInitializer";
                if (String.IsNullOrWhiteSpace(methodName))
                    methodName = "Initialize";

                StrongNameKeyPair snkpair;
                if (!String.IsNullOrWhiteSpace(strongNameKeyPair))
                {
                    if (!File.Exists(strongNameKeyPair))
                    {
                        outputCollection.Add(
                            OutputImportance.Error,
                            "The '{0}' strong name keypair was not found.",
                            strongNameKeyPair);
                        return outputCollection;
                    }

                    // Accessing public key requires UnmanagedCode security permission.
                    try
                    {
                        SecurityPermission sp = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
                        sp.Demand();
                    }
                    catch (Exception e)
                    {
                        outputCollection.Add(
                            OutputImportance.Error,
                            "Could not instrument '{0}' as cannot resign assembly, UnmanagedCode security permission denied.",
                            strongNameKeyPair,
                            e.Message);
                        return outputCollection;
                    }

                    try
                    {
                        /*
                         * Turns out that if we get the strong name key pair directly using the StrongNameKeyPair(string filename)
                         * constructor overload, then retrieving the public key fails due to file permissions.
                         * Opening the filestream ourselves with read access, and reading the snk that way works, and allows
                         * us to successfully resign (who knew?).
                         */
                        using (FileStream fs = new FileStream(strongNameKeyPair, FileMode.Open, FileAccess.Read))
                            snkpair = new StrongNameKeyPair(fs);

                        // Ensure we can access public key - this will be done by mono later so check works now.
                        // ReSharper disable once UnusedVariable
                        byte[] publicKey = snkpair.PublicKey;
                    }
                    catch (Exception e)
                    {
                        outputCollection.Add(
                            OutputImportance.Error,
                            "Error occurred whilst accessing public key from '{0}' strong name keypair. {1}",
                            strongNameKeyPair,
                            e.Message);
                        return outputCollection;
                    }
                }
                else
                    // No resigning necessary.
                    snkpair = null;

                if (!File.Exists(assemblyFile))
                {
                    outputCollection.Add(OutputImportance.Error, "The '{0}' assembly was not found.", assemblyFile);
                    return outputCollection;
                }

                // Look for PDB file.
                string pdbFile = Path.ChangeExtension(assemblyFile, ".pdb");

                // Read the assembly definition
                ReaderParameters readParams = new ReaderParameters(ReadingMode.Immediate);
                bool hasPdb = false;
                if (File.Exists(pdbFile))
                {
                    readParams.ReadSymbols = true;
                    readParams.SymbolReaderProvider = new PdbReaderProvider();
                    hasPdb = true;
                }
                AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyFile, readParams);
                if (assembly == null)
                {
                    outputCollection.Add(
                        OutputImportance.Error,
                        "Failed to load assembly definition for '{0}'.",
                        assemblyFile);
                    return outputCollection;
                }

                // Find the main module.
                ModuleDefinition module = assembly.MainModule;
                if (module == null)
                {
                    outputCollection.Add(
                        OutputImportance.Error,
                        "Failed to load main module definition from assembly '{0}'.",
                        assemblyFile);
                    return outputCollection;
                }

                if (module.Types == null)
                {
                    outputCollection.Add(
                        OutputImportance.Error,
                        "Failed to load main module types from assembly '{0}'.",
                        assemblyFile);
                    return outputCollection;
                }

                // Find the <Module> type definition
                // ReSharper disable once PossibleNullReferenceException
                TypeDefinition moduleType = module.Types.SingleOrDefault(t => t.Name == "<Module>");
                if (moduleType == null)
                {
                    outputCollection.Add(
                        OutputImportance.Error,
                        "Could not find type '<Module>' in assembly '{0}'.",
                        assemblyFile);
                    return outputCollection;
                }

                // Find void type
                // ReSharper disable once PossibleNullReferenceException
                TypeReference voidRef = module.TypeSystem.Void;
                if (voidRef == null)
                {
                    outputCollection.Add(
                        OutputImportance.Error,
                        "Could not find type 'void' in assembly '{0}'.",
                        assemblyFile);
                    return outputCollection;
                }

                // Find the type definition
                // ReSharper disable once PossibleNullReferenceException
                TypeDefinition typeDefinition = module.Types.SingleOrDefault(t => t.Name == typeName);

                if (typeDefinition == null)
                {
                    outputCollection.Add(
                        OutputImportance.Warning,
                        "Could not find type '{0}' in assembly '{1}'.",
                        typeName,
                        assemblyFile);
                    return outputCollection;
                }

                // Find the method
                MethodDefinition callee = typeDefinition.Methods != null
                    ? typeDefinition.Methods.FirstOrDefault(
                    // ReSharper disable PossibleNullReferenceException
                        m => m.Name == methodName && m.Parameters.Count == 0)
                    // ReSharper restore PossibleNullReferenceException
                    : null;

                if (callee == null)
                {
                    outputCollection.Add(
                        OutputImportance.Warning,
                        "Could not find method '{0}' with no parameters in type '{1}' in assembly '{2}'.",
                        methodName,
                        typeName,
                        assemblyFile);
                    return outputCollection;
                }

                if (callee.IsPrivate)
                {
                    outputCollection.Add(
                        OutputImportance.Error,
                        "Method '{0}' in type '{1}' in assembly '{2}' cannot be private as it can't be accessed by the Module Initializer.",
                        methodName,
                        typeName,
                        assemblyFile);
                    return outputCollection;
                }

                if (!callee.IsStatic)
                {
                    outputCollection.Add(
                        OutputImportance.Error,
                        "Method '{0}' in type '{1}' in assembly '{2}' cannot be an instance method as it can't be instantiated by the Module Initializer.",
                        methodName,
                        typeName,
                        assemblyFile);
                    return outputCollection;
                }


                outputCollection.Add(
                    OutputImportance.MessageHigh,
                    "Method '{0}' in type '{1}' in assembly '{2}' will be called during Module initialization.",
                    methodName,
                    typeName,
                    assemblyFile);

                // Create the module initializer.
                MethodDefinition cctor = new MethodDefinition(
                    ".cctor",
                    MethodAttributes.Static
                    | MethodAttributes.SpecialName
                    | MethodAttributes.RTSpecialName,
                    voidRef);
                // ReSharper disable PossibleNullReferenceException
                ILProcessor il = cctor.Body.GetILProcessor();
                il.Append(il.Create(OpCodes.Call, callee));
                il.Append(il.Create(OpCodes.Ret));
                moduleType.Methods.Add(cctor);
                // ReSharper restore PossibleNullReferenceException

                WriterParameters writeParams = new WriterParameters();
                if (hasPdb)
                {
                    writeParams.WriteSymbols = true;
                    writeParams.SymbolWriterProvider = new PdbWriterProvider();
                }
                if (snkpair != null)
                {
                    writeParams.StrongNameKeyPair = snkpair;
                    outputCollection.Add(
                        OutputImportance.MessageHigh,
                        "Assembly '{0}' is being resigned by '{1}'.",
                        assemblyFile,
                        strongNameKeyPair);
                }
                else
                {
                    outputCollection.Add(
                        OutputImportance.MessageHigh,
                        "Assembly '{0}' will not be resigned.",
                        assemblyFile);
                }

                assembly.Write(assemblyFile, writeParams);
                return outputCollection;
            }
            catch (Exception ex)
            {
                outputCollection.Add(OutputImportance.Error, "An unexpected error occurred. {0}", ex.Message);
                return outputCollection;
            }
        }
    }
}