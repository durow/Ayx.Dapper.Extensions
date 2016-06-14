using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ayx.Dapper.Extensions.Sql
{
    public class SelectProvider<T> : SqlBase
    {
        public string FieldsPart { get; set; }
        public string WherePart { get; set; }

        public SelectProvider(DbTableInfo tableInfo, SqlCache cache)
            : base(typeof(T), tableInfo, cache)
        {
            Verb = "SELECT";
        }

        protected override string GetParam()
        {
            return MakeParam(FieldsPart, WherePart);
        }

        protected override string MakeSQL()
        {
            var fields = GetSelectFields();
            var where = GetWhere(WherePart);
            return $"SELECT {fields} FROM {TableName}{where}";
        }

        public SelectProvider<T> Where(string where)
        {
            WherePart = where;
            return this;
        }

        public SelectProvider<T> Fields(string fields)
        {
            FieldsPart = fields;
            return this;
        }

        public string GetSelectFields()
        {
            if (FieldsPart == null)
                return "*";

            var result = new List<string>();
            if (string.IsNullOrEmpty(FieldsPart))
            {
                foreach (var property in ModelType.GetProperties())
                {
                    result.Add(GetSelectField(property));
                }
            }
            else
            {
                foreach (var field in FieldsPart.Split(','))
                {
                    result.Add(GetSelectField(field));
                }
            }
            return JoinFields(result);
        }

        public string GetSelectField(PropertyInfo property)
        {
            if (!CheckDbProperty(property)) return "";

            var dbField = GetFieldName(property);

            if (string.IsNullOrEmpty(dbField))
                return property.Name;

            if (dbField != property.Name)
                return dbField + " AS " + property.Name;

            return property.Name;
        }

        public string GetSelectField(string field)
        {
            if (TableInfo != null)
            {
                var dbField = TableInfo.GetField(field);
                if (dbField != null && dbField.DbFieldName != field)
                    return dbField.DbFieldName + " AS " + field;
                else
                    return field;
            }

            var property = GetProperty(field);
            if (property != null)
            {
                var dbField = DbAttributes.GetDbFieldName(property);
                if (dbField == field)
                    return field;
                else
                    return dbField + " AS " + field;
            }

            return field;
        }

        public IEnumerable<T> Go(
            object param = null, 
            IDbTransaction transaction = null, 
            bool buffered = true,
            int? timeOut = null, 
            CommandType? commandType = null)
        {
            var sql = GetSQL();
            return Connection.Query<T>(sql, param, transaction, buffered, timeOut, commandType);
        }
    }
}
