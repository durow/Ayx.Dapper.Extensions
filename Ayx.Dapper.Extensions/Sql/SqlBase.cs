using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ayx.Dapper.Extensions.Sql
{
    public abstract class SqlBase
    {
        public string Verb { get; set; }
        public string TableName { get; set; }
        public Type ModelType { get; set; }
        public DbTableInfo TableInfo { get; set; }
        public IDbConnection Connection { get; set; }
        public SqlCache Cache { get; set; }

        public SqlBase(Type type, DbTableInfo tableInfo, SqlCache cache)
        {
            ModelType = type;
            TableInfo = tableInfo;
            Cache = cache;
            TableName = GetTableName();
        }

        public string GetSQL()
        {
            var param = GetParam();
            var cache = Cache.Get(ModelType, Verb, param);
            if (cache != null) return cache.SqlString;

            var sql = MakeSQL();
            Cache.Add(ModelType, Verb, param, sql);
            return sql;
        }

        protected abstract string GetParam();
        protected abstract string MakeSQL();

        public static bool CheckDbProperty(PropertyInfo property)
        {
            if (!property.PropertyType.IsValueType && property.PropertyType != typeof(string))
                return false;
            return true;
        }

        public static string GetWhere(string where)
        {
            if (string.IsNullOrEmpty(where))
                return "";
            return " WHERE " + where;
        }

        public string GetKeyWhere(string where)
        {
            if (!string.IsNullOrEmpty(where))
                return GetWhere(where);
            if (TableInfo == null)
                throw new Exception("can't find primary key when generate delete command!");

            foreach (var property in ModelType.GetProperties())
            {
                var fieldInfo = TableInfo.GetField(property.Name);
                if (fieldInfo == null) continue;
                if (fieldInfo.IsPrimaryKey)
                    return " WHERE " + fieldInfo.DbFieldName + "=@" + fieldInfo.PropertyName;
            }

            throw new Exception("can't find primary key when generate delete command!");
        }

        public string GetTableName()
        {
            if (TableInfo == null)
                return ModelType.Name;
            return TableInfo.TableName;
        }

        public string MakeParam(params string[] paramList)
        {
            var result = "";
            foreach (var param in paramList)
            {
                if (param == null)
                    result += "NULL";
                else
                    result += param;
            }
            return result;
        }
    }
}
