#region © Copyright Web Applications (UK) Ltd, 2017.  All rights reserved.
// Copyright (c) 2017, Web Applications UK Ltd
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

using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestEncodingHelper : UtilitiesTestBase
    {
        [NotNull]
        private static readonly Encoding _windows1252Encoding = Encoding.GetEncoding(1252);

        private static void AssertValidString(Encoding encoding)
        {
            string randomString = Tester.RandomString(encoding);
            Assert.IsTrue(
                randomString.IsValidEncoding(encoding),
                "The string should be valid {1}. {0}",
                randomString,
                encoding.WebName);
        }

        private static void AssertValidChars(Encoding encoding)
        {
            string randomString = Tester.RandomString(encoding);
            char[] randomChars = randomString.ToCharArray();
            Assert.IsTrue(
                randomChars.IsValidEncoding(encoding),
                "The character array should be valid {1}. {0}",
                randomString,
                encoding.WebName);
        }

        private static void AssertValidBytes(Encoding encoding)
        {
            string randomString = Tester.RandomString(encoding);
            byte[] randomBytes = encoding.GetBytes(randomString);
            Assert.IsTrue(
                randomBytes.IsValidEncoding(encoding),
                "The byte array should be valid {1}. {0}",
                randomString,
                encoding.WebName);
        }

        #region Valid String
        [TestMethod]
        public void TestIsValidAsciiString_ValidString()
        {
            string randomString = Tester.RandomString(unicode: false);
            Assert.IsTrue(randomString.IsValidAscii(), "The string should be valid ASCII. {0}", randomString);
        }

        [TestMethod]
        public void TestIsValidEncoding_ASCII_ValidString() => AssertValidString(Encoding.ASCII);

        [TestMethod]
        public void TestIsValidEncoding_1252_ValidString() => AssertValidString(_windows1252Encoding);

        [TestMethod]
        public void TestIsValidEncoding_UTF7_ValidString() => AssertValidString(Encoding.UTF7);

        [TestMethod]
        public void TestIsValidEncoding_UTF8_ValidString() => AssertValidString(Encoding.UTF8);

        [TestMethod]
        public void TestIsValidEncoding_UTF32_ValidString() => AssertValidString(Encoding.UTF32);

        [TestMethod]
        public void TestIsValidEncoding_Unicode_ValidString() => AssertValidString(Encoding.Unicode);
        #endregion

        #region Valid Chars
        [TestMethod]
        public void TestIsValidAsciiString_ValidChars()
        {
            string randomString = Tester.RandomString(unicode: false);
            char[] randomChars = randomString.ToCharArray();
            Assert.IsTrue(randomChars.IsValidAscii(), "The character array should be valid ASCII. {0}", randomString);
        }

        [TestMethod]
        public void TestIsValidEncoding_ASCII_ValidChars() => AssertValidChars(Encoding.ASCII);

        [TestMethod]
        public void TestIsValidEncoding_1252_ValidChars() => AssertValidChars(_windows1252Encoding);

        [TestMethod]
        public void TestIsValidEncoding_UTF7_ValidChars() => AssertValidChars(Encoding.UTF7);

        [TestMethod]
        public void TestIsValidEncoding_UTF8_ValidChars() => AssertValidChars(Encoding.UTF8);

        [TestMethod]
        public void TestIsValidEncoding_UTF32_ValidChars() => AssertValidChars(Encoding.UTF32);

        [TestMethod]
        public void TestIsValidEncoding_Unicode_ValidChars() => AssertValidChars(Encoding.Unicode);
        #endregion

        #region Valid Bytes
        [TestMethod]
        public void TestIsValidAsciiString_ValidBytes()
        {
            string randomString = Tester.RandomString(unicode: false);
            byte[] randomBytes = Encoding.ASCII.GetBytes(randomString);
            Assert.IsTrue(randomBytes.IsValidAscii(), "The character array should be valid ASCII. {0}", randomString);
        }

        [TestMethod]
        public void TestIsValidEncoding_ASCII_ValidBytes() => AssertValidBytes(Encoding.ASCII);

        [TestMethod]
        public void TestIsValidEncoding_1252_ValidBytes() => AssertValidBytes(_windows1252Encoding);

        [TestMethod]
        public void TestIsValidEncoding_UTF7_ValidBytes() => AssertValidBytes(Encoding.UTF7);

        [TestMethod]
        public void TestIsValidEncoding_UTF8_ValidBytes() => AssertValidBytes(Encoding.UTF8);

        [TestMethod]
        public void TestIsValidEncoding_UTF32_ValidBytes() => AssertValidBytes(Encoding.UTF32);

        [TestMethod]
        public void TestIsValidEncoding_Unicode_ValidBytes() => AssertValidBytes(Encoding.Unicode);
        #endregion

        private static void AssertInvalidString(Encoding encoding, string invalidStr)
        {
            string randomString = Tester.RandomString() + invalidStr;
            Assert.IsFalse(
                randomString.IsValidEncoding(encoding),
                "The string should be invalid {1}. {0}",
                randomString,
                encoding.WebName);
        }

        private static void AssertInvalidChars(Encoding encoding, string invalidStr)
        {
            string randomString = Tester.RandomString() + invalidStr;
            char[] randomChars = invalidStr.ToCharArray();
            Assert.IsFalse(
                randomChars.IsValidEncoding(encoding),
                "The character array should be invalid {1}. {0}",
                randomString,
                encoding.WebName);
        }

        private static void AssertInvalidBytes(Encoding encoding, params byte[] invalidBytes)
        {
            Assert.IsFalse(
                invalidBytes.IsValidEncoding(encoding),
                "The byte array should be invalid {1}. {0}",
                invalidBytes.ToHexLower(),
                encoding.WebName);
        }

        #region Invalid String
        [TestMethod]
        public void TestIsValidAsciiString_InvalidString()
        {
            string randomString = Tester.RandomString(unicode: false) + "\u00ff";
            Assert.IsFalse(randomString.IsValidAscii(), "The string should be invalid ASCII. {0}", randomString);
        }

        [TestMethod]
        public void TestIsValidEncoding_ASCII_InvalidString() => AssertInvalidString(Encoding.ASCII, "\u00ff");

        [TestMethod]
        public void TestIsValidEncoding_1252_InvalidString() => AssertInvalidString(
            _windows1252Encoding,
            "\uD800\uD800");

        // TODO Invalid UTF7?

        [TestMethod]
        public void TestIsValidEncoding_UTF8_InvalidString() => AssertInvalidString(Encoding.UTF8, "\uD800\uD800");

        [TestMethod]
        public void TestIsValidEncoding_UTF32_InvalidString() => AssertInvalidString(Encoding.UTF32, "\uD800\uD800");

        [TestMethod]
        public void TestIsValidEncoding_Unicode_InvalidString() =>
            AssertInvalidString(Encoding.Unicode, "\uD800\uD800");
        #endregion

        #region Invalid Chars
        [TestMethod]
        public void TestIsValidAsciiString_InvalidChars()
        {
            string randomString = Tester.RandomString(unicode: false) + "\u00ff";
            char[] randomChars = randomString.ToCharArray();
            Assert.IsFalse(
                randomChars.IsValidAscii(),
                "The character array should be invalid ASCII. {0}",
                randomString);
        }

        [TestMethod]
        public void TestIsValidEncoding_ASCII_InvalidChars() => AssertInvalidChars(Encoding.ASCII, "\u00ff");

        [TestMethod]
        public void TestIsValidEncoding_1252_InvalidChars() => AssertInvalidChars(_windows1252Encoding, "\uD800\uD800");

        // TODO Invalid UTF7?

        [TestMethod]
        public void TestIsValidEncoding_UTF8_InvalidChars() => AssertInvalidChars(Encoding.UTF8, "\uD800\uD800");

        [TestMethod]
        public void TestIsValidEncoding_UTF32_InvalidChars() => AssertInvalidChars(Encoding.UTF32, "\uD800\uD800");

        [TestMethod]
        public void TestIsValidEncoding_Unicode_InvalidChars() => AssertInvalidChars(Encoding.Unicode, "\uD800\uD800");
        #endregion

        #region Invalid Bytes
        [TestMethod]
        public void TestIsValidAsciiString_InvalidBytes()
        {
            byte[] invalidBytes = new byte[] { 255 };
            Assert.IsFalse(
                invalidBytes.IsValidAscii(),
                "The byte array should be invalid ASCII. {0}",
                invalidBytes.ToHexLower());
        }

        [TestMethod]
        public void TestIsValidEncoding_ASCII_InvalidBytes() => AssertInvalidBytes(Encoding.ASCII, 255);

        [TestMethod]
        public void TestIsValidEncoding_UTF7_InvalidBytes() =>
            AssertInvalidBytes(Encoding.UTF7, 0xd8, 0xd8, 0xd8, 0xd8);

        [TestMethod]
        public void TestIsValidEncoding_UTF8_InvalidBytes() =>
            AssertInvalidBytes(Encoding.UTF8, 0xd8, 0xd8, 0xd8, 0xd8);

        [TestMethod]
        public void TestIsValidEncoding_UTF32_InvalidBytes() => AssertInvalidBytes(Encoding.UTF32, 255, 255, 255, 255);

        [TestMethod]
        public void TestIsValidEncoding_Unicode_InvalidBytes() => AssertInvalidBytes(
            Encoding.Unicode,
            0xd8,
            0xd8,
            0xd8,
            0xd8);
        #endregion
    }
}