using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Text
{
    /// <summary>
    /// Class TextExtensions provides extension methods for dealing with Unicode Text.
    /// </summary>
    public static partial class TextExtensions
    {
        /// <summary>
        /// Converts a <see cref="string"/> into an <see cref="IEnumerable{T}">enumeration</see> of <see cref="UnicodeChar">unicode characters</see>.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>IEnumerable&lt;UnicodeChar&gt;.</returns>
        public static IEnumerable<UnicodeChar> ToUnicodeChars(this string s)
        {
            if (s == null) yield break;

            for (int i = 0; i < s.Length; ++i)
            {
                yield return new UnicodeChar(s, i);
                if (char.IsHighSurrogate(s, i))
                    i++;
            }
        }
    }
}
