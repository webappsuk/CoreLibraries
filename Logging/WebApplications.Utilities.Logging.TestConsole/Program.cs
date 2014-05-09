using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Logging.Loggers;

namespace WebApplications.Utilities.Logging.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.SetTrace(validLevels: LoggingLevels.None);
            Log.SetConsole(Log.XMLFormat);
            FormatBuilder prompt = new FormatBuilder()
                .AppendForegroundColor(Color.Chartreuse)
                .Append('[')
                .AppendFormat("{time:hh:MM:ss.ffff}")
                .AppendResetForegroundColor()
                .Append("] >");

            Log.Add(new LogContext().Set("Test key", "Test value"), "Test Message");

            //Log.Add(new InvalidOperationException("An invalid operation example."));

            //foreach (LoggingLevel level in Enum.GetValues(typeof(LoggingLevel)))
            //    Log.Add(level, level.ToString());



            string line;
            do
            {
                prompt.WriteToConsole(null, new Dictionary<string, object> { { "time", DateTime.UtcNow } });
                line = Console.ReadLine();
            } while (!string.IsNullOrWhiteSpace(line));
        }
        private static double Distance(Color a, Color b)
        {
            int rd = a.R - b.R;
            int gd = a.G - b.G;
            int bd = a.B - b.B;
            int ad = a.A - b.A;
            return rd * rd +
                   gd * gd +
                   bd * bd +
                   ad * ad;
        }
    }
}
