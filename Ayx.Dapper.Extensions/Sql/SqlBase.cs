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

        public bool CheckDbProperty(PropertyInfo property)
        {
            if (!property.PropertyType.IsValueType && property.PropertyType != typeof(string))
                return false;

            if (CheckDbField(property))
                return true;

            return false;
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
            if (TableInfo != null)
                return TableInfo.TableName;

            var attrName = DbAttributes.GetTableName(ModelType);
            if (!string.IsNullOrEmpty(attrName))
                return attrName;

            return ModelType.Name;
        }

        public bool CheckDbField(PropertyInfo property)
        {
            if (TableInfo != null)
                return TableInfo.CheckDbField(property.Name);

            return DbAttributes.CheckDbField(property);
        }

        public bool CheckPrimaryKey(PropertyInfo property)
        {
            if (TableInfo != null)
                return TableInfo.CheckPrimaryKey(property.Name);

            return DbAttributes.CheckPrimaryKey(property);
        }

        public bool CheckAutoIncrement(PropertyInfo property)
        {
            if (TableInfo != null)
                return TableInfo.CheckAutoIncrement(property.Name);

            return DbAttributes.CheckAutoIncrement(property);
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
