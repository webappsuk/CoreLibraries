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

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("WebApplications.Testing")]
[assembly: AssemblyDescription("Standard Web Applications .NET testing utilities.")]
[assembly: ComVisible(false)]
[assembly: Guid("d62c1e9f-6857-4be9-b1ce-3ea218c90e7d")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCompany("Web Applications UK Ltd")]
[assembly: AssemblyProduct("Web Applications Testing")]
[assembly: AssemblyCopyright("Copyright © Web Applications UK Ltd, 2006-2015")]
[assembly: AssemblyTrademark("Web Applications")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("5.1.*")]
#if SIGNED
[assembly: InternalsVisibleTo("WebApplications.Testing.Test, PublicKey=" +
                              "002400000480000094000000060200000024000052534131000400000100010009234d0a978c01" +
                              "fec4232555decda613950e25b0d9d9f0400cb4ee0ae0c21921a96bbc5c82d60e4b928eb392503a" +
                              "5c24c81495256bf77e0cb80244c7d253a6ab396da91ead9b624ae7e53e03fba409507cc029ae8e" +
                              "a874da063a8c86c92fcc00055951e6c9c2e5adaad2e1ba02c044683691e2d60e6ee4a43d73f78a" +
                              "fb026eba")]
#else
[assembly: InternalsVisibleTo("WebApplications.Testing.Test")]
#endif