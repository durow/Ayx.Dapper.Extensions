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

        private string fieldsPart;
        private string valuesPart;

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
            GetInsertFields();
            var identity = GetIdentity();
            return $"INSERT INTO {TableName}({fieldsPart}) VALUES({valuesPart}){Identity}";
        }

        public void GetInsertFields()
        {

            if(string.IsNullOrEmpty(FieldsPart))
                MakeEmptyFields();
            else
                MakeFields();

            if (this.fieldsPart.Length > 0)
                this.fieldsPart = this.fieldsPart.Substring(0, this.fieldsPart.Length - 1);
            if (valuesPart.Length > 0)
                valuesPart = valuesPart.Substring(0, valuesPart.Length - 1);
        }

        private void MakeEmptyFields()
        {
            foreach (var property in ModelType.GetProperties())
            {
                if (!CheckDbProperty(property))
                    continue;

                if (TableInfo != null)
                {
                    var field = TableInfo.GetField(property.Name);
                    if (field != null)
                    {
                        if (field.NotDbField) continue;
                        if (field.IsAutoIncrement) continue;

                        AddField(field.PropertyName, field.DbFieldName);
                        continue;
                    }
                }

                AddField(property.Name);
            }
        }

        private void MakeFields()
        {
            foreach (var field in FieldsPart.Split(','))
            {
                if(TableInfo != null)
                {
                    var fieldInfo = TableInfo.GetField(field);
                    if(fieldInfo != null)
                    {
                        if (fieldInfo.NotDbField) continue;
                        if (fieldInfo.IsAutoIncrement) continue;

                        AddField(fieldInfo.PropertyName, fieldInfo.DbFieldName);
                        continue;
                    }
                }
                AddField(field);
            }
        }

        private void AddField(string propertyName, string fieldName = null)
        {
            if (string.IsNullOrEmpty(fieldName))
                fieldName = propertyName;

            fieldsPart += fieldName + ",";
            valuesPart += $"@{propertyName},";
        }

        public string GetIdentity()
        {
            if (!Identity) return "";

            return ";SELECT @@IDENTITY";
        }
    }
}
