using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ayx.Dapper.Extensions.Sql
{
    public class SqlCache
    {
        public int Count { get { return sqlList.Count; } }
        private List<SqlInfo> sqlList = new List<SqlInfo>();

        public void Add(Type modelType, string verb, string param1, string param2, string sql)
        {
            sqlList.Add(new SqlInfo
            {
                ModelType = modelType,
                Verb = verb,
                Param1 = param1,
                Param2 = param2,
                SqlString = sql,
            });
        }

        public void Add(Type modelType, string verb, string param, string sql)
        {
            sqlList.Add(new SqlInfo
            {
                ModelType = modelType,
                Verb = verb,
                Param = param,
                SqlString = sql,
            });
        }

        public SqlInfo Get(Type modelType, string verb, string param1, string param2)
        {
            return sqlList
                .Where(c => c.ModelType == modelType &&
                                  c.Verb == verb &&
                                  c.Param1 == param1 &&
                                  c.Param2 == param2)
                .FirstOrDefault();
        }

        public SqlInfo Get(Type modelType, string verb, string param)
        {
            return sqlList
                .Where(c => c.ModelType == modelType &&
                                  c.Verb == verb &&
                                  c.Param == param)
                .FirstOrDefault();
        }
    }
}
