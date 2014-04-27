#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Ranges;

namespace WebApplications.Utilities.Test.Ranges
{
    [TestClass]
    public class RangeTests : UtilitiesTestBase
    {
        [TestMethod]
        public void Range_NumberBelowRange_BindReturnsStart()
        {
            int start = Random.Next(int.MinValue / 2, int.MaxValue / 2);
            int end = Random.Next(start, int.MaxValue / 2);
            int testValue = Random.Next(int.MinValue, start);

            Assert.AreEqual(
                start,
                (new Range<int>(start, end)).Bind(testValue),
                "Bind should return the lower bound of the range if the input is below the range");
        }

        [TestMethod]
        public void Range_NumberAboveRange_BindReturnsEnd()
        {
            int start = Random.Next(int.MinValue / 2, int.MaxValue / 2);
            int end = Random.Next(start, int.MaxValue / 2);
            int testValue = Random.Next(end, int.MaxValue);

            Assert.AreEqual(
                end,
                (new Range<int>(start, end)).Bind(testValue),
                "Bind should return the upper bound of the range if the input is above the range");
        }

        [TestMethod]
        public void Range_NumberWithinRange_BindReturnsInput()
        {
            int start = Random.Next(int.MinValue / 2, int.MaxValue / 2);
            int end = Random.Next(start, int.MaxValue / 2);
            int testValue = Random.Next(start, end);

            Assert.AreEqual(
                testValue,
                (new Range<int>(start, end)).Bind(testValue),
                "Bind should return the input if it is within the range");
        }

        [TestMethod]
        [ExpectedException(typeof (TypeInitializationException))]
        public void Range_IncompatibleTypes_ThrowsTypeInitializationException()
        {
            int start = Random.Next(int.MinValue / 2, int.MaxValue / 2);
            int end = Random.Next(start, int.MaxValue / 2);

            new Range<float, int>(start, end, 1);
        }
    }
}