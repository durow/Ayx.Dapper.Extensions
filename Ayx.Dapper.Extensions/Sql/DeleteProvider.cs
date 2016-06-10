using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ayx.Dapper.Extensions.Sql
{
    public class DeleteProvider : SqlBase
    {
        public string WherePart { get; set; }

        public DeleteProvider(Type type, DbTableInfo tableInfo, SqlCache cache)
            : base(type, tableInfo, cache)
        {
            Verb = "DELETE";
        }

        public DeleteProvider Where(string where)
        {
            WherePart = GetKeyWhere(where);
            return this;
        }

        protected override string GetParam()
        {
            return WherePart;
        }

        protected override string MakeSQL()
        {
            return $"DELETE FROM {TableName}{WherePart}";
        }
    }
}
