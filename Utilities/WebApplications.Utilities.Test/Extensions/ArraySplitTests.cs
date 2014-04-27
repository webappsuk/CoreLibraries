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
            int[] array = new[] {1, 2, 3, 4, 5};
            int[][] split = array.Split(2).ToArray();
            CheckSplit(array, split);
        }

        [TestMethod]
        public void Split_EmptyIndicesReturnOriginalArray()
        {
            int[] array = new[] {1, 2, 3, 4, 5};
            int[][] split = array.Split().ToArray();
            CheckSplit(array, split);
            Assert.AreSame(array, split[0]);
        }

        [TestMethod]
        public void Split_InvalidIndicesAreIgnored()
        {
            int[] array = new[] {1, 2, 3, 4, 5};
            int[][] split = array.Split(-1, 6).ToArray();
            CheckSplit(array, split);
            Assert.AreSame(array, split[0]);
        }

        [TestMethod]
        public void Split_DuplicateSplitIntroducesEmptyArray()
        {
            int[] array = new[] {1, 2, 3, 4, 5};
            int[][] split = array.Split(3, 3).ToArray();
            Assert.AreEqual(3, split.Length);
            Assert.AreEqual(0, split[1].Length);
            CheckSplit(array, split);
        }
    }
}