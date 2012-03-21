using System;
using System.IO;
using System.Linq;

namespace WebApplications.Testing
{
    /// <summary>
    /// Base class for unit tests on Databases.
    /// </summary>
    public abstract class TestDatabaseBase : TestBase
    {

        /// <summary>
        /// Static constructor of the <see cref="T:System.Object"/> class, used to initialize the locatoin of the data directory for all tests.
        /// </summary>
        static TestDatabaseBase()
        {
            // Find the data directory
            string path = Path.GetDirectoryName(typeof(TestBase).Assembly.Location);
            string root = Path.GetPathRoot(path);
            string dataDirectory;
            do
            {
                // Look recursively for directory called Data containing mdf files.
                dataDirectory = Directory.GetDirectories(path, "Data", SearchOption.AllDirectories)
                    .SingleOrDefault(d => Directory.GetFiles(d, "*.mdf", SearchOption.TopDirectoryOnly).Any());

                // Move up a directory
                path = Path.GetDirectoryName(path);
            } while ((dataDirectory == null) &&
                     !String.IsNullOrWhiteSpace(path) &&
                     !path.Equals(root, StringComparison.CurrentCultureIgnoreCase));

            if (dataDirectory == null)
                throw new InvalidOperationException("Could not find the data directory.");

            // Set the DataDirectory data in the current AppDomain for use in connection strings.
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
        }

        /// <summary>
        /// Creates the connection string.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="isAsync">if set to <see langword="true"/> then set connection to asynchronous.</param>
        /// <returns></returns>
        protected static string CreateConnectionString(string databaseName, bool isAsync = false)
        {
            return
                String.Format(
                    @"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\{0}.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True;{1}",
                    databaseName,
                    isAsync ? "Asynchronous Processing=true" : String.Empty);
        }
    }
}