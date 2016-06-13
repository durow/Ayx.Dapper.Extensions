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
    public class SelectProviderTests
    {
        [TestMethod()]
        public void GetSelectFieldsTest()
        {
            var select = new SelectProvider(typeof(AttributeModel), null, new SqlCache());
            var test1 = select.GetSelectFields(null);
            var test2 = select.GetSelectFields("");
            var test3 = select.GetSelectFields("ID,StringProperty,IntProperty");
            var test4 = select.GetSelectFields("ID,StringProperty,IntField");

            var expected1 = @"ID,StringProperty,IntField";
            Assert.AreEqual("*", test1);
            Assert.AreEqual(expected1, test2);
            Assert.AreEqual(expected1, test3);
            Assert.AreEqual(expected1, test4);
        }
    }
}