using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ayx.Dapper.Extensions
{
    public static class DbAttributes
    {
        public static DbFieldAttribute GetDbFieldInfo(PropertyInfo property)
        {
            return property.GetCustomAttributes(typeof(DbFieldAttribute), false)
                .FirstOrDefault() as DbFieldAttribute;
        }

        public static string GetTableName(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(DbTableAttribute), false)
                .FirstOrDefault() as DbTableAttribute;
            return attr?.TableName??null;
        }

        public static string GetDbFieldName(PropertyInfo property)
        {
            var fieldAttr = GetDbFieldInfo(property);
            if (fieldAttr != null)
            {
                if (string.IsNullOrEmpty(fieldAttr.FieldName))
                    return property.Name;
                else
                    return fieldAttr.FieldName;
            }
            return property.Name;
        }

        public static bool CheckDbField(PropertyInfo property)
        {
            var attr = property.GetCustomAttributes(typeof(NotDbFieldAttribute), false)
                .FirstOrDefault() as NotDbFieldAttribute;
            return (attr == null) ? true : false;
        }

        public static bool CheckAutoIncrement(PropertyInfo property)
        {
            var attr = property.GetCustomAttributes(typeof(AutoIncrementAttribute), false)
                .FirstOrDefault() as AutoIncrementAttribute;
            return (attr == null) ? false : true;
        }

        public static bool CheckPrimaryKey(PropertyInfo property)
        {
            var attr = property.GetCustomAttributes(typeof(PrimaryKeyAttribute), false)
                .FirstOrDefault() as PrimaryKeyAttribute;
            return (attr == null) ? false : true;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DbFieldAttribute : Attribute
    {
        public string FieldName { get; set; }
        public DbFieldType FieldType { get; set; }

        public DbFieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DbTableAttribute : Attribute
    {
        public string TableName { get; private set; }
        public DbTableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PrimaryKeyAttribute:Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AutoIncrementAttribute:Attribute
    { }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NotDbFieldAttribute:Attribute
    { }
}
