using Ayx.Dapper.Extensions.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace Ayx.Dapper.Extensions
{
    public static class DapperExtensions
    {
        public static SelectProvider<T> Select<T>(this IDbConnection con, DbInfo dbInfo = null)
        {
            if (dbInfo == null)
                dbInfo = DbInfo.Default;

            return dbInfo.Select<T>(con);
        }
    }
}
