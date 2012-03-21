using System;
using System.Text;

namespace WebApplications.Testing
{
    /// <summary>
    /// Base class for Unit tests.
    /// </summary>
    public abstract class TestBase
    {
        public static readonly Random Random = new Random();

        /// <summary>
        /// Generates a random string.
        /// </summary>
        /// <param name="maxLength">Maximum length.</param>
        /// <param name="unicode">if set to <see langword="true"/> string is UTF16; otherwise it uses ASCII.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected static string GenerateRandomString(int maxLength = -1, bool unicode = true)
        {
            // Get string length, if there's no maximum then use 8001 (as 8000 is max specific size in SQL Server).
            int length = (maxLength < 0 ? 8001 : maxLength) * (unicode ? 2 : 1);
            byte[] bytes = new byte[length];
            Random.NextBytes(bytes);
            return unicode ? new UnicodeEncoding().GetString(bytes) : new ASCIIEncoding().GetString(bytes);
        }
    }
}
