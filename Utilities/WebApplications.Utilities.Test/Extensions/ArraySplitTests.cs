#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: ArraySplitTests.cs
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
// © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
#endregion

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Test.Extensions
{
    [TestClass]
    public class ArraySplitTests
    {
        public void CheckSplit<T>(T[] array, T[][] split)
        {
            Assert.IsNotNull(array);
            Assert.IsNotNull(split);

            int chunkIndex = 0;
            int index = 0;
            foreach (T item in array)
            {
                Assert.IsTrue(chunkIndex < split.Length);
                T[] chunk = split[chunkIndex];
                Assert.IsNotNull(chunk);

                while (index >= chunk.Length)
                {
                    index = 0;
                    chunkIndex++;
                    Assert.IsTrue(chunkIndex < split.Length);
                    chunk = split[chunkIndex];
                    Assert.IsNotNull(chunk);
                }

                Assert.IsNotNull(index < chunk.Length);
                Assert.AreEqual(item, split[chunkIndex][index++]);
            }
        }

        [TestMethod]
        public void Split_SingleIndexSplits()
        {
            int[] array = new[] { 1, 2, 3, 4, 5 };
            int[][] split = array.Split(2).ToArray();
            CheckSplit(array, split);
        }

        [TestMethod]
        public void Split_EmptyIndicesReturnOriginalArray()
        {
            int[] array = new[] { 1, 2, 3, 4, 5 };
            int[][] split = array.Split().ToArray();
            CheckSplit(array, split);
            Assert.AreSame(array, split[0]);
        }

        [TestMethod]
        public void Split_InvalidIndicesAreIgnored()
        {
            int[] array = new[] { 1, 2, 3, 4, 5 };
            int[][] split = array.Split(-1, 6).ToArray();
            CheckSplit(array, split);
            Assert.AreSame(array, split[0]);
        }

        [TestMethod]
        public void Split_DuplicateSplitIntroducesEmptyArray()
        {
            int[] array = new[] { 1, 2, 3, 4, 5 };
            int[][] split = array.Split(3,3).ToArray();
            Assert.AreEqual(3, split.Length);
            Assert.AreEqual(0, split[1].Length);
            CheckSplit(array, split);
        }
    }
}