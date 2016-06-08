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
    public class DbInfoTests
    {
        [TestMethod()]
        public void RegisterTest()
        {
            var db = new DbInfo();
            db.Register<TestModel>();
            Assert.AreEqual(1, db.Count);
        }

        [TestMethod()]
        public void GetTableTest()
        {
            var db = new DbInfo();
            db.Register<TestModel>("TestTable").SetFields(
                new DbFieldInfo("IntProperty", "IntField"),
                new DbFieldInfo("StringProperty", "StringField"));

            var test = db.GetTable<TestModel>();
            Assert.AreEqual("TestTable", test.TableName);
            Assert.AreEqual(typeof(TestModel), test.ModelType);
            Assert.AreEqual(null, test.Token);
            Assert.AreEqual(2, test.FieldInfoList.Count);
        }
    }
}