using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var result = new List<string>();

            if (string.IsNullOrEmpty(FieldsPart))
            {
                
                foreach (var property in ModelType.GetProperties())
                {
                    result.Add(GetUpdateField(property));
                }
            }
            else
            {
                foreach (var field in FieldsPart.Split(','))
                {
                    result.Add(GetUpdateField(field));
                }
            }
            return JoinFields(result);
        }

        public string GetUpdateField(string propertyName)
        {
            var property = GetProperty(propertyName);
            if(property != null)
            {
                return GetUpdateField(property);
            }
            return null;
        }

        public string GetUpdateField(PropertyInfo property)
        {
            if (!CheckDbProperty(property))
                return null;
            if (CheckAutoIncrement(property))
                return null;
            
            var dbFieldName = GetFieldName(property);
            return $"{dbFieldName}=@{property.Name}";
        }
    }
}
