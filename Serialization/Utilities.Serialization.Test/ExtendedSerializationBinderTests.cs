#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Serialization;

namespace WebApplications.Utilities.Test.Serialization
{
    [TestClass]
    public class ExtendedSerializationBinderTests
    {
        private static void GetTestTypeAndAssemblyName(out String typeName, out String assemblyName)
        {
            String[] assemblyQualifiedNamePieces = typeof (TestType).AssemblyQualifiedName.Split(new[] {','}, 2);
            typeName = assemblyQualifiedNamePieces.First();
            assemblyName = assemblyQualifiedNamePieces.Last();
        }

        [TestMethod]
        public void Default_IsNotNull()
        {
            ExtendedSerializationBinder Default = ExtendedSerializationBinder.Default;
            Assert.IsNotNull(Default);
        }

        [TestMethod]
        public void BindToType_UsingOnlyTypeName_ReturnsCorrectType()
        {
            Type result = ExtendedSerializationBinder.Default.BindToType("", typeof (TestType).Name);
            Assert.AreEqual(typeof (TestType), result);
        }

        [TestMethod]
        public void BindToType_UsingFullyQualifiedName_ReturnsCorrectType()
        {
            String typeName, assemblyName;
            GetTestTypeAndAssemblyName(out typeName, out assemblyName);
            Type result = ExtendedSerializationBinder.Default.BindToType(assemblyName, typeName);
            Assert.AreEqual(typeof (TestType), result);
        }

        [TestMethod]
        public void BindToType_UsingSimpleQualifiedName_ReturnsCorrectType()
        {
            String typeName, assemblyName;
            GetTestTypeAndAssemblyName(out typeName, out assemblyName);
            String simpleAssemblyName = assemblyName.Split(',').First();
            Type result = ExtendedSerializationBinder.Default.BindToType(simpleAssemblyName, typeName);
            Assert.AreEqual(typeof (TestType), result);
        }

        [TestMethod]
        public void BindToType_UsingFullyQualifiedNameOfOldVersion_ReturnsCurrentVersionCorrectType()
        {
            String typeName, assemblyName;
            GetTestTypeAndAssemblyName(out typeName, out assemblyName);
            String simpleAssemblyName = assemblyName.Split(',').First();
            String oldVersionAssemblyName = String.Format("{0}, version=0.0", simpleAssemblyName);
            Type result = ExtendedSerializationBinder.Default.BindToType(oldVersionAssemblyName, typeName);
            Assert.AreEqual(typeof (TestType), result);
        }

        [TestMethod]
        public void BindToType_InvalidAssemblyName_ReturnsNull()
        {
            String typeName, assemblyName;
            GetTestTypeAndAssemblyName(out typeName, out assemblyName);
            String invalidAssemblyName = String.Format("NonExistantAssemblyName.{0}", assemblyName.Split(',').First());
            Type result = ExtendedSerializationBinder.Default.BindToType(invalidAssemblyName, typeName);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void BindToType_InvalidTypeName_ReturnsNull()
        {
            String typeName, assemblyName;
            GetTestTypeAndAssemblyName(out typeName, out assemblyName);
            Type result = ExtendedSerializationBinder.Default.BindToType(assemblyName, "NoTypeExistsByThisName");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void BindToType_AfterAddingMappingUsingMapType_ReturnsMappedType()
        {
            const string assemblyName = "testAssembly";
                // This is not a real assembly, as a real one is needed and using a false one allows better test isolation.
            const string typeName = "AfterAddingMappingUsingMapType";
            Type boundType = typeof (int);
            ExtendedSerializationBinder.MapType(assemblyName, typeName, boundType);
            Type result = ExtendedSerializationBinder.Default.BindToType(assemblyName, typeName);
            Assert.AreEqual(boundType, result);
        }

        [TestMethod]
        public void BindToType_AfterAddingMappingUsingTypeSafeMapType_ReturnsMappedType()
        {
            const string assemblyName = "testAssembly";
                // This is not a real assembly, as a real one is needed and using a false one allows better test isolation.
            const string typeName = "AfterAddingMappingUsingTypeSafeMapType";
            ExtendedSerializationBinder.MapType<String>(assemblyName, typeName);
            Type result = ExtendedSerializationBinder.Default.BindToType(assemblyName, typeName);
            Assert.AreEqual(typeof (String), result);
        }

        [TestMethod]
        public void BindToType_AfterAddingThenUpdatingMappingUsingMapType_ReturnsUpdatedMappedType()
        {
            const string assemblyName = "testAssembly";
                // This is not a real assembly, as a real one is needed and using a false one allows better test isolation.
            const string typeName = "AfterAddingThenUpdatingMappingUsingMapType";
            ExtendedSerializationBinder.MapType(assemblyName, typeName, typeof (String));
            Type boundType = typeof (int);
            ExtendedSerializationBinder.MapType(assemblyName, typeName, boundType);
            Type result = ExtendedSerializationBinder.Default.BindToType(assemblyName, typeName);
            Assert.AreEqual(boundType, result);
        }

        #region Nested type: TestType
        private class TestType
        {
        }
        #endregion
    }
}