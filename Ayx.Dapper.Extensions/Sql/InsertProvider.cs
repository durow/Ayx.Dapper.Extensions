using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ayx.Dapper.Extensions.Sql
{
    public class InsertProvider:SqlBase
    {
        public string FieldsPart { get; set; }
        public string ValuesPart { get; set; }
        public string Identity { get; set; }

        public InsertProvider(Type modelType, DbTableInfo tableInfo, SqlCache cache)
            :base(modelType,tableInfo,cache)
        {
            Verb = "INSERT";
        }

        protected override string GetParam()
        {
            return FieldsPart + Identity;
        }

        protected override string MakeSQL()
        {
            return $"INSERT INTO {TableName}({FieldsPart})";
        }
    }
}
