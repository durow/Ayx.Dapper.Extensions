using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ayx.Dapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ayx.Dapper.ExtensionsTests;

namespace Ayx.Dapper.Extensions.Tests
{
    [TestClass()]
    public class DbTableInfoTests
    {
        [TestMethod()]
        public void CheckDbFieldTest()
        {
            var tableInfo1 = new DbTableInfo(typeof(TestModel));
            var tableInfo2 = new DbTableInfo(typeof(TestModel)).SetFields(
                new DbFieldInfo("NotField").SetNotDbField());

            Assert.IsTrue(tableInfo1.CheckDbField("NotField"));
            Assert.IsFalse(tableInfo2.CheckDbField("NotField"));
        }

        [TestMethod()]
        public void CheckPrimaryKeyTest()
        {
            var tableInfo1 = new DbTableInfo(typeof(TestModel));
            var tableInfo2 = new DbTableInfo(typeof(TestModel)).SetFields(
                new DbFieldInfo("ID").SetPrimaryKey());

            Assert.IsFalse(tableInfo1.CheckPrimaryKey("ID"));
            Assert.IsTrue(tableInfo2.CheckPrimaryKey("ID"));
        }

        [TestMethod()]
        public void CheckAutoIncrementTest()
        {
            var tableInfo1 = new DbTableInfo(typeof(TestModel));
            var tableInfo2 = new DbTableInfo(typeof(TestModel)).SetFields(
                new DbFieldInfo("ID").SetAutoIncrement());

            Assert.IsFalse(tableInfo1.CheckAutoIncrement("ID"));
            Assert.IsTrue(tableInfo2.CheckAutoIncrement("ID"));
        }
    }
}