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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using WebApplications.Testing;

namespace WebApplications.Utilities.Cryptography.Test
{
    [TestClass]
    public class RSACryptographyProviderTests
    {
        public void TestEncryptDecrypt(int keySize, Func<int, int, int> getLengthFunc, int blocks, bool preserveLength = true)
        {
            RSACryptographyProvider provider = RSACryptographyProvider.Create(keySize);
            Assert.IsTrue(provider.CanEncrypt);
            Trace.WriteLine($"Input Block Size:{provider.InputBlockSize}; OutputBlockSize:{provider.OutputBlockSize}");

            // Generate random input.            
            int length = getLengthFunc(provider.InputBlockSize, provider.OutputBlockSize);

            byte[] input = new byte[length];
            if (length > 0)
                Tester.RandomGenerator.NextBytes(input);

            byte[] encryptedBytes = provider.Encrypt(input, preserveLength);
            Assert.IsNotNull(encryptedBytes);

            // Check we got the correct number of blocks.
            Assert.AreEqual(0, encryptedBytes.LongLength % provider.OutputBlockSize);
            Assert.AreEqual(blocks, encryptedBytes.LongLength / provider.OutputBlockSize);

            Assert.IsTrue(provider.CanDecrypt);
            byte[] output = provider.Decrypt(encryptedBytes, preserveLength);
            Assert.IsNotNull(output);
            Assert.AreEqual(length, output.LongLength);
            CollectionAssert.AreEqual(input, output);
        }

        [TestMethod]
        public void Test_Null_1024()
        {
            RSACryptographyProvider provider = RSACryptographyProvider.Create(1024);
            Assert.IsTrue(provider.CanEncrypt);
            Assert.IsNull(provider.Encrypt((byte[])null));
            Assert.IsTrue(provider.CanDecrypt);
            Assert.IsNull(provider.Decrypt(null));
        }

        [TestMethod]
        public void Test_Zero_Length_1024()
        {
            TestEncryptDecrypt(1024, (i, o) => 0, 1);
        }

        [TestMethod]
        public void Test_Less_Than_One_Block_1024()
        {
            TestEncryptDecrypt(1024, (i, o) => i - 2, 1);
        }

        [TestMethod]
        public void Test_Exactly_One_Block_1024()
        {
            TestEncryptDecrypt(1024, (i, o) => i - 1, 1);
        }

        [TestMethod]
        public void Test_Just_Two_Blocks_1024()
        {
            TestEncryptDecrypt(1024, (i, o) => i, 2);
        }

        [TestMethod]
        public void Test_Null_2048()
        {
            RSACryptographyProvider provider = RSACryptographyProvider.Create(2048);
            Assert.IsTrue(provider.CanEncrypt);
            Assert.IsNull(provider.Encrypt((byte[])null));
            Assert.IsTrue(provider.CanDecrypt);
            Assert.IsNull(provider.Decrypt(null));
        }

        [TestMethod]
        public void Test_Zero_Length_2048()
        {
            TestEncryptDecrypt(2048, (i, o) => 0, 1);
        }

        [TestMethod]
        public void Test_Less_Than_One_Block_2048()
        {
            TestEncryptDecrypt(2048, (i, o) => i - 2, 1);
        }

        [TestMethod]
        public void Test_Exactly_One_Block_2048()
        {
            TestEncryptDecrypt(2048, (i, o) => i - 1, 1);
        }

        [TestMethod]
        public void Test_Just_Two_Blocks_2048()
        {
            TestEncryptDecrypt(2048, (i, o) => i, 2);
        }
    }
}