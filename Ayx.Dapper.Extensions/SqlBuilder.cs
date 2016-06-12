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


        public string GetSelect(Type type, DbTableInfo tableInfo, string fields=null, string where = null)
        {
            return Select(type, tableInfo).Fields(fields).Where(where).GetSQL();
        }

        public SelectProvider Select(Type type,DbTableInfo tableInfo)
        {
            return new SelectProvider(type, tableInfo, sqlCache);
        }

        public string GetDelete(Type type, DbTableInfo tableInfo, string where = null)
        {
            return Delete(type, tableInfo).Where(where).GetSQL();
        }

        public DeleteProvider Delete(Type type, DbTableInfo tableInfo)
        {
            return new DeleteProvider(type, tableInfo, sqlCache);
        }

        public string GetUpdate(Type type, DbTableInfo tableInfo, string fields = null, string where = null)
        {
            return Update(type, tableInfo).Fields(fields).Where(where).GetSQL();
        }

        public UpdateProvider Update(Type type, DbTableInfo tableInfo)
        {
            return new UpdateProvider(type, tableInfo,sqlCache);
        }

        public string GetInsert(Type type, DbTableInfo tableInfo, string fields = null)
        {
            return Insert(type, tableInfo).Fields(fields).GetSQL();
        }

        public InsertProvider Insert(Type type, DbTableInfo tableInfo)
        {
            return new InsertProvider(type, tableInfo, sqlCache);
        } 
    }
}
