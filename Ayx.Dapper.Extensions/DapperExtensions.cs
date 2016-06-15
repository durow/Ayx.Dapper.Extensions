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
        #region Select

        public static SelectProvider<T> Select<T>(this IDbConnection connection, DbInfo dbInfo = null)
        {
            if (dbInfo == null)
                dbInfo = DbInfo.Default;

            return dbInfo.Select<T>(connection);
        }

        #endregion  

        #region Delete

        public static DeleteProvider<T> Delete<T>(this IDbConnection connection, DbInfo dbInfo = null)
        {
            if (dbInfo == null)
                dbInfo = DbInfo.Default;

            return dbInfo.Delete<T>(connection);
        }

        public static int Delete<T>(this IDbConnection connection,
            object param, 
            IDbTransaction transaction = null,
            int? timeOut = null,
            CommandType? commandType = null)
        {
            return DbInfo.Default.Delete<T>(connection).Go(param, transaction, timeOut, commandType);
        }

        public static int Delete<T>(this IDbConnection connection,
            List<T> itemList,
            IDbTransaction transaction = null,
            int? timeOut = null,
            CommandType? commandType = null)
        {
            var result = 0;
            var trans = connection.BeginTransaction();
            try
            {
                foreach (var item in itemList)
                {
                    result += connection.Delete<T>(item,trans);
                }
                trans.Commit();
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            return result;
        }

        #endregion

        #region Update

        public static UpdateProvider<T> Update<T>(this IDbConnection connection, DbInfo dbInfo = null)
        {
            if (dbInfo == null)
                dbInfo = DbInfo.Default;

            return dbInfo.Update<T>(connection);
        }

        public static int Update<T>(this IDbConnection connection,
            object param,
            IDbTransaction transaction = null,
            int? timeOut = null,
            CommandType? commandType = null)
        {
            return DbInfo.Default.Update<T>(connection).Go(param, transaction, timeOut, commandType);
        }

        public static int Update<T>(this IDbConnection connection,
            List<T> itemList,
            IDbTransaction transaction = null,
            int? timeOut = null,
            CommandType? commandType = null)
        {
            var result = 0;
            var trans = connection.BeginTransaction();
            try
            {
                foreach (var item in itemList)
                {
                    result += connection.Update<T>(item, trans);
                }
                trans.Commit();
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            return result;
        }

        #endregion

        #region Insert

        public static InsertProvider<T>  Insert<T> (this IDbConnection connection, DbInfo dbInfo = null)
        {
            if (dbInfo == null)
                dbInfo = DbInfo.Default;

            return dbInfo.Insert<T>(connection);
        }

        public static int Insert<T>(this IDbConnection connection,
            object param,
            IDbTransaction transaction = null,
            int? timeOut = null,
            CommandType? commandType = null)
        {
            return DbInfo.Default.Insert<T>(connection).Go(param, transaction, timeOut, commandType);
        }

        public static int InsertIdentity<T>(this IDbConnection connection,
            object param,
            IDbTransaction transaction = null,
            int? timeOut = null,
            CommandType? commandType = null)
        {
            return DbInfo
                .Default
                .Insert<T>(connection)
                .ReturnIdentity()
                .Go(param, transaction, timeOut, commandType);
        }

        public static int Insert<T>(this IDbConnection connection,
            List<T> itemList,
            IDbTransaction transaction = null,
            int? timeOut = null,
            CommandType? commandType = null)
        {
            var result = 0;
            var trans = connection.BeginTransaction();
            try
            {
                foreach (var item in itemList)
                {
                    result += connection.Insert<T>(item, trans);
                }
                trans.Commit();
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            return result;
        }

        #endregion
    }
}
