using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ayx.Dapper.Extensions.Sql
{
    public class UpdateProvider:SqlBase
    {
        public string FieldsPart { get; set; }
        public string WherePart { get; set; }

        public UpdateProvider(Type type, DbTableInfo tableInfo, SqlCache cache)
            :base(type,tableInfo,cache)
        {
            Verb = "UPDATE";
        }

        protected override string GetParam()
        {
            return MakeParam(FieldsPart, WherePart);
        }

        protected override string MakeSQL()
        {
            var fields = GetUpdateFields();
            var where = GetKeyWhere(WherePart);
            return $"UPDATE {TableName} SET {fields}{where}";
        }

        public UpdateProvider Fields(string fields)
        {
            FieldsPart = fields;
            return this;
        }

        public UpdateProvider Where(string where)
        {
            WherePart = where;
            return this;
        }

        public string GetUpdateFields()
        {
            var result = "";

            if (!string.IsNullOrEmpty(FieldsPart))
            {
                var fieldList = FieldsPart.Split(',');
                foreach (var field in fieldList)
                {
                    if (field.Contains("@") && field.Contains("="))
                    {
                        result += $"{field},";
                        continue;
                    }
                    result += GetUpdateField(field);
                }
            }
            else
            {
                foreach (var property in ModelType.GetProperties())
                {
                    if (!CheckDbProperty(property))
                        continue;

                    result += GetUpdateField(property.Name);
                }
            }

            return result.Substring(0, result.Length - 1);
        }

        public string GetUpdateField(string propertyName)
        {
            if (TableInfo != null)
            {
                var fieldInfo = TableInfo.GetField(propertyName);
                if (fieldInfo != null)
                {
                    if (fieldInfo.IsAutoIncrement) return "";
                    if (fieldInfo.NotDbField) return "";
                    return $"{fieldInfo.DbFieldName}=@{fieldInfo.PropertyName},";
                }
            }
            return $"{propertyName}=@{propertyName},";
        }
    }
}
