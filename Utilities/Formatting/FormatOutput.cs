using System;
using System.IO;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Implements <see cref="IWriteable"/> to produce an object that, when written, ouputs it's specified format.
    /// For example, ToString("My format") => "My format".
    /// </summary>
    public sealed class FormatOutput : IFormattable, IWriteable
    {
        /// <summary>
        /// The default instance.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly FormatOutput Default = new FormatOutput();

        /// <summary>
        /// Prevents a default instance of the <see cref="FormatOutput"/> class from being created.
        /// </summary>
        private FormatOutput()
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            using (TextWriter writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        [StringFormatMethod("format")]
        public string ToString([CanBeNull] string format, [CanBeNull] IFormatProvider formatProvider = null)
        {
            using (TextWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, format);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull] FormatBuilder format, [CanBeNull] IFormatProvider formatProvider = null)
        {
            using (TextWriter writer = new StringWriter(formatProvider))
            {
                WriteTo(writer, format);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Writes this instance to the <see paramref="writer" />, using the optional <see paramref="format" />.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        // ReSharper disable once CodeAnnotationAnalyzer
        public void WriteTo(TextWriter writer, string format = null)
        {
            if (!string.IsNullOrEmpty(format))
                writer.Write(format);
        }
    }
}