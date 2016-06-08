using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ayx.Dapper.Extensions
{
    public class SqlGenerator
    {

        public int CacheCount { get { return Cache.Count; } }
        private List<SqlStringInfo> Cache = new List<SqlStringInfo>();

        public string GenerateSelect(Type type, DbTableInfo tableInfo, string fields=null, string where = null)
        {
            var cache = GetFromCache(type, "SELECT", fields, where);
            if (cache != null) return cache.SqlString;

            var tableName = MakeTableName(type, tableInfo);
            var fieldsPart = MakeSelectFields(type,tableInfo,fields);
            var wherePart = MakeWhere(where);
            var result = $"SELECT {fieldsPart} FROM {tableName}{wherePart}";
            AddToCache(type, "SELECT", fields, where, result);

            return result;
        }

        public void AddToCache(Type modelType,string verb,string param1,string param2,string sql)
        {
            Cache.Add(new SqlStringInfo
            {
                ModelType = modelType,
                Verb = verb,
                Param1 = param1,
                Param2 = param2,
                SqlString = sql,
            });
        }

        public SqlStringInfo GetFromCache(Type modelType,string verb,string param1,string param2)
        {
            return Cache
                .Where(c => c.ModelType == modelType &&
                                  c.Verb == verb &&
                                  c.Param1 == param1 &&
                                  c.Param2 == param2)
                .FirstOrDefault();
        }

        public static string MakeSelectFields(Type modelType, DbTableInfo tableInfo, string fields)
        {
            if (fields == null)
                return "*";
            if (!string.IsNullOrEmpty(fields))
                return fields;

            var result = "";
            foreach (var property in modelType.GetProperties())
            {
                if (!property.PropertyType.IsValueType && property.PropertyType != typeof(string))
                    continue;

                if(tableInfo == null)
                {
                    result += property.Name + ",";
                    continue;
                }
                var fieldInfo = tableInfo.GetField(property.Name);
                if(fieldInfo != null)
                {
                    if (fieldInfo.NotDbField)
                        continue;
                    result += fieldInfo.DbFieldName + ",";
                    continue;
                }
                result += property.Name + ",";
            }
            return result.Substring(0, result.Length - 1);
        }

        public static string MakeWhere(string where)
        {
            if (string.IsNullOrEmpty(where))
                return "";
            return " WHERE " + where;
        }

        public static string MakeTableName(Type modelType,DbTableInfo tableInfo)
        {
            if (tableInfo == null)
                return modelType.Name;
            return tableInfo.TableName;
        }
    }
}
