using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ayx.Dapper.Extensions
{
    public class DbFieldInfo
    {
        public string PropertyName { get; private set; }
        public string DbFieldName { get; private set; }
        public bool IsPrimaryKey { get; private set; }
        public bool IsAutoIncrement { get; private set; }
        public bool NotDbField { get; private set; }
        public IDbFieldType DbFieldType { get; private set; }

        public DbFieldInfo(string propertyName, string dbFieldName = null)
        {
            PropertyName = propertyName;
            if (string.IsNullOrEmpty(dbFieldName))
                DbFieldName = propertyName;
            else
                DbFieldName = dbFieldName;
        }

        public DbFieldInfo SetPrimaryKey()
        {
            IsPrimaryKey = true;
            return this;
        }

        public DbFieldInfo SetAutoIncrement()
        {
            IsAutoIncrement = true;
            return this;
        }

        public DbFieldInfo SetDbFieldType(IDbFieldType fieldType)
        {
            DbFieldType = fieldType;
            return this;
        }

        public DbFieldInfo SetNotDbField()
        {
            NotDbField = true;
            return this;
        }
    }
}
