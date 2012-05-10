using System;
using WebApplications.Testing;

namespace WebApplications.Utilities.Cryptography.Test
{
    /// <summary>
    /// Serialization Test Base.
    /// </summary>
    public abstract class SerializationTestBase : TestBase
    {
        protected static readonly Random Random = new Random();
    }
}