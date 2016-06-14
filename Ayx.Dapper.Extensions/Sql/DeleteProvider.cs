using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ayx.Dapper.Extensions.Sql
{
    public class DeleteProvider<T> : SqlBase
    {
        public string WherePart { get; set; }

        public DeleteProvider(DbTableInfo tableInfo, SqlCache cache)
            : base(typeof(T), tableInfo, cache)
        {
            Verb = "DELETE";
        }

        public DeleteProvider<T> Where(string where)
        {
            WherePart = where;
            return this;
        }

        protected override string GetParam()
        {
            return MakeParam(WherePart);
        }

        protected override string MakeSQL()
        {
            var where = GetKeyWhere(WherePart);
            return $"DELETE FROM {TableName}{where}";
        }
    }
}
