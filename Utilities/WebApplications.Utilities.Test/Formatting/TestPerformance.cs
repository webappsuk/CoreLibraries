using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Test.Formatting
{
    [TestClass]
    public class TestPerformance
    {
        [TestMethod]
        [Timeout(60000)]
        public void TestThreadSafetyNestedLayoutLongLine()
        {
            using (StringWriter stringWriter = new StringWriter())
            using (FormatTextWriter formatTextWriter = new FormatTextWriter(stringWriter))
            {
                Stopwatch watch = Stopwatch.StartNew();
                Parallel.For(
                    0,
                    1000,
                    new ParallelOptions() { MaxDegreeOfParallelism = 8 },
                    i => new FormatBuilder()
                        .Append(FormatResources.ButIMustExplain)
                        .WriteTo(formatTextWriter));
                watch.Stop();
                Trace.WriteLine(watch.Elapsed.TotalMilliseconds);
                Assert.AreEqual(970000, formatTextWriter.Position);
                string result = stringWriter.ToString();

                string[] lines = result
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                // Check number of lines and maximum line length, if we have any race conditions we expect these to change.
                Assert.AreEqual(1, lines.Length);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public void TestLargeEnumerable()
        {
            FormatBuilder builder = new FormatBuilder("{List:[{<ITEM>:0.00}{<JOIN>:, }]}");
            const int loop = 1000;
            const int items = 100;

            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();
            Stopwatch sw = Stopwatch.StartNew();
            Parallel.For(0, loop, i => Enumerable.Range(0, items).ToArray());
            sw.Stop();

            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();
            sw.Restart();
            Parallel.For(0, loop, i => Enumerable.Range(0, items).ToArray());
            sw.Stop();
            TimeSpan control = sw.Elapsed;

            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();
            sw.Restart();
            Parallel.For(0, loop, i =>
            {
                string s = builder.ToString(
                (w, chunk) => string.Equals(chunk.Tag, "list", StringComparison.CurrentCultureIgnoreCase)
                    ? new Optional<object>(Enumerable.Range(0, items))
                    : Optional<object>.Unassigned);
            });
            sw.Stop();
            double builderTime = ((sw.Elapsed - control).TotalMilliseconds / loop);
            
            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();
            sw.Restart();
            Parallel.For(0, loop, i => { var s = '[' + string.Join(", ", Enumerable.Range(0, items).Select(j => j.ToString("0.00"))) + ']'; });
            sw.Stop();
            double simpleTime = ((sw.Elapsed - control).TotalMilliseconds / loop);

            Trace.WriteLine(string.Format("Builder: {0:0.000}ms per item.", builderTime));
            Trace.WriteLine(string.Format("Simple: {0:0.000}ms per item.", simpleTime));
            Trace.WriteLine(string.Format("Simple takes {0:0.000}% of builder time.", (simpleTime / builderTime) * 100));
            Trace.WriteLine(string.Format("Builder takes {0:0.000}x longer than simple.", (builderTime / simpleTime)));
        }
    }
}
