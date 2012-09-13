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
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestHashes
    {
        private const string CharacterPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvw xyz1234567890";

        [TestMethod]
        public void TestHashing()
        {
            string testString = GetType().ToString();
            string s1 = GetHash(new SHA1CryptoServiceProvider(), testString);
            string s2 = GetHash(new SHA1Cng(), testString);
            string s3 = GetHash(new SHA1Managed(), testString);
            Assert.AreEqual(s1, s2);
            Assert.AreEqual(s1, s3);

            const int testCount = 100;
            for (int a = 0; a < 10; a++)
            {
                TestHash(new SHA384CryptoServiceProvider(), testCount, a);
                TestHash(new SHA384Cng(), testCount, a);
                TestHash(new SHA384Managed(), testCount, a);
                Trace.WriteLine("");
            }
            /*
            this.TestHash(new SHA1CryptoServiceProvider(), testCount);
            this.TestHash(new SHA1Cng(), testCount);
            this.TestHash(new SHA1Managed(), testCount);

            this.TestHash(new SHA256CryptoServiceProvider(), testCount);
            this.TestHash(new SHA256Cng(), testCount);
            this.TestHash(new SHA256Managed(), testCount);

            this.TestHash(new SHA384CryptoServiceProvider(), testCount);
            this.TestHash(new SHA384Cng(), testCount);
            this.TestHash(new SHA384Managed(), testCount);

            this.TestHash(new SHA512CryptoServiceProvider(), testCount);
            this.TestHash(new SHA512Cng(), testCount);
            this.TestHash(new SHA512Managed(), testCount);
             */
        }

        private static void TestHash(HashAlgorithm hashAlgorithm, int count, int size)
        {
            // Create test string of relevant length.
            Random random = new Random();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < count; i++)
            {
                StringBuilder sb = new StringBuilder();
                for (int a = 0; a < size; a++)
                    sb.Append(CharacterPool[(int) (random.NextDouble()*CharacterPool.Length)]);

                string testString = sb.ToString();
                GetHash(hashAlgorithm, testString);
            }
            stopwatch.Stop();
            Trace.WriteLine(
                string.Format(
                    "{1}ms\t{2} chars x {3}\t{0}",
                    hashAlgorithm.GetType(),
                    stopwatch.ElapsedTicks*1000/Stopwatch.Frequency,
                    size,
                    count));
        }

        private void TestIntelligentHash(
            HashAlgorithm hashAlgorithm, HashAlgorithm hashAlgorithm2, int count, int size, int breakPoint)
        {
            // Create test string of relevant length.
            Random random = new Random();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < count; i++)
            {
                StringBuilder sb = new StringBuilder();
                for (int a = 0; a < size; a++)
                    sb.Append(CharacterPool[(int) (random.NextDouble()*CharacterPool.Length)]);

                string testString = sb.ToString();
                string hash = GetHash(testString.Length < breakPoint ? hashAlgorithm : hashAlgorithm2, testString);
            }
            stopwatch.Stop();
            Trace.WriteLine(
                string.Format(
                    "{1}ms\t{2} chars x {3}\t{0}",
                    "INTELLIGENT",
                    stopwatch.ElapsedTicks*1000/Stopwatch.Frequency,
                    size,
                    count));
        }

        private void TestHashConstruction(int count, int size)
        {
            // Create test string of relevant length.
            Random random = new Random();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < count; i++)
            {
                StringBuilder sb = new StringBuilder();
                for (int a = 0; a < size; a++)
                    sb.Append(CharacterPool[(int) (random.NextDouble()*CharacterPool.Length)]);

                string testString = sb.ToString();
                string hash = GetHash(new SHA1Managed(), testString);
            }
            stopwatch.Stop();
            Trace.WriteLine(
                string.Format(
                    "{1}ms\t{2} chars x {3}\t{0}",
                    "SHA1Managed constructed",
                    stopwatch.ElapsedTicks*1000/Stopwatch.Frequency,
                    size,
                    count));
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = Encoding.Unicode.GetBytes(input);
            byte[] output = hashAlgorithm.ComputeHash(data);
            return Convert.ToBase64String(output);
        }
    }
}