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
