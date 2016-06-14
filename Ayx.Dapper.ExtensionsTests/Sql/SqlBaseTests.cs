using Ayx.Dapper.Extensions.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Ayx.Dapper.ExtensionsTests;
using System.Collections.Generic;

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

            Assert.IsFalse(new SelectProvider(type, null, new SqlCache()).CheckDbProperty(modelProperty));
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

        [TestMethod()]
        public void GetWhereTest()
        {
            var actual1 = SqlBase.GetWhere("");
            var actual2 = SqlBase.GetWhere("ID>10");

            Assert.AreEqual("", actual1);
            Assert.AreEqual(" WHERE ID>10", actual2);
        }

        [TestMethod()]
        public void GetKeyWhereTest()
        {
            var type = typeof(AttributeModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("IntProperty", "IntKey").SetPrimaryKey());

            var test = new SelectProvider(type, tableInfo, new SqlCache());
            var test2 = new SelectProvider(type, null, null);
            var actual = test.GetKeyWhere();
            var actual2 = test2.GetKeyWhere();

            Assert.AreEqual(" WHERE IntKey=@IntProperty", actual);
            Assert.AreEqual(" WHERE ID=@ID", actual2);
        }

        [TestMethod()]
        public void GetFieldsTest()
        {
            var type = typeof(AttributeModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("IntProperty", "IntKey"),
                new DbFieldInfo("NotField").SetNotDbField());
            var test = new InsertProvider(type, tableInfo, null);
            var actual1 = test.GetFields(null);
            var actual2 = test.GetFields("ID,IntProperty");

            Assert.AreEqual("ID,StringProperty,IntKey", actual1);
            Assert.AreEqual("ID,IntKey", actual2);
        }

        [TestMethod()]
        public void GetFieldNameTest()
        {
            var type = typeof(AttributeModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("IntProperty", "IntKey"),
                new DbFieldInfo("NotField").SetNotDbField());
            var property = type.GetProperties().Where(p => p.Name == "IntProperty").FirstOrDefault();
            if (property == null)
                Assert.Fail();

            var test1 = new SelectProvider(type, tableInfo, null);
            var test2 = new SelectProvider(type, null, null);

            var actual1 = test1.GetFieldName(property);
            var actual2 = test2.GetFieldName(property);

            Assert.AreEqual("IntKey", actual1);
            Assert.AreEqual("IntField", actual2);
        }

        [TestMethod()]
        public void GetPropertyTest()
        {
            var test = new SelectProvider(typeof(AttributeModel), null, null);
            var actual = test.GetProperty("StringProperty");
            if (actual == null) Assert.Fail();

            Assert.AreEqual("StringProperty", actual.Name);
        }

        [TestMethod()]
        public void JoinFieldsTest()
        {
            var actual = SqlBase.JoinFields(
                new List<string>
                {
                    "aaaaa",
                    "bbbbb",
                    "",
                    "ccc",
                    "",
                    "ddddddd",
                });
            Assert.AreEqual("aaaaa,bbbbb,ccc,ddddddd", actual);
        }
    }
}