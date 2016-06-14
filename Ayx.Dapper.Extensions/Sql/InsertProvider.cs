using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ayx.Dapper.Extensions.Sql
{
    public class InsertProvider:SqlBase
    {
        public string FieldsPart { get; private set; }
        public bool Identity { get; private set; }

        public InsertProvider(Type modelType, DbTableInfo tableInfo, SqlCache cache)
            :base(modelType,tableInfo,cache)
        {
            Verb = "INSERT";
        }

        public InsertProvider Fields(string fields)
        {
            FieldsPart = fields;
            return this;
        }

        public InsertProvider ReturnIdentity()
        {
            Identity = true;
            return this;
        }

        protected override string GetParam()
        {
            return MakeParam(FieldsPart, Identity.ToString());
        }

        protected override string MakeSQL()
        {
            var fieldsAndValues = GetInsertFields();
            var identity = GetIdentity();
            return $"INSERT INTO {TableName}({fieldsAndValues.Fields}) VALUES({fieldsAndValues.Values}){identity}";
        }

        public FieldsAndValues GetInsertFields()
        {

            if(string.IsNullOrEmpty(FieldsPart))
                return MakeEmptyFields();
            else
                return MakeFields();
        }

        public FieldsAndValues MakeEmptyFields()
        {
            var result = new FieldsAndValues();
            foreach (var property in ModelType.GetProperties())
            {
                if (!CheckDbProperty(property))
                    continue;
                if (CheckAutoIncrement(property))
                    continue;

                var dbField = GetFieldName(property);
                result.Add(property.Name, dbField);
            }
            return result;
        }

        public FieldsAndValues MakeFields()
        {
            var result = new FieldsAndValues();
            foreach (var field in FieldsPart.Split(','))
            {
                if(TableInfo != null)
                {
                    var fieldInfo = TableInfo.GetField(field);
                    if(fieldInfo != null)
                    {
                        if (fieldInfo.NotDbField) continue;
                        if (fieldInfo.IsAutoIncrement) continue;

                        result.Add(field, fieldInfo.DbFieldName);
                        continue;
                    }
                }
                var property = GetProperty(field);
                if (property == null)
                    result.Add(field);
                else
                {
                    if (!DbAttributes.CheckDbField(property))
                        continue;
                    if (DbAttributes.CheckAutoIncrement(property))
                        continue;
                    var dbFieldName = DbAttributes.GetDbFieldName(property);
                    result.Add(property.Name, dbFieldName);
                }
            }
            return result;
        }

        public string GetIdentity()
        {
            if (!Identity) return "";

            return ";SELECT @@IDENTITY";
        }
    }

    public sealed class FieldsAndValues
    {
        public string Fields
        {
            get
            {
                return SqlBase.JoinFields(_fieldList);
            }
        }
        public string Values
        {
            get
            {
                return SqlBase.JoinFields(_valuesList);
            }
        }

        private List<string> _fieldList = new List<string>();
        private List<string> _valuesList = new List<string>();

        public FieldsAndValues Add(string propertyName, string fieldName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                return this;

            if (string.IsNullOrEmpty(fieldName))
                fieldName = propertyName;

            _fieldList.Add(fieldName);
            _valuesList.Add("@" + propertyName);
            return this;
        }
    }
}
