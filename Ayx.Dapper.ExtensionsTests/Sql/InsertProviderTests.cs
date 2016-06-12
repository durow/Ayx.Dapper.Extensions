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
            var insert = new InsertProvider(typeof(TestModel), tableInfo, new SqlCache());

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
            var insert = new InsertProvider(typeof(TestModel), tableInfo, new SqlCache()).Fields("IntProperty");

            var actual2 = insert.GetSQL();
            var expected2 = @"INSERT INTO TestModel(IntField) VALUES(@IntProperty)";

            Assert.AreEqual(expected2, actual2);
        }
    }
}