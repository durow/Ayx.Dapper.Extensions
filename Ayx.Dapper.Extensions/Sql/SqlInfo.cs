﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ayx.Dapper.Extensions.Sql
{
    public class SqlInfo
    {
        public Type ModelType { get; set; }
        public string Verb { get; set; }
        public string Param { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string SqlString { get; set; }
    }
}
