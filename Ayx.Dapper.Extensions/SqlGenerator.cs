using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ayx.Dapper.Extensions
{
    public class SqlGenerator
    {
        private List<SqlStringInfo> Cache = new List<SqlStringInfo>();

        public string GetSelect()
        {
            return "";
        }

        private void AddToCache()
        { }

        private SqlStringInfo GetFromCache(Type modelType,string verb,string param1,string param2)
        {
            return Cache
                .Where(c => c.ModelType == modelType &&
                                  c.Verb == verb &&
                                  c.Param1 == param1 &&
                                  c.Param2 == param2)
                .FirstOrDefault();
        }
    }
}
