#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: TestHashing.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
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