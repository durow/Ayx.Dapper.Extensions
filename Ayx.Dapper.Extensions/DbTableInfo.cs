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

        public DbTableInfo(Type modelType, string tableName, string token = null)
        {
            ModelType = modelType;
            TableName = tableName;
            FieldInfoList = new List<DbFieldInfo>();
            Token = token;
        }

        public void SetFields(params DbFieldInfo[] fields)
        {
            FieldInfoList.AddRange(fields);
        }

        public DbFieldInfo GetField(string propertyName)
        {
            return FieldInfoList
                .Where(f => f.PropertyName == propertyName)
                .FirstOrDefault();
        }
    }
}
