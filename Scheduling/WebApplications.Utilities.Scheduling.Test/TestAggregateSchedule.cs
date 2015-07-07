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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using WebApplications.Testing;
using WebApplications.Utilities.Scheduling.Schedules;

namespace WebApplications.Utilities.Scheduling.Test
{
    [TestClass]
    public class TestAggregateSchedule : TestBase
    {
        [TestMethod]
        public void EmptyAggregateSchedule()
        {
            string name = Tester.RandomGenerator.RandomString();
            AggregateSchedule aggregateSchedule = new AggregateSchedule(name);
            Assert.AreEqual(name, aggregateSchedule.Name);
            Assert.AreEqual(Instant.MaxValue, aggregateSchedule.Next(TimeHelpers.Clock.Now));
            Assert.AreEqual(0, aggregateSchedule.Count());
        }

        [TestMethod]
        public void SingleAggregateSchedule()
        {
            Instant i = new Instant(Tester.RandomGenerator.RandomInt32());
            OneOffSchedule oneOffSchedule = new OneOffSchedule(i);
            AggregateSchedule aggregateSchedule = new AggregateSchedule(oneOffSchedule);
            Assert.AreEqual(1, aggregateSchedule.Count());
            Assert.AreEqual(i, aggregateSchedule.Next(i - TimeHelpers.OneSecond));
            Assert.AreEqual(i, aggregateSchedule.Next(i));
            Assert.AreEqual(Instant.MaxValue, aggregateSchedule.Next(i + Duration.FromTicks(1)));
        }

        [TestMethod]
        public void DoubleAggregateSchedule()
        {
            Instant i = new Instant(Tester.RandomGenerator.RandomInt32());
            Instant j = i + TimeHelpers.OneSecond;
            OneOffSchedule s1 = new OneOffSchedule(i);
            OneOffSchedule s2 = new OneOffSchedule(j);
            AggregateSchedule aggregateSchedule = new AggregateSchedule(s1, s2);
            Assert.AreEqual(2, aggregateSchedule.Count());
            Assert.AreEqual(i, aggregateSchedule.Next(i - TimeHelpers.OneSecond));
            Assert.AreEqual(i, aggregateSchedule.Next(i));
            Assert.AreEqual(j, aggregateSchedule.Next(i + Duration.FromTicks(1)));
            Assert.AreEqual(j, aggregateSchedule.Next(j));
            Assert.AreEqual(Instant.MaxValue, aggregateSchedule.Next(j + Duration.FromTicks(1)));
        }
    }
}