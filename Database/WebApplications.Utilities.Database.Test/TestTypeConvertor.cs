using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Database.Schema;

namespace WebApplications.Utilities.Database.Test
{
    [TestClass]
    [Ignore]
    public class TestTypeConvertor
    {
        [TestMethod]
        public void LoadTypes()
        {
            CheckAssignability<Int64, Int64>();
            CheckAssignability<Int64, Int64?>();
            CheckAssignability<Int64, Int32>();
            CheckAssignability<Int64, Int32?>();


            CheckAssignability<SqlInt64, Int64>();
            CheckAssignability<SqlInt64, Int32>();
            CheckAssignability<SqlInt64, Int32?>();
        }

        private void CheckAssignability<TTo, TFrom>()
        {
            Trace.WriteLine(string.Format("{0} can{2} be assigned to, can{3} be converted to {1}.",
                                          typeof (TFrom),
                                          typeof (TTo),
                                          typeof (TTo).IsAssignableFrom(typeof (TFrom)) ? string.Empty : "not",
                                          typeof(TFrom).GetConversion(typeof(TTo)) != null ? string.Empty : "not"));
        }
    }
}
