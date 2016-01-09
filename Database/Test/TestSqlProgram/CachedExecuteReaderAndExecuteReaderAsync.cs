using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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

            long lastTimeStamp = Stopwatch.GetTimestamp();
            Action<string> stamp = msg =>
            {
                long ts = Stopwatch.GetTimestamp();
                Trace.WriteLine($"{1000.0 * (ts - lastTimeStamp) / Stopwatch.Frequency}ms : {msg}");
                lastTimeStamp = ts;
            };

//            for (int cacheId = 0; cacheId < 29; cacheId++)
//            {
                int cacheId = -1;
                byte[] buffer;
                using (
                    SqlConnection connection =
                        new SqlConnection(
                            @"Data Source=RELEASESERVER1\SQL2008_R2;Initial Catalog=WUK_35109;Integrated Security=true;Type System Version=SQL Server 2012;")
                    )
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    stamp($"Opened {cacheId}");
                    using (SqlCommand sqlCommand = new SqlCommand("spThgWebData2", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 120
                    })
                    {
//                        sqlCommand.Parameters.AddWithValue("@CacheId", 15);
                        using (SqlDataReader reader =
                            await
                                sqlCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken)
                                    .ConfigureAwait(false))
                        {
                            stamp($"Executed {cacheId}");
                            buffer = await reader.GetBytesAsync(cancellationToken).ConfigureAwait(false);
                            Assert.IsNotNull(buffer);
                            stamp($"Cached {cacheId} {buffer.Length.ToMemorySize()}");
                        }
                    }
                }

                stamp("Closed");
                using (CachedDataReader cachedReader = new CachedDataReader(buffer))
                {
                    stamp($"Opened cached {cacheId}");
                    do
                    {
                        Trace.WriteLine($"Table started - {cachedReader.TableDefinition}{Environment.NewLine}({cachedReader.RecordsAffected} records affected)");

                        object[] values = new object[cachedReader.FieldCount];
                        while (await cachedReader.ReadAsync(cancellationToken))
                        {
                            cachedReader.GetValues(values);
                            //Trace.WriteLine($"\t\t{string.Join(" | ", values)}");
                        }
                    } while (await cachedReader.NextResultAsync(cancellationToken));
                }
                stamp($"Read cached {cacheId}");
//            }
        }
    }
}
