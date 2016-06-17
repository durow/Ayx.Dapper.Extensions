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
        public string FieldsPart { get; private set; }
        public string WherePart { get; private set; }
        public bool IsDistinct { get; private set; }
        public string OrderByPart { get; private set; }
        public int TopPart { get; private set; } = 0;
        public string OrderTypePart { get; private set; }

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
            var distinct = GetDistinct();
            var top = GetTop();
            var order = GetOrderBy();

            return $"SELECT {distinct}{top}{fields} FROM {TableName}{where}{order}";
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

        public SelectProvider<T> Distinct()
        {
            IsDistinct = true;
            return this;
        }

        public SelectProvider<T> OrderBy(string orderBy)
        {
            OrderByPart = orderBy;
            return this;
        }

        public SelectProvider<T> Top(int top)
        {
            TopPart = top;
            return this;
        }

        public SelectProvider<T> Desc()
        {
            OrderTypePart = "DESC";
            return this;
        }

        public SelectProvider<T> Asc()
        {
            OrderTypePart = "ASC";
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

        public string GetTop()
        {
            if (TopPart > 0)
                return $"TOP{TopPart} ";
            else
                return "";
        }

        public string GetDistinct()
        {
            if (IsDistinct)
                return "DISTINCT ";
            else
                return "";
        }

        public string GetOrderBy()
        {
            if (string.IsNullOrEmpty(OrderByPart))
                return "";
            else
                return $" ORDER BY {OrderByPart} {OrderTypePart}";
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
