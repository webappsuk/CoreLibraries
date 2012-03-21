using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Ionic.Zip;
using JetBrains.Annotations;

namespace WebApplications.Utilities.PowerShell
{
    /// <summary>
    /// Provide zip utilities.
    /// </summary>
    [UsedImplicitly]
    public static class Zip
    {
        /// <summary>
        /// Compresses the specified directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="zipFile">The zip file.</param>
        /// <param name="force">if set to <see langword="true"/> overwrites zip if present.</param>
        [UsedImplicitly]
        public static void Compress([NotNull]string directory, [NotNull]string zipFile, bool force)
        {
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException(String.Format(Resources.Zip_Compress_DirectoryNotFound,
                                                                   directory));

            if (File.Exists(zipFile))
            {
                if (!force)
                    throw new InvalidOperationException(String.Format(Resources.Zip_Compress_CannotOverwriteExistingFile, zipFile));

                File.Delete(zipFile);
            }

            using (ZipFile z = new ZipFile(zipFile))
            {
                z.AddDirectory(directory);
                z.Save();
            }
        }
        /// <summary>
        /// Compresses the specified directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="zipFile">The zip file.</param>
        /// <param name="force">if set to <see langword="true"/> overwrites zip if present.</param>
        [UsedImplicitly]
        public static void Decompress([NotNull]string zipFile, [NotNull]string directory, bool force)
        {
            if (!File.Exists(zipFile))
                throw new FileNotFoundException(string.Format(Resources.Zip_Decompress_FileNotFound, zipFile));
            
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (ZipFile z = new ZipFile(zipFile))
            {
                z.ExtractAll(directory,
                             force ? ExtractExistingFileAction.OverwriteSilently : ExtractExistingFileAction.Throw);
            }
        }
    }
}
