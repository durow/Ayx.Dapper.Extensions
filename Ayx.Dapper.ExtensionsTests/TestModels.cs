using Ayx.Dapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ayx.Dapper.ExtensionsTests
{
    public class TestModel
    {
        public int ID { get; set; }
        public string StringProperty{ get; set; }
        public int IntProperty { get; set; }
        public string NotField { get; set; }
        public MapModel Model { get; set; }
    }

    public class MapModel
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
    }

    [DbTable("TestModel")]
    public class AttributeModel
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        public string StringProperty { get; set; }

        [DbField("IntField")]
        public int IntProperty { get; set; }

        [NotDbField]
        public string NotField { get; set; }
        public MapModel Model { get; set; }
    }
}
