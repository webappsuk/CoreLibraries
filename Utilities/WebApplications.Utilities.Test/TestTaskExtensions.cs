#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: TestTaskExtensions.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestTaskExtensions
    {
        [TestMethod]
        public void MyTest()
        {
            ((Func<Task>) ReturnTask).Safe();
        }

        public Task ReturnTask()
        {
            return new Task(() =>
                                {
                                    // DO nothing
                                });
        }

        [TestMethod]
        public void TestContinueWithAll()
        {
            Random random = new Random();
            Parallel.For(0, 1000, i =>
                                      {
                                          int taskCount = random.Next(50);
                                          int expectedResult = 0;
                                          List<Task<int>> tasks = new List<Task<int>>(taskCount);
                                          for (int a = 0; a < taskCount; a++)
                                          {
                                              int value = random.Next(100);
                                              tasks.Add(Task.Factory.StartNew(() => value));
                                              expectedResult += value;
                                          }
                                          Task<int> continuation = tasks.AfterAll(
                                              t =>
                                              t != null ? t.Aggregate(0, (current, task) => current + task.Result) : 0,
                                              TaskCreationOptions.None);
                                          Assert.AreEqual(expectedResult, continuation.Result);
                                      });
        }
    }
}