using System;
using System.Collections.Generic;

namespace Dapper.Context
{
    public interface IDapperContext
    {
        bool Save<T>(T entity, string sql);
        bool Save<T>(IEnumerable<T> entity, string sql);
        T GetSave<T>(T entity, string sql);
        T Find<T>(string sql, object where = null);
        IEnumerable<T> FindAll<T>(string sql, object where = null);
        void Join<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object where = null, string indexs = "Id");
        void Join<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object where = null, string indexs = "Id");
        bool Delete(string sql, object where = null);
        T ExecFunction<T>(string function);
        void Commit();
    }
}
