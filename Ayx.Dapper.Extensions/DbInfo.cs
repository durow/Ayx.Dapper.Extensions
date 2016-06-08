using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Ayx.Dapper.Extensions
{
    public class DbInfo
    {

        private static DbInfo _default;
        public static DbInfo Default
        {
            get
            {
                if (_default == null)
                    _default = new DbInfo();
                return _default;
            } }

        public Func<IDbConnection> CreateConnection { get; set; }
        public int Count { get { return tableList.Count; } }

        private List<DbTableInfo> tableList = new List<DbTableInfo>();
        private SqlGenerator SqlGenerator = new SqlGenerator();

        public DbInfo(Func<IDbConnection> createConnection = null)
        {
            CreateConnection = createConnection;
        }
        public DbTableInfo Register<TModel>(string tableName = null,string token = null)
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

        public DbTableInfo GetTable(Type type, string token = null)
        {
            return tableList
                .Where(p => p.ModelType == type && p.Token == token)
                .FirstOrDefault();
        }

        public DbTableInfo GetTable<TModel>(string token=null)
        {
            return GetTable(typeof(TModel), token);
        }

        public void Clear()
        {
            tableList.Clear();
        }
    }
}
