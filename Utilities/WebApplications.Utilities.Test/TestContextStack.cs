#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: TestContextStack.cs
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx;
using WebApplications.Utilities.Threading;
using WebApplications.Testing;

namespace WebApplications.Utilities.Test
{
#if false
    [TestClass]
    public class TestContextStack
    {
        public int Loops = 10000;

        [TestMethod]
        public void TestPerformance()
        {
            ContextStack<int> contextStack = new ContextStack<int>();
            Stopwatch s = new Stopwatch();
            s.Start();
            Parallel.For(0, Loops, i =>
                                       {
                                           using (contextStack.Region(i))
                                           {
                                               Assert.AreEqual(i, contextStack.Current);
                                           }
                                       });
            s.Stop();
            Trace.WriteLine(s.ToString("{0} loops", Loops));
        }

        private int _task1ThreadSwitch;
        private int _task2ThreadSwitch;
        private int _task2NewThread;

        [TestMethod]
        public void TestAsync()
        {
            const int taskCount = 5000;
            
            // Create an asynchronous context in which to run.
            AsyncContext.Run(
                async ()
                =>
                          {
                              Task[] tasks = new Task[taskCount];
                              for (int a = 0; a < taskCount; a++)
                                  tasks[a] = TaskEx.RunEx(
                                      async () =>
                                                {
                                                    ContextStack<string> stack = new ContextStack<string>();
                                                    TaskCompletionSource tcs1 = new TaskCompletionSource();
                                                    TaskCompletionSource tcs2 = new TaskCompletionSource();
                                                    TaskCompletionSource tcs3 = new TaskCompletionSource();

                                                    string randomString = Guid.NewGuid().ToString();
                                                    string randomString2 = Guid.NewGuid().ToString();
                                                    using (stack.Region(randomString))
                                                    {
                                                        // Check we have 'A' in the current stack.
                                                        Assert.AreEqual(randomString, stack.Current);
                                                        Assert.AreEqual(1, stack.CurrentStack.Count());
                                                        Assert.AreEqual(randomString, stack.CurrentStack.Last());
                                                        int threadId = Thread.CurrentThread.ManagedThreadId;

                                                        // Await the task
                                                        await TaskEx.Delay(500);

                                                        // Check we still have 'A' in the current stack.
                                                        Assert.AreEqual(randomString, stack.Current);
                                                        Assert.AreEqual(1, stack.CurrentStack.Count());
                                                        Assert.AreEqual(randomString, stack.CurrentStack.Last());


                                                        // Create new thread.
                                                        Task task = TaskEx.RunEx(
                                                            async () =>
                                                                      {
                                                                          int task2Thread = Thread.CurrentThread.ManagedThreadId;

                                                                          // Assess if this is a new thread
                                                                          if (threadId != task2Thread)
                                                                              Interlocked.Increment(ref _task2NewThread);

                                                                          // Check we have 'A' in the current stack.
                                                                          Assert.AreEqual(randomString,
                                                                                          stack.Current);
                                                                          Assert.AreEqual(1, stack.CurrentStack.Count());
                                                                          Assert.AreEqual(randomString,
                                                                                          stack.CurrentStack.Last());

                                                                          // Wait for the first signal
                                                                          await tcs1.Task;

                                                                          // Check we still have 'A' in the current stack (i.e. we're not affected by additions in first thread.
                                                                          Assert.AreEqual(randomString, stack.Current);
                                                                          Assert.AreEqual(1, stack.CurrentStack.Count());
                                                                          Assert.AreEqual(randomString,
                                                                                          stack.CurrentStack.Last());

                                                                          // Add C to stack.
                                                                          using (stack.Region("C"))
                                                                          {
                                                                              // We should have A, C in stack now.
                                                                              Assert.AreEqual("C", stack.Current);
                                                                              Assert.AreEqual(2,
                                                                                              stack.CurrentStack.Count());
                                                                              Assert.AreEqual(randomString,
                                                                                              stack.CurrentStack.First());

                                                                              // Second signal
                                                                              tcs2.SetResult();

                                                                              // Wait for the 3rd signal
                                                                              await tcs3.Task;

                                                                              // We should still have A, C in stack now.
                                                                              Assert.AreEqual("C", stack.Current);
                                                                              Assert.AreEqual(2,
                                                                                              stack.CurrentStack.Count());
                                                                              Assert.AreEqual(randomString,
                                                                                              stack.CurrentStack.First());
                                                                          }

                                                                          // Back to just having C.
                                                                          Assert.AreEqual(randomString, stack.Current);
                                                                          Assert.AreEqual(1, stack.CurrentStack.Count());
                                                                          Assert.AreEqual(randomString,
                                                                                          stack.CurrentStack.Last());

                                                                          // Wait a bit before finishing.
                                                                          await TaskEx.Delay(100);


                                                                          if (task2Thread !=
                                                                              Thread.CurrentThread.ManagedThreadId)
                                                                              Interlocked.Increment(
                                                                                  ref _task2ThreadSwitch);
                                                                      });


                                                        // Add B to stack.
                                                        using (stack.Region(randomString2))
                                                        {
                                                            // We should have A, B in stack now.
                                                            Assert.AreEqual(randomString2, stack.Current);
                                                            Assert.AreEqual(2, stack.CurrentStack.Count());
                                                            Assert.AreEqual(randomString, stack.CurrentStack.First());

                                                            // Signal 2nd task with first signal.
                                                            tcs1.SetResult();

                                                            // Wait for 2nd task to signal back with 2nd second.
                                                            await tcs2.Task;

                                                            // We should still have A, B in stack now.
                                                            Assert.AreEqual(randomString2, stack.Current);
                                                            Assert.AreEqual(2, stack.CurrentStack.Count());
                                                            Assert.AreEqual(randomString, stack.CurrentStack.First());

                                                            // Signal 2nd task with third signal
                                                            tcs3.SetResult();

                                                            // Wait for task to finish.
                                                            await task;

                                                            // We should still have A, B in stack now.
                                                            Assert.AreEqual(randomString2, stack.Current);
                                                            Assert.AreEqual(2, stack.CurrentStack.Count());
                                                            Assert.AreEqual(randomString, stack.CurrentStack.First());
                                                        }

                                                        // We should just have A in stack.
                                                        Assert.AreEqual(randomString, stack.Current);
                                                        Assert.AreEqual(1, stack.CurrentStack.Count());

                                                        if (threadId != Thread.CurrentThread.ManagedThreadId)
                                                            Interlocked.Increment(ref _task1ThreadSwitch);
                                                    }

                                                    // The stack should be empty
                                                    Assert.IsNull(stack.Current);
                                                    Assert.AreEqual(0, stack.CurrentStack.Count());
                                                });

                              await TaskEx.WhenAll(tasks);
                          });

            Trace.WriteLine(
                String.Format(
                    "Task1 Thread Switch: {1}{0}Task2 Thread Switch: {2}{0}Task2 new thread: {3}{0}Total tasks: {4}",
                    Environment.NewLine,
                    _task1ThreadSwitch,
                    _task2ThreadSwitch,
                    _task2NewThread,
                    taskCount));
        }

        [TestMethod]
        public void TestThreading()
        {
            ContextStack<string> stack = new ContextStack<string>();
            TestContext(stack);
        }


        public async Task TestContext([NotNull]ContextStack<string> stack)
        {
        }

        #region Nested type: TestObject
        [Serializable]
        public class TestObject : ICloneable
        {
            private static int _instanceCounter;
            public readonly int Id;
            public readonly int InstanceId;
            public int Value;

            public TestObject(int id)
            {
                Id = id;
                InstanceId = ++_instanceCounter;
            }

            #region ICloneable Members
            /// <summary>
            ///   Creates a new object that is a copy of the current instance.
            /// </summary>
            /// <returns>
            ///   A new object that is a copy of this instance.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public object Clone()
            {
                return new TestObject(Id) {Value = Value};
            }
            #endregion
        }
        #endregion
    }
#endif
}