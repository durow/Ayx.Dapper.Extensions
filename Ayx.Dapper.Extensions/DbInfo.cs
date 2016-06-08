using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ayx.Dapper.Extensions
{
    public static class DbInfo
    {
        public static int Count { get { return tableList.Count; } }
        private static List<DbTableInfo> tableList = new List<DbTableInfo>();

        public static DbTableInfo Register<TModel>(string tableName = null,string token = null)
        {
            var type = typeof(TModel);
            if (string.IsNullOrEmpty(tableName))
                tableName = type.Name;

            var table = GetTable<TModel>();
            if (table != null)
                throw new Exception("Model has existed");

            table = new DbTableInfo(type, tableName);
            tableList.Add(table);

            return table;
        }

        public static DbTableInfo GetTable(Type type, string token = null)
        {
            return tableList
                .Where(p => p.ModelType == type && p.Token == token)
                .FirstOrDefault();
        }

        public static DbTableInfo GetTable<TModel>(string token=null)
        {
            return GetTable(typeof(TModel), token);
        }

        public static void Clear()
        {
            tableList.Clear();
        }
    }
}
