#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: TestPowerShell.cs
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.PowerShell;
using WebApplications.Testing;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestPowerShell
    {
        [Ignore]
        [TestMethod]
        public void TestMethod1()
        {
            const string root = @"C:\Sandboxes\Tools\trunk\Core";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            IEnumerable<Solution> solutions = Solution.GetAll(root, true);

            int count = 0;
            foreach (Solution solution in solutions.Where(s => s.HasNuSpecs))
            {
                Trace.WriteLine(solution);
                count++;
            }
            stopwatch.Stop();
            Trace.WriteLine(string.Format("Found {0} solutions with nuspecs.", count));
            Trace.WriteLine(stopwatch.ToString("Finding solutions"));

            Trace.WriteLine("Buid order");
            foreach (Solution solution in Solution.GetNugetBuildOrder(root))
            {
                Trace.WriteLine("=============================================");
                Trace.WriteLine(solution);
            }
        }
    }
}