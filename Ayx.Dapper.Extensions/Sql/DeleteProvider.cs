using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

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

        public int Go(
            object param = null,
            IDbTransaction transaction = null,
            int? timeOut = null,
            CommandType? commandType = null)
        {
            var sql = GetSQL();
            return Connection.Execute(sql, param, transaction, timeOut, commandType);
        }
    }
}
