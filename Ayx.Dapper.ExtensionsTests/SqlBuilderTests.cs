﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ayx.Dapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ayx.Dapper.ExtensionsTests;

namespace Ayx.Dapper.Extensions.Tests
{
    [TestClass()]
    public class SqlBuilderTests
    {
        [TestMethod()]
        public void GenerateSelectTest()
        {
            var sqlGen = new SqlBuilder();
            var type = typeof(TestModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("IntProperty", "IntField"),
                new DbFieldInfo("NotField").SetNotDbField());

            var test1 = sqlGen.GetSelect<TestModel>(tableInfo);
            var test2 = sqlGen.GetSelect<TestModel>(tableInfo, "");
            var test3 = sqlGen.GetSelect<TestModel>(tableInfo, "StringProperty", "IntField>34 AND StringProperty=@StringProperty ORDER BY IntField");

            var expected1 = "SELECT * FROM TestModel";
            var expected2 = "SELECT ID,StringProperty,IntField AS IntProperty FROM TestModel";
            var expected3 = "SELECT StringProperty FROM TestModel WHERE IntField>34 AND StringProperty=@StringProperty ORDER BY IntField";

            Assert.AreEqual(expected1, test1);
            Assert.AreEqual(expected2, test2);
            Assert.AreEqual(expected3, test3);
            Assert.AreEqual(3, sqlGen.CacheCount);

            var test4 = sqlGen.GetSelect<TestModel>(tableInfo, "StringProperty", "IntField>34 AND StringProperty=@StringProperty ORDER BY IntField");
            Assert.ReferenceEquals(test3, test4);
            Assert.AreEqual(3, sqlGen.CacheCount);
        }

        [TestMethod()]
        public void GenerateDeleteTest()
        {
            var gen = new SqlBuilder();
            var type = typeof(TestModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("IntProperty", "IntField"),
                new DbFieldInfo("NotField").SetNotDbField(),
                new DbFieldInfo("ID").SetPrimaryKey());

            var test1 = gen.GetDelete<TestModel>(tableInfo);
            var test2 = gen.GetDelete<TestModel>(tableInfo, "ID>10");
            var expected1 = "DELETE FROM TestModel WHERE ID=@ID";
            var expected2 = "DELETE FROM TestModel WHERE ID>10";

            Assert.AreEqual(expected1, test1);
            Assert.AreEqual(expected2, test2);
            Assert.AreEqual(2, gen.CacheCount);

            var test3 = gen.GetDelete<TestModel>(tableInfo, "ID>10");
            Assert.ReferenceEquals(test2, test3);
            Assert.AreEqual(2, gen.CacheCount);
        }

        [TestMethod()]
        public void GenerateDeleteTest1()
        {
            var gen = new SqlBuilder();
            var type = typeof(TestModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("IntProperty", "IntField"),
                new DbFieldInfo("NotField").SetNotDbField());

            try
            {
                var test1 = gen.GetDelete<TestModel>(tableInfo);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("can't find primary key when generate delete command!", e.Message);
            }

            try
            {
                var test2 = gen.GetDelete<TestModel>(null);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("can't find primary key when generate delete command!", e.Message);
            }
        }

        [TestMethod()]
        public void GenerateUpdateTest()
        {
            var gen = new SqlBuilder();
            var type = typeof(TestModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("ID").SetAutoIncrement().SetPrimaryKey(),
                new DbFieldInfo("IntProperty", "IntField"),
                new DbFieldInfo("NotField").SetNotDbField());

            var test1 = gen.GetUpdate<TestModel>(tableInfo);
            var test2 = gen.GetUpdate<TestModel>(tableInfo, "StringProperty=@StringProperty", "ID>9");
            var test3 = gen.GetUpdate<TestModel>(tableInfo, "IntProperty");

            var expected1 = "UPDATE TestModel SET StringProperty=@StringProperty,IntField=@IntProperty WHERE ID=@ID";
            var expected2 = "UPDATE TestModel SET StringProperty=@StringProperty WHERE ID>9";
            var expected3 = "UPDATE TestModel SET IntField=@IntProperty WHERE ID=@ID";

            Assert.AreEqual(expected1, test1);
            Assert.AreEqual(expected2, test2);
            Assert.AreEqual(expected3, test3);
            Assert.AreEqual(3, gen.CacheCount);

            var test4 = gen.GetUpdate<TestModel>(tableInfo);
            Assert.ReferenceEquals(test1, test4);
            Assert.AreEqual(3, gen.CacheCount);
        }
    }
}