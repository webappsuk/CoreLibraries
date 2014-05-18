using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Service
{
    // TODO Move to ConsoleHelper
    public static class ConsoleEx
    {
        /// <summary>
        /// Invalid characters when reading a password line.
        /// </summary>
        [NotNull]
        private static readonly HashSet<char> _passwordFilter = new HashSet<char>(new[] {'\0', '\x1b', '\t', '\n'});

        /// <summary>
        /// Like System.Console.ReadLine(), only with a mask.
        /// </summary>
        /// <param name="mask">a <c>char</c> representing your choice of console mask</param>
        /// <returns>the string the user typed in</returns>
        [NotNull]
        public static string ReadPassword(char mask = '*')
        {
            LinkedList<char> pass = new LinkedList<char>();
            char chr;

            while ((chr = Console.ReadKey(true).KeyChar) != '\r')
            {
                switch (chr)
                {
                    case '\b':
                        if (pass.Count > 0)
                        {
                            Console.Write("\b \b");
                            pass.RemoveLast();
                        }
                        break;
                    case '\x7f':
                        int c = pass.Count;
                        pass.Clear();
                        string cb = new string('\b', c);
                        string cs = new string(' ', c);
                        Console.Write(cb + cs + cb);
                        break;
                    default:
                        if (!_passwordFilter.Contains(chr))
                        {
                            pass.AddLast(chr);
                            Console.Write(mask);
                        }
                        break;
                }
            }

            Console.WriteLine();
            return new string(pass.ToArray());
        }
    }
}