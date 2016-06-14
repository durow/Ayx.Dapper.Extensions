using Ayx.Dapper.Extensions.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ayx.Dapper.Extensions
{
    public class SqlBuilder
    {
        private SqlCache sqlCache = new SqlCache();
        public int CacheCount { get { return sqlCache.Count; } }

        public string GetSelect<T>(DbTableInfo tableInfo, string fields=null, string where = null)
        {
            return Select<T>( tableInfo).Fields(fields).Where(where).GetSQL();
        }

        public SelectProvider<T> Select<T>(DbTableInfo tableInfo)
        {
            return new SelectProvider<T>(tableInfo, sqlCache);
        }

        public string GetDelete<T>(DbTableInfo tableInfo, string where = null)
        {
            return Delete<T>(tableInfo).Where(where).GetSQL();
        }

        public DeleteProvider<T> Delete<T>(DbTableInfo tableInfo)
        {
            return new DeleteProvider<T>(tableInfo, sqlCache);
        }

        public string GetUpdate<T>(DbTableInfo tableInfo, string fields = null, string where = null)
        {
            return Update<T>(tableInfo).Fields(fields).Where(where).GetSQL();
        }

        public UpdateProvider<T> Update<T>(DbTableInfo tableInfo)
        {
            return new UpdateProvider<T>(tableInfo,sqlCache);
        }

        public string GetInsert<T>(DbTableInfo tableInfo, string fields = null)
        {
            return Insert<T>(tableInfo).Fields(fields).GetSQL();
        }

        public InsertProvider<T> Insert<T>(DbTableInfo tableInfo)
        {
            return new InsertProvider<T>(tableInfo, sqlCache);
        } 
    }
}
