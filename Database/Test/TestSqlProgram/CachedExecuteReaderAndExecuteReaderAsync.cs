using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Database.Caching;

namespace WebApplications.Utilities.Database.Test.TestSqlProgram
{
    [TestClass]
    public class CachedExecuteReaderAndExecuteReaderAsync : SqlProgramTestBase
    {
        [TestMethod]
        public async Task TempTest()
        {
            CancellationToken cancellationToken = CancellationToken.None;
            using (SqlConnection connection = new SqlConnection(@"Data Source=RELEASESERVER1\SQL2008_R2;Initial Catalog=WUK_35109;Integrated Security=true;"))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                byte[] buffer;
                using (SqlCommand sqlCommand = new SqlCommand("spThgWebData2", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 120
                })
                {
                    sqlCommand.Parameters.AddWithValue("@CacheId", 6);
                    using (SqlDataReader reader =
                            await sqlCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                    {
                        buffer = await reader.GetBytesAsync(cancellationToken).ConfigureAwait(false);
                        Assert.IsNotNull(buffer);
                    }
                }

                using (CachedDataReader cachedReader = new CachedDataReader(buffer))
                {
                    do
                    {
                        while (await cachedReader.ReadAsync(cancellationToken))
                        {
                        }
                    } while (await cachedReader.NextResultAsync(cancellationToken));
                }
            }

        }
    }
}
