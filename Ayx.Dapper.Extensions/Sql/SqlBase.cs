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
        #region Properties

        public string Verb { get; set; }
        public string TableName { get; set; }
        public Type ModelType { get; set; }
        public DbTableInfo TableInfo { get; set; }
        public IDbConnection Connection { get; set; }
        public SqlCache Cache { get; set; }

        #endregion

        public SqlBase(Type type, DbTableInfo tableInfo, SqlCache cache)
        {
            ModelType = type;
            TableInfo = tableInfo;
            Cache = cache;
            TableName = GetTableName();
            if (Cache == null)
                Cache = new SqlCache();
        }

        #region Abstract Methods

        protected abstract string GetParam();
        protected abstract string MakeSQL();

        #endregion

        #region Public Methods

        public string GetSQL()
        {
            var param = GetParam();
            var cache = Cache.Get(ModelType, Verb, param);
            if (cache != null) return cache.SqlString;

            var sql = MakeSQL();
            Cache.Add(ModelType, Verb, param, sql);
            return sql;
        }

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

        public string GetKeyWhere(string where = null)
        {
            if (!string.IsNullOrEmpty(where))
                return GetWhere(where);

            if (TableInfo != null)
            {
                var field = TableInfo.GetPrimaryKeyField();
                if (field != null)
                    return " WHERE " + field.DbFieldName + "=@" + field.PropertyName;
            }

            foreach (var property in ModelType.GetProperties())
            {
                if (!DbAttributes.CheckPrimaryKey(property))
                    continue;

                var fieldName = property.Name;
                var attr = DbAttributes.GetDbFieldInfo(property);
                if (attr != null)
                    fieldName = attr.FieldName;

                return " WHERE " + fieldName + "=@" + property.Name;
            }

            throw new Exception("can't find primary key when generate delete command!");
        }

        public string GetFields(string fields)
        {
            var result = new List<string>();

            if (string.IsNullOrEmpty(fields))
            {
                foreach (var property in ModelType.GetProperties())
                {
                    result.Add(GetFieldName(property));
                }
            }
            else
            {
                foreach (var field in fields.Split(','))
                {
                    var property = GetProperty(field);
                    if (property == null)
                        result.Add(field);
                    else
                        result.Add(GetFieldName(property));
                }
            }

            return JoinFields(result);
        }

        public string GetFieldName(PropertyInfo property)
        {
            if (property == null) return null;

            if (!CheckDbProperty(property))
                return null;

            if (TableInfo != null)
            {
                return TableInfo.GetDbFieldName(property.Name);
            }
            return DbAttributes.GetDbFieldName(property);
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

        public PropertyInfo GetProperty(string propertyName)
        {
            if (ModelType == null) return null;

            return ModelType
                .GetProperties()
                .Where(p => p.Name == propertyName)
                .FirstOrDefault();
        }

        public static string JoinFields(List<string> fields, string separator = ",")
        {
            var sb = new StringBuilder();
            foreach (var field in fields)
            {
                if (string.IsNullOrEmpty(field))
                    continue;
                sb.Append(field).Append(separator);
            }
            if (sb.Length > 0) ;
            return sb.ToString(0, sb.Length - 1);
        }

        #endregion
    }
}
