using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ayx.Dapper.Extensions
{
    public class DbTableInfo
    {
        public string Token { get; private set; }
        public Type ModelType { get; set; }
        public string TableName { get; private set; }
        public List<DbFieldInfo> FieldInfoList { get; private set; }

        public DbTableInfo(Type modelType, string tableName = null, string token = null)
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = modelType.Name;

            ModelType = modelType;
            TableName = tableName;
            FieldInfoList = new List<DbFieldInfo>();
            Token = token;
        }

        public DbTableInfo SetFields(params DbFieldInfo[] fields)
        {
            FieldInfoList.AddRange(fields);
            return this;
        }

        public DbFieldInfo GetField(string propertyName)
        {
            return FieldInfoList
                .Where(f => f.PropertyName == propertyName)
                .FirstOrDefault();
        }

        public bool CheckDbField(string propertyName)
        {
            var fieldInfo = GetField(propertyName);
            if (fieldInfo != null)
                return !fieldInfo.NotDbField;
            else
                return true;
        }

        public bool CheckPrimaryKey(string propertyName)
        {
            var fieldInfo = GetField(propertyName);
            if (fieldInfo != null)
                return fieldInfo.IsPrimaryKey;
            else
                return false;
        }

        public bool CheckAutoIncrement(string propertyName)
        {
            var fieldInfo = GetField(propertyName);
            if (fieldInfo != null)
                return fieldInfo.IsAutoIncrement;
            else
                return false;
        }

        public string GetDbFieldName(string propertyName)
        {
            var fieldInfo = GetField(propertyName);
            if (fieldInfo != null)
                return fieldInfo.DbFieldName;
            else
                return propertyName;
        }

        public DbFieldInfo GetPrimaryKeyField()
        {
            return FieldInfoList.Where(f => f.IsPrimaryKey).FirstOrDefault();
        }
    }
}
