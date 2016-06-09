using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public string GenerateDelete(Type type, DbTableInfo tableInfo, string where = null)
        {
            var cache = GetFromCache(type, "DELETE", null, where);
            if (cache != null) return cache.SqlString;

            var tableName = MakeTableName(type, tableInfo);
            var wherePart = MakeKeyWhere(type, tableInfo, where);
            if (string.IsNullOrEmpty(wherePart))
                throw new Exception("can't find primary key when generate delete command!");
            var sql = $"DELETE FROM {tableName}{wherePart}";
            AddToCache(type, "DELETE", null, where, sql);
            return sql;
        }

        public string GenerateUpdate(Type type, DbTableInfo tableInfo, string fields = null, string where = null)
        {
            var cache = GetFromCache(type, "UPDATE", fields, where);
            if (cache != null) return cache.SqlString;

            var tableName = MakeTableName(type, tableInfo);
            var fieldsPart = MakeUpdateFields(type, tableInfo, fields);
            var wherePart = MakeKeyWhere(type, tableInfo, where);

            if(string.IsNullOrEmpty(wherePart))
                throw new Exception("can't find primary key when generate delete command!");
            var sql = $"UPDATE {tableName} SET {fieldsPart}{wherePart}";
            AddToCache(type, "UPDATE", fields, where, sql);
            return sql;
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
                if (!CheckDbProperty(property))
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

        public static string MakeUpdateFields(Type modelType,DbTableInfo tableInfo, string fields)
        {
            var result = "";

            if (!string.IsNullOrEmpty(fields))
            {
                var fieldList = fields.Split(',');
                foreach (var field in fieldList)
                {
                    if (field.Contains("@") && field.Contains("="))
                    {
                        result += $"{field},";
                        continue;
                    }
                    result += MakeUpdateField(tableInfo, field);
                }
            }
            else
            {
                foreach (var property in modelType.GetProperties())
                {
                    if (!CheckDbProperty(property))
                        continue;

                    result += MakeUpdateField(tableInfo, property.Name);
                }
            }

            return result.Substring(0, result.Length - 1);
        }

        public static string MakeUpdateField(DbTableInfo tableInfo,string propertyName)
        {
            if (tableInfo != null)
            {
                var fieldInfo = tableInfo.GetField(propertyName);
                if (fieldInfo != null)
                {
                    if (fieldInfo.IsAutoIncrement) return "";
                    if (fieldInfo.NotDbField) return "";
                    return $"{fieldInfo.DbFieldName}=@{fieldInfo.PropertyName},";
                }
            }
            return $"{propertyName}=@{propertyName},";
        }

        public static string MakeWhere(string where)
        {
            if (string.IsNullOrEmpty(where))
                return "";
            return " WHERE " + where;
        }

        public static string MakeKeyWhere(Type type, DbTableInfo tableInfo, string where)
        {
            if (!string.IsNullOrEmpty(where))
                return MakeWhere(where);
            if (tableInfo == null)
                throw new Exception("can't find primary key when generate delete command!");

            foreach (var property in type.GetProperties())
            {
                var fieldInfo = tableInfo.GetField(property.Name);
                if (fieldInfo == null) continue;
                if (fieldInfo.IsPrimaryKey)
                    return " WHERE " + fieldInfo.DbFieldName + "=@" + fieldInfo.PropertyName;
            }

            return null;
        }

        public static string MakeTableName(Type modelType,DbTableInfo tableInfo)
        {
            if (tableInfo == null)
                return modelType.Name;
            return tableInfo.TableName;
        }

        public static bool CheckDbProperty(PropertyInfo property)
        {
            if (!property.PropertyType.IsValueType && property.PropertyType != typeof(string))
                return false;
            return true;
        }
    }
}
