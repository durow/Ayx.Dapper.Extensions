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
            var test3 = sqlGen.GenerateSelect(type, tableInfo, "StringProperty","IntField>34 AND StringProperty=@StringProperty ORDER BY IntField");

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
    }
}