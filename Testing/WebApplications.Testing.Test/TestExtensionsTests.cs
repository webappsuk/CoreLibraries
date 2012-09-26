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
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Testing.Test
{
    [TestClass]
    public class TestExtensionsTests : TestBase
    {
        [TestMethod]
        public void Stopwatch_ToString_ReturnsCorrectFormat()
        {
            Stopwatch s = new Stopwatch();
            Random random = Tester.RandomGenerator;
            s.Start();
            Thread.Sleep(random.Next(0, 1000));
            s.Stop();
            string randomString = random.RandomString();
            Assert.AreEqual(
                String.Format("Test stopwatch {0} completed in {1}ms.", randomString,
                              (s.ElapsedTicks*1000M)/Stopwatch.Frequency),
                s.ToString("Test stopwatch {0}", randomString));
        }
    }

    [TestClass]
    public class Base64Tests
    {
        [TestMethod]
        public void TestDecode()
        {
            string encodedString = "1YHQKTSVD7EKN2FRRA2AU4CR94";
            string decodedString = Base64Encoder.Decode(encodedString);
            Trace.WriteLine(decodedString);
        }
    }

    public class Base64Encoder
    {
        public static string Encode(params object[] input)
        {
            try
            {
                byte[] encodedKeyByte = Encoding.UTF8.GetBytes(input.ToString());
                string encodedKey = Convert.ToBase64String(encodedKeyByte);
                return encodedKey;
            }
            catch
            {
                throw new Exception("Unable to encode");
            }
        }

        public static string Decode(string input)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            Decoder utf8Decode = encoder.GetDecoder();

            byte[] toDecodeByte = Convert.FromBase64String(input);
            int charCount = utf8Decode.GetCharCount(toDecodeByte, 0, toDecodeByte.Length);
            char[] decodedChar = new char[charCount];
            utf8Decode.GetChars(toDecodeByte, 0, toDecodeByte.Length, decodedChar, 0);
            return new string(decodedChar);
        }
    }
}