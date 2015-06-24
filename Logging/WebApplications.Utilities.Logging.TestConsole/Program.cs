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
using System.Collections.Generic;
using System.Drawing;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Logging.Loggers;

namespace WebApplications.Utilities.Logging.TestConsole
{
    internal class Program
    {
        private static void Main([NotNull] string[] args)
        {
            Log.SetTrace(validLevels: LoggingLevels.None);
            Log.SetConsole(Log.ShortFormat);
            //Log.AddLogger(new EventLogger("Events"));

            FormatBuilder prompt = new FormatBuilder()
                .AppendForegroundColor(Color.Chartreuse)
                .Append('[')
                .AppendFormat("{time:hh:MM:ss.ffff}")
                .AppendResetForegroundColor()
                .Append("] > ");

            Log.Add(new LogContext().Set("Test key", "Test value"), "Test Message");

            Log.Add(new InvalidOperationException("An invalid operation example."));

            foreach (LoggingLevel level in Enum.GetValues(typeof (LoggingLevel)))
                Log.Add(level, level.ToString());

            string line;
            do
            {
                prompt.WriteToConsole(null, new Dictionary<string, object> {{"time", DateTime.UtcNow}});
                line = Console.ReadLine();
            } while (!string.IsNullOrWhiteSpace(line));
        }
    }
}