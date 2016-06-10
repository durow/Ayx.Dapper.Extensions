using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ayx.Dapper.Extensions.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ayx.Dapper.ExtensionsTests;

namespace Ayx.Dapper.Extensions.Sql.Tests
{
    [TestClass()]
    public class SqlCacheTests
    {
        [TestMethod()]
        public void AddAndGetTest()
        {
            var cache = new SqlCache();
            cache.Add(typeof(TestModel), "SELECT", null, null, "Test");
            Assert.AreEqual(1, cache.Count);

            var sql = cache.Get(typeof(TestModel), "SELECT", null, null);
            Assert.AreEqual("Test", sql.SqlString);
        }
    }
}