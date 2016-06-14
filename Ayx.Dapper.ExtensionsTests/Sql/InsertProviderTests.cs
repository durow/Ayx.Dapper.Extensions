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
    public class InsertProviderTests
    {
        [TestMethod()]
        public void GetInsertFieldsTest()
        {
            var type = typeof(TestModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("IntProperty", "IntField"),
                new DbFieldInfo("NotField").SetNotDbField(),
                new DbFieldInfo("ID").SetPrimaryKey().SetAutoIncrement());
            var insert = new InsertProvider<TestModel>(tableInfo, new SqlCache());

            var actual1 = insert.GetSQL();
            var expected1 = @"INSERT INTO TestModel(StringProperty,IntField) VALUES(@StringProperty,@IntProperty)";
            Assert.AreEqual(expected1, actual1);
        }

        [TestMethod()]
        public void GetInsertFieldsTest2()
        {
            var type = typeof(TestModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("IntProperty", "IntField"),
                new DbFieldInfo("NotField").SetNotDbField(),
                new DbFieldInfo("ID").SetPrimaryKey().SetAutoIncrement());
            var insert = new InsertProvider<TestModel>(tableInfo, new SqlCache()).Fields("IntProperty");

            var actual2 = insert.GetSQL();
            var expected2 = @"INSERT INTO TestModel(IntField) VALUES(@IntProperty)";

            Assert.AreEqual(expected2, actual2);
        }

        [TestMethod()]
        public void MakeFieldsTest()
        {
            var type = typeof(AttributeModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("IntProperty", "IntKey"),
                new DbFieldInfo("NotField").SetNotDbField(),
                new DbFieldInfo("ID").SetPrimaryKey().SetAutoIncrement());

            var insert = new InsertProvider<AttributeModel>(null, null);
            var insert2 = new InsertProvider<TestModel>(tableInfo, null);
            var test1 = insert.MakeEmptyFields();
            var test2 = insert2.MakeEmptyFields();
            var test3 = insert.Fields("IntProperty").MakeFields();
            var test4 = insert2.Fields("IntProperty").MakeFields();

            Assert.AreEqual("StringProperty,IntField", test1.Fields);
            Assert.AreEqual("@StringProperty,@IntProperty", test1.Values);
            Assert.AreEqual("StringProperty,IntKey", test2.Fields);
            Assert.AreEqual("@StringProperty,@IntProperty", test2.Values);
            Assert.AreEqual("IntField", test3.Fields);
            Assert.AreEqual("@IntProperty", test3.Values);
            Assert.AreEqual("IntKey", test4.Fields);
            Assert.AreEqual("@IntProperty", test4.Values);
        }
    }
}