using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ayx.Dapper.Extensions.Sql
{
    public class SelectProvider:SqlBase
    {
        public string FieldsPart { get; set; }
        public string WherePart { get; set; }

        public SelectProvider(Type type, DbTableInfo tableInfo, SqlCache cache)
            : base(type, tableInfo, cache)
        {
            Verb = "SELECT";
        }

        protected override string GetParam()
        {
            return FieldsPart + WherePart;
        }

        protected override string MakeSQL()
        {
            return $"SELECT {FieldsPart} FROM {TableName}{WherePart}";
        }

        public SelectProvider Where(string where)
        {
            WherePart = GetWhere(where);
            return this;
        }

        public SelectProvider Fields(string fields)
        {
            FieldsPart = GetSelectFields(fields);
            return this;
        }

        public string GetSelectFields(string fields)
        {
            if (fields == null)
                return "*";
            if (!string.IsNullOrEmpty(fields))
                return fields;

            var result = "";
            foreach (var property in ModelType.GetProperties())
            {
                if (!CheckDbProperty(property))
                    continue;

                if (TableInfo == null)
                {
                    result += property.Name + ",";
                    continue;
                }
                var fieldInfo = TableInfo.GetField(property.Name);
                if (fieldInfo != null)
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
    }
}
