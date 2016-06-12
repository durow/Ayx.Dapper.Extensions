﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Ayx.Dapper.ExtensionsTests;

namespace Ayx.Dapper.Extensions.Sql.Tests
{
    [TestClass()]
    public class SqlBaseTests
    {
        [TestMethod()]
        public void CheckDbPropertyTest()
        {
            var type = typeof(TestModel);

            var intProperty = type.GetProperties().Where(p => p.Name == "IntProperty").FirstOrDefault();
            var modelProperty = type.GetProperties().Where(p => p.Name == "Model").FirstOrDefault();

            if (intProperty == null || modelProperty == null)
                Assert.Fail();

            Assert.IsFalse(new SelectProvider(type,null,new SqlCache()).CheckDbProperty(modelProperty));
            Assert.IsTrue(new SelectProvider(type, null, new SqlCache()).CheckDbProperty(intProperty));
        }

        [TestMethod()]
        public void GetTableNameTest()
        {
            var type = typeof(AttributeModel);
            var tableInfo = new DbTableInfo(type, "TestTable");

            var test = new SelectProvider(type, tableInfo, new SqlCache());
            var test2 = new SelectProvider(type, null, new SqlCache());
            var test3 = new SelectProvider(typeof(TestModel), null, new SqlCache());

            var actual1 = test.GetTableName();
            var actual2 = test2.GetTableName();
            var actual3 = test3.GetTableName();

            Assert.AreEqual("TestTable", actual1);
            Assert.AreEqual("TestModel", actual2);
            Assert.AreEqual("TestModel", actual3);
        }

        [TestMethod()]
        public void CheckDbFieldTest()
        {
            var type = typeof(TestModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("NotField").SetNotDbField());

            var test1 = new InsertProvider(type, tableInfo, new SqlCache());
            var test2 = new InsertProvider(typeof(AttributeModel), null, new SqlCache());

            var intProperty = type.GetProperties().Where(p => p.Name == "IntProperty").FirstOrDefault();
            var notProperty = type.GetProperties().Where(p => p.Name == "NotField").FirstOrDefault();
            var attProperty = typeof(AttributeModel).GetProperties().Where(p => p.Name == "NotField").FirstOrDefault();

            Assert.IsTrue(test1.CheckDbField(intProperty));
            Assert.IsFalse(test1.CheckDbField(notProperty));
            Assert.IsFalse(test2.CheckDbField(attProperty));
        }

        [TestMethod()]
        public void CheckPrimaryKeyTest()
        {
            var type = typeof(TestModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("ID").SetPrimaryKey());

            var test1 = new InsertProvider(type, tableInfo, new SqlCache());
            var test2 = new InsertProvider(typeof(AttributeModel), null, new SqlCache());

            var intProperty = type.GetProperties().Where(p => p.Name == "IntProperty").FirstOrDefault();
            var idProperty = type.GetProperties().Where(p => p.Name == "ID").FirstOrDefault();
            var attProperty = typeof(AttributeModel).GetProperties().Where(p => p.Name == "ID").FirstOrDefault();

            Assert.IsFalse(test1.CheckPrimaryKey(intProperty));
            Assert.IsTrue(test1.CheckPrimaryKey(idProperty));
            Assert.IsTrue(test2.CheckPrimaryKey(attProperty));
        }

        [TestMethod()]
        public void CheckAutoIncrementTest()
        {
            var type = typeof(TestModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("ID").SetAutoIncrement());

            var test1 = new InsertProvider(type, tableInfo, new SqlCache());
            var test2 = new InsertProvider(typeof(AttributeModel), null, new SqlCache());

            var intProperty = type.GetProperties().Where(p => p.Name == "IntProperty").FirstOrDefault();
            var idProperty = type.GetProperties().Where(p => p.Name == "ID").FirstOrDefault();
            var attProperty = typeof(AttributeModel).GetProperties().Where(p => p.Name == "ID").FirstOrDefault();

            Assert.IsFalse(test1.CheckAutoIncrement(intProperty));
            Assert.IsTrue(test1.CheckAutoIncrement(idProperty));
            Assert.IsTrue(test2.CheckAutoIncrement(attProperty));
        }
    }
}