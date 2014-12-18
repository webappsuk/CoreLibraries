using System;
using System.Collections.Generic;
using System.Linq;
using WebApplications.Utilities.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Reflect;

namespace WebApplications.Utilities.Logging.Test
{
    [TestClass]
    public class LogContextTest : LoggingTestBase
    {
        /// <summary>
        /// Points directly to LogContext._keyReservations
        /// </summary>
        [NotNull]
        private static readonly Dictionary<string, Guid> _keyReservations;

        /// <summary>
        /// Points directly to LogContext._prefixReservations
        /// </summary>
        [NotNull]
        private static readonly Dictionary<string, Guid> _prefixReservations;

        static LogContextTest()
        {
            // Set up access to private fields in LogContext.
            ExtendedType et = ExtendedType.Get(typeof (LogContext));
            _keyReservations =
                et.Fields.Single(f => f.Info.IsStatic && f.Info.IsInitOnly && f.Info.Name == "_keyReservations")
                  .Getter<Dictionary<string, Guid>>()();
            _prefixReservations =
                et.Fields.Single(f => f.Info.IsStatic && f.Info.IsInitOnly && f.Info.Name == "_prefixReservations")
                  .Getter<Dictionary<string, Guid>>()();
        }

        [TestMethod]
        public void TestKeyReservation()
        {
            lock (_keyReservations)
            {
                _keyReservations.Clear();
                _prefixReservations.Clear();
            }
            Guid reservation = Guid.NewGuid();
            string key = "My test key";
            string key2 = LogContext.ReserveKey(key, reservation);
            Assert.AreEqual(key, key2);
            Guid r2;
            Assert.IsTrue(_keyReservations.TryGetValue(key, out r2));
            Assert.AreEqual(reservation, r2);
            string value = Tester.RandomGenerator.RandomString();
            LogContext context = new LogContext();
            context.Set(reservation, key, value);
            Assert.IsNotNull(context);
            string value2 = context.Get(key);
            Assert.AreEqual(value, value2);
        }

        [TestMethod]
        [ExpectedException(typeof(LoggingException), "Using a reserved key without the reservation should throw an error.")]
        public void TestKeyReservationErrors()
        {
            lock (_keyReservations)
            {
                _keyReservations.Clear();
                _prefixReservations.Clear();
            }
            Guid reservation = Guid.NewGuid();
            string key = Tester.RandomGenerator.RandomString();
            string key2 = LogContext.ReserveKey(key, reservation);
            Assert.AreEqual(key, key2);
            Guid r2;
            Assert.IsTrue(_keyReservations.TryGetValue(key, out r2));
            Assert.AreEqual(reservation, r2);
            string value = Tester.RandomGenerator.RandomString();
            LogContext context = new LogContext();
            context.Set(key,value);
        }
    }
}
