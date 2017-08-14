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
        bool Delete(string sql, object where = null);
        void Commit();
    }
}
