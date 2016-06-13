﻿using System;
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
            return MakeParam(FieldsPart, WherePart);
        }

        protected override string MakeSQL()
        {
            var fields = GetSelectFields(FieldsPart);
            var where = GetWhere(WherePart);
            return $"SELECT {fields} FROM {TableName}{where}";
        }

        public SelectProvider Where(string where)
        {
            WherePart = where;
            return this;
        }

        public SelectProvider Fields(string fields)
        {
            FieldsPart = fields;
            return this;
        }

        public string GetSelectFields(string fields)
        {
            if (fields == null)
                return "*";

            return GetFields(fields);
        }
    }
}
