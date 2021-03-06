﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ayx.Dapper.Extensions.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ayx.Dapper.ExtensionsTests;

namespace Ayx.Dapper.Extensions.Sql.Tests
{
    [TestClass()]
    public class SelectProviderTests
    {
        [TestMethod()]
        public void GetSelectFieldsTest()
        {
            var type = typeof(AttributeModel);
            var tableInfo = new DbTableInfo(type).SetFields(
                new DbFieldInfo("IntProperty", "IntKey"),
                new DbFieldInfo("NotField").SetNotDbField());

            var select = new SelectProvider<AttributeModel>(null, null);
            var select2 = new SelectProvider<AttributeModel>(tableInfo, null);

            var test1 = select.Fields(null).GetSelectFields();
            var test2 = select.Fields("").GetSelectFields();
            var test3 = select.Fields("ID,StringProperty,IntProperty").GetSelectFields();
            var test4 = select.Fields("ID,StringProperty,IntField").GetSelectFields();
            var test5 = select2.Fields("").GetSelectFields();
            var test6 = select2.Fields("ID,StringProperty,IntProperty").GetSelectFields();

            var expected1 = @"ID,StringProperty,IntField AS IntProperty";
            var expected2 = @"ID,StringProperty,IntField";
            var expected3 = @"ID,StringProperty,IntKey AS IntProperty";

            Assert.AreEqual("*", test1);
            Assert.AreEqual(expected1, test2);
            Assert.AreEqual(expected1, test3);
            Assert.AreEqual(expected2, test4);
            Assert.AreEqual(expected3, test5);
            Assert.AreEqual(expected3, test6);
        }

        [TestMethod]
        public void GetSqlTest()
        {
            var select = new SelectProvider<AttributeModel>(null,null)
                .Distinct().Top(10).Fields("ID,IntProperty").Where("ID>20").OrderBy("ID").Desc();
            var actual = select.GetSQL();
            var expected = @"SELECT DISTINCT TOP10 ID,IntField AS IntProperty FROM TestModel WHERE ID>20 ORDER BY ID DESC";

            Assert.AreEqual(expected, actual);
        }
    }
}