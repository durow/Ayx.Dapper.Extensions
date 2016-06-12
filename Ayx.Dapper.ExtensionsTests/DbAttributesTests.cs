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
    public class DbAttributesTests
    {
        [TestMethod()]
        public void GetTableNameTest()
        {
            var actual = DbAttributes.GetTableName(typeof(AttributeModel));
            var expected = "TestModel";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetDbFieldInfoTest()
        {
            var type = typeof(AttributeModel);

            var property = type.GetProperties().Where(p => p.Name == "IntProperty").FirstOrDefault();
            if (property == null)
                Assert.Fail();
            var attr = DbAttributes.GetDbFieldInfo(property);
            if (attr == null)
                Assert.Fail();
            Assert.AreEqual("IntField", attr.FieldName);
        }

        [TestMethod()]
        public void CheckDbFieldTest()
        {
            var type = typeof(AttributeModel);

            var intProperty = type.GetProperties().Where(p => p.Name == "IntProperty").FirstOrDefault();
            var noDbProperty = type.GetProperties().Where(p => p.Name == "NotField").FirstOrDefault();

            if (intProperty == null || noDbProperty == null)
                Assert.Fail();

            Assert.IsTrue(DbAttributes.CheckDbField(intProperty));
            Assert.IsFalse(DbAttributes.CheckDbField(noDbProperty));
        }

        [TestMethod()]
        public void CheckAutoIncrementTest()
        {
            var type = typeof(AttributeModel);

            var intProperty = type.GetProperties().Where(p => p.Name == "IntProperty").FirstOrDefault();
            var idProperty = type.GetProperties().Where(p => p.Name == "ID").FirstOrDefault();

            if (intProperty == null || idProperty == null)
                Assert.Fail();

            Assert.IsFalse(DbAttributes.CheckAutoIncrement(intProperty));
            Assert.IsTrue(DbAttributes.CheckAutoIncrement(idProperty));
        }

        [TestMethod()]
        public void CheckPrimaryKeyTest()
        {
            var type = typeof(AttributeModel);

            var intProperty = type.GetProperties().Where(p => p.Name == "IntProperty").FirstOrDefault();
            var idProperty = type.GetProperties().Where(p => p.Name == "ID").FirstOrDefault();

            if (intProperty == null || idProperty == null)
                Assert.Fail();

            Assert.IsFalse(DbAttributes.CheckPrimaryKey(intProperty));
            Assert.IsTrue(DbAttributes.CheckPrimaryKey(idProperty));
        }
    }
}