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
        /// Converts a <see cref="string"/> into an <see cref="IEnumerable{T}">enumeration</see> of <see cref="UChar">unicode characters</see>.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>Enumeration of <see cref="UChar"/>.</returns>
        public static IEnumerable<UChar> ToUChars(this string s)
        {
            if (s == null) yield break;

            for (int i = 0; i < s.Length; ++i)
            {
                yield return new UChar(s, i);
                if (char.IsHighSurrogate(s, i))
                    i++;
            }
        }
    }
}
