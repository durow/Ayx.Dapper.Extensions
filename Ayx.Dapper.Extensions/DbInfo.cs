using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Dapper;
using Ayx.Dapper.Extensions.Sql;

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
        public SqlBuilder SqlBuilder { get; set; }

        public DbInfo(Func<IDbConnection> createConnection = null)
        {
            CreateConnection = createConnection;
            SqlBuilder = new SqlBuilder();
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

        #region Database Methods

        public SelectProvider<T> Select<T>(IDbConnection connection = null)
        {
            if (connection == null)
                connection = CreateConnection();

            var tableInfo = GetTable<T>();
            var result = SqlBuilder.Select<T>(tableInfo);
            result.Connection = connection;
            return result;
        }

        public DeleteProvider<T> Delete<T>(IDbConnection connection = null)
        {
            if (connection == null)
                connection = CreateConnection();

            var tableInfo = GetTable<T>();
            var result = SqlBuilder.Delete<T>(tableInfo);
            result.Connection = connection;
            return result;
        }

        public InsertProvider<T> Insert<T>(IDbConnection connection = null)
        {
            if (connection == null)
                connection = CreateConnection();

            var tableInfo = GetTable<T>();
            var result = SqlBuilder.Insert<T>(tableInfo);
            result.Connection = connection;
            return result;
        }

        public UpdateProvider<T> Update<T>(IDbConnection connection = null)
        {
            if (connection == null)
                connection = CreateConnection();

            var tableInfo = GetTable<T>();
            var result = SqlBuilder.Update<T>(tableInfo);
            result.Connection = connection;
            return result;
        }

        #endregion
    }
}
