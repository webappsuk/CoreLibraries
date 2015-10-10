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

using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Cryptography.Test
{
    [TestClass]
    public class PBKDF2Tests
    {
        /// <summary>
        /// Tests that <see cref="PBKDF2{T}"/> using the <see cref="HMACSHA1"/> algorithm produces the same bytes as the built in <see cref="Rfc2898DeriveBytes"/> class.
        /// </summary>
        [TestMethod]
        public void TestRfc2898DeriveBytes()
        {
            byte[] password = new byte[20];
            byte[] salt = new byte[20];
            int bytes = 64;

            for (int iterations = 1000; iterations <= 10000; iterations += 1000)
            {
                Tester.RandomGenerator.NextBytes(password);
                Tester.RandomGenerator.NextBytes(salt);

                using (PBKDF2<HMACSHA1> pbkdf2 = new PBKDF2<HMACSHA1>(password, salt, iterations))
                using (Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(password, salt, iterations))
                {
                    byte[] pbkdf2Bytes = pbkdf2.GetBytes(bytes);
                    byte[] rfc2898Bytes = rfc2898.GetBytes(bytes);

                    Assert.AreEqual(bytes, rfc2898Bytes.Length);
                    Assert.AreEqual(bytes, pbkdf2Bytes.Length);

                    for (int i = 0; i < bytes; i++)
                        Assert.AreEqual(rfc2898Bytes[i], pbkdf2Bytes[i]);
                }
            }
        }
    }
}