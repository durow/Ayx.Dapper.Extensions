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
    public class SqlGeneratorTests
    {
        [TestMethod()]
        public void MakeSelectFieldsTest()
        {
            var type = typeof(TestModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("IntProperty", "IntField"),
                new DbFieldInfo("NotField").SetNotDbField());

            var test1 = SqlGenerator.MakeSelectFields(type, tableInfo, null);
            var test2 = SqlGenerator.MakeSelectFields(type, tableInfo, "");
            Assert.AreEqual("*", test1);
            Assert.AreEqual("ID,StringProperty,IntField", test2);
        }

        [TestMethod()]
        public void MakeWhereTest()
        {
            var where1 = "";
            var where2 = "IntField>34 AND StringProperty=@StringProperty";
            var test1 = SqlGenerator.MakeWhere(where1);
            var test2 = SqlGenerator.MakeWhere(where2);

            Assert.AreEqual("", test1);
            Assert.AreEqual(" WHERE IntField>34 AND StringProperty=@StringProperty", test2);
        }

        [TestMethod()]
        public void MakeTableNameTest()
        {
            var type = typeof(TestModel);
            var tableInfo = new DbTableInfo(type, "TestTable");

            var test1 = SqlGenerator.MakeTableName(type, null);
            var test2 = SqlGenerator.MakeTableName(type, tableInfo);

            Assert.AreEqual("TestModel", test1);
            Assert.AreEqual("TestTable", test2);
        }

        [TestMethod()]
        public void AddAndGetTest()
        {
            var gen = new SqlGenerator();
            gen.AddToCache(typeof(TestModel), "SELECT", null, null, "Test");
            Assert.AreEqual(1, gen.CacheCount);

            var sql = gen.GetFromCache(typeof(TestModel), "SELECT", null, null);
            Assert.AreEqual("Test", sql.SqlString);
        }

        [TestMethod()]
        public void GenerateSelectTest()
        {
            var sqlGen = new SqlGenerator();
            var type = typeof(TestModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("IntProperty", "IntField"),
                new DbFieldInfo("NotField").SetNotDbField());

            var test1 = sqlGen.GenerateSelect(type, tableInfo);
            var test2 = sqlGen.GenerateSelect(type, tableInfo, "");
            var test3 = sqlGen.GenerateSelect(type, tableInfo, "StringProperty", "IntField>34 AND StringProperty=@StringProperty ORDER BY IntField");

            var expected1 = "SELECT * FROM TestModel";
            var expected2 = "SELECT ID,StringProperty,IntField FROM TestModel";
            var expected3 = "SELECT StringProperty FROM TestModel WHERE IntField>34 AND StringProperty=@StringProperty ORDER BY IntField";

            Assert.AreEqual(expected1, test1);
            Assert.AreEqual(expected2, test2);
            Assert.AreEqual(expected3, test3);
            Assert.AreEqual(3, sqlGen.CacheCount);

            var test4 = sqlGen.GenerateSelect(type, tableInfo, "StringProperty", "IntField>34 AND StringProperty=@StringProperty ORDER BY IntField");
            Assert.ReferenceEquals(test3, test4);
            Assert.AreEqual(3, sqlGen.CacheCount);
        }

        [TestMethod()]
        public void MakeDeleteWhereTest()
        {
            var type = typeof(TestModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("IntProperty", "IntField"),
                new DbFieldInfo("NotField").SetNotDbField(),
                new DbFieldInfo("ID").SetPrimaryKey());
            var test1 = SqlGenerator.MakeDeleteWhere(type, tableInfo, null);
            var test2 = SqlGenerator.MakeDeleteWhere(type, tableInfo, "ID>10");

            Assert.AreEqual(" WHERE ID=@ID", test1);
            Assert.AreEqual(" WHERE ID>10", test2);
        }

        [TestMethod()]
        public void GenerateDeleteTest()
        {
            var gen = new SqlGenerator();
            var type = typeof(TestModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("IntProperty", "IntField"),
                new DbFieldInfo("NotField").SetNotDbField(),
                new DbFieldInfo("ID").SetPrimaryKey());

            var test1 = gen.GenerateDelete(type, tableInfo);
            var test2 = gen.GenerateDelete(type, tableInfo, "ID>10");
            var expected1 = "DELETE FROM TestModel WHERE ID=@ID";
            var expected2 = "DELETE FROM TestModel WHERE ID>10";

            Assert.AreEqual(expected1, test1);
            Assert.AreEqual(expected2, test2);
            Assert.AreEqual(2, gen.CacheCount);

            var test3 = gen.GenerateDelete(type, tableInfo, "ID>10");
            Assert.ReferenceEquals(test2, test3);
            Assert.AreEqual(2, gen.CacheCount);
        }

        [TestMethod()]
        public void GenerateDeleteTest1()
        {
            var gen = new SqlGenerator();
            var type = typeof(TestModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("IntProperty", "IntField"),
                new DbFieldInfo("NotField").SetNotDbField());

            try
            {
                var test1 = gen.GenerateDelete(type, tableInfo);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("can't find primary key when generate delete command!", e.Message);
            }

            try
            {
                var test2 = gen.GenerateDelete(type, null);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("can't find primary key when generate delete command!", e.Message);
            }
        }
    }
}