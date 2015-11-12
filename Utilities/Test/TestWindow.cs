#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestWindow
    {
        [TestMethod]
        public void TestImplicitCastPreservesDataLink()
        {
            List<int> list = new List<int> { 1, 2, 3, 4 };
            Window<int> window = list;
            ReadOnlyWindow<int> readOnlyWindow = window;

            CollectionAssert.AreEqual(list, window);
            CollectionAssert.AreEqual(list, readOnlyWindow);

            // Check that modifications are reflected in the windows.
            window[1] = 5;
            Assert.AreEqual(5, list[1]);
            Assert.AreEqual(window[1], list[1]);
            CollectionAssert.AreEqual(list, window);
            CollectionAssert.AreEqual(list, readOnlyWindow);
        }

        [TestMethod]
        public void TestLengthZero()
        {
            List<int> list = new List<int>();
            Window<int> window = list;
            ReadOnlyWindow<int> readOnlyWindow = list;

            Assert.AreEqual(0, window.Count);
            Assert.AreEqual(0, window.Length);
            Assert.AreEqual(0, window.Offset);
            Assert.AreEqual(0, readOnlyWindow.Count);
            Assert.AreEqual(0, readOnlyWindow.Length);
            Assert.AreEqual(0, readOnlyWindow.Offset);
        }

        [TestMethod]
        public void TestLengthImplicitZeroOffsetOne()
        {
            List<int> list = new List<int> { 1 };
            Window<int> window = new Window<int>(list, 1);
            ReadOnlyWindow<int> readOnlyWindow = new ReadOnlyWindow<int>(list, 1);

            Assert.AreEqual(0, window.Count);
            Assert.AreEqual(1, window.Length);
            Assert.AreEqual(1, window.Offset);
            Assert.AreEqual(0, readOnlyWindow.Count);
            Assert.AreEqual(1, readOnlyWindow.Length);
            Assert.AreEqual(1, readOnlyWindow.Offset);
        }

        [TestMethod]
        public void TestLengthExplicitZeroOffsetOne()
        {
            List<int> list = new List<int> { 1, 2 };
            Window<int> window = new Window<int>(list, 1, 0);
            ReadOnlyWindow<int> readOnlyWindow = new ReadOnlyWindow<int>(list, 1, 0);

            Assert.AreEqual(0, window.Count);
            Assert.AreEqual(2, window.Length);
            Assert.AreEqual(1, window.Offset);
            Assert.AreEqual(0, readOnlyWindow.Count);
            Assert.AreEqual(2, readOnlyWindow.Length);
            Assert.AreEqual(1, readOnlyWindow.Offset);
        }

        [TestMethod]
        public void TestInsert()
        {
            List<int> list = new List<int> { 1, 2, 3 };
            Window<int> window = new Window<int>(list, 1);
            ReadOnlyWindow<int> readOnlyWindow = new ReadOnlyWindow<int>(list, 1);

            int[] l = list.Skip(1).ToArray();
            CollectionAssert.AreEqual(l, window);
            CollectionAssert.AreEqual(l, readOnlyWindow);

            Assert.AreEqual(2, window.Count);
            Assert.AreEqual(3, window.Length);
            Assert.AreEqual(1, window.Offset);
            Assert.AreEqual(2, readOnlyWindow.Count);
            Assert.AreEqual(3, readOnlyWindow.Length);
            Assert.AreEqual(1, readOnlyWindow.Offset);

            // Insert through window
            window.Insert(1, 4);
            Assert.AreEqual(4, list[2]);
            Assert.AreEqual(window[1], list[2]);

            l = list.Skip(1).ToArray();
            CollectionAssert.AreEqual(l, window);
            CollectionAssert.AreEqual(l, readOnlyWindow);

            Assert.AreEqual(3, window.Count);
            Assert.AreEqual(4, window.Length);
            Assert.AreEqual(1, window.Offset);
            Assert.AreEqual(3, readOnlyWindow.Count);
            Assert.AreEqual(4, readOnlyWindow.Length);
            Assert.AreEqual(1, readOnlyWindow.Offset);

            // Insert through underlying data
            list.Insert(3, 5);
            Assert.AreEqual(5, list[3]);
            Assert.AreEqual(window[2], list[3]);

            l = list.Skip(1).ToArray();
            CollectionAssert.AreEqual(l, window);
            CollectionAssert.AreEqual(l, readOnlyWindow);

            Assert.AreEqual(4, window.Count);
            Assert.AreEqual(5, window.Length);
            Assert.AreEqual(1, window.Offset);
            Assert.AreEqual(4, readOnlyWindow.Count);
            Assert.AreEqual(5, readOnlyWindow.Length);
            Assert.AreEqual(1, readOnlyWindow.Offset);
        }

        [TestMethod]
        public void TestRemove()
        {
            List<int> list = new List<int> { 1, 2, 3 };
            Window<int> window = new Window<int>(list, 1);
            ReadOnlyWindow<int> readOnlyWindow = new ReadOnlyWindow<int>(list, 1);

            int[] l = list.Skip(1).ToArray();
            CollectionAssert.AreEqual(l, window);
            CollectionAssert.AreEqual(l, readOnlyWindow);

            Assert.AreEqual(2, window.Count);
            Assert.AreEqual(3, window.Length);
            Assert.AreEqual(1, window.Offset);
            Assert.AreEqual(2, readOnlyWindow.Count);
            Assert.AreEqual(3, readOnlyWindow.Length);
            Assert.AreEqual(1, readOnlyWindow.Offset);

            // Remove through window
            window.Remove(2);
            l = list.Skip(1).ToArray();
            CollectionAssert.AreEqual(l, window);
            CollectionAssert.AreEqual(l, readOnlyWindow);

            Assert.AreEqual(1, window.Count);
            Assert.AreEqual(2, window.Length);
            Assert.AreEqual(1, window.Offset);
            Assert.AreEqual(1, readOnlyWindow.Count);
            Assert.AreEqual(2, readOnlyWindow.Length);
            Assert.AreEqual(1, readOnlyWindow.Offset);

            // Remove from underlying list (note that the offset doesn't change, the window is effectively moved).
            list.Remove(1);
            l = list.Skip(1).ToArray();
            CollectionAssert.AreEqual(l, window);
            CollectionAssert.AreEqual(l, readOnlyWindow);

            Assert.AreEqual(0, window.Count);
            Assert.AreEqual(1, window.Length);
            Assert.AreEqual(1, window.Offset);
            Assert.AreEqual(0, readOnlyWindow.Count);
            Assert.AreEqual(1, readOnlyWindow.Length);
            Assert.AreEqual(1, readOnlyWindow.Offset);

            // Remove all items!
            list.Clear();

            Assert.AreEqual(0, window.Count);
            Assert.AreEqual(0, window.Length);
            Assert.AreEqual(1, window.Offset); // Note the offset is beyond the end of the underlying data
            Assert.AreEqual(0, readOnlyWindow.Count);
            Assert.AreEqual(0, readOnlyWindow.Length);
            Assert.AreEqual(1, readOnlyWindow.Offset); // Note the offset is beyond the end of the underlying data
        }

        [TestMethod]
        public void TestWindowOnBadOffset()
        {
            List<int> list = new List<int> { 1 };
            Window<int> window = new Window<int>(list, 1);

            // Remove all items!
            list.Clear();

            Assert.AreEqual(0, window.Count);
            Assert.AreEqual(0, window.Length);
            Assert.AreEqual(1, window.Offset); // Note the offset is beyond the end of the underlying data

            window.Add(1);

        }

        [TestMethod]
        public void TestReadOnlyMapWithOverlap()
        {
            List<int> list = new List<int> { 1, 2, 3, 4 };
            IReadOnlyList<int> map = new ReadOnlyMap<int>
            {
                new ReadOnlyWindow<int>(list, 1, 1),
                new ReadOnlyWindow<int>(list, 0, 3)
            };

            Assert.AreEqual(4, map.Count);
            CollectionAssert.AreEqual(new[] { 2, 1, 2, 3 }, (ICollection) map);
            Assert.AreEqual(2, map[0]);
            Assert.AreEqual(1, map[1]);
        }
    }
}