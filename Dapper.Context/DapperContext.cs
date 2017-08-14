using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Dapper.Context
{
    public class DapperContext : IDapperContext, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public DapperContext()
        {
            _connection = new SqlConnection("connection string");
            _transaction = _connection.BeginTransaction();
        }

        public bool Save<T>(T entity, string sql)
        {
            IsEntityValid(entity);
            IsCommandSQlValid(sql);

            return SaveExec<T>(sql, entity);
        }

        public T GetSave<T>(T entity, string sql)
        {
            IsEntityValid(entity);
            IsCommandSQlValid(sql);

            return Save<T>(sql, entity);
        }

        public bool Save<T>(IEnumerable<T> entity, string sql)
        {
            IsEntityValid(entity);
            IsCommandSQlValid(sql);

            return SaveExec<T>(sql, entity); ;
        }

        private T Save<T>(string sql, object entity) => _connection.Query<T>(sql, entity, _transaction).SingleOrDefault();
        private bool SaveExec<T>(string sql, object entity) => _connection.Execute(sql, entity, _transaction) > 0;

        public T Find<T>(string sql, object where = null)
        {
            IsCommandSQlValid(sql);
            return _connection.Query<T>(sql).SingleOrDefault();
        }

        public IEnumerable<T> FindAll<T>(string sql, object where = null)
        {
            IsCommandSQlValid(sql);
            return _connection.Query<T>(sql).ToList();
        }

        public bool Delete(string sql, object where = null)
        {
            IsCommandSQlValid(sql);
            return _connection.Execute(sql, where, _transaction) > 0;
        }

        public void Commit()
        {
            if (_connection.State == ConnectionState.Open && _transaction != null)
            {
                _transaction.Commit();
                Dispose();
            }
        }

        public void Dispose()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }

        private void IsCommandSQlValid(string sql)
        {
            if (string.IsNullOrEmpty(sql))
                throw new NullReferenceException("The Command Sql can not be null");
        }

        private void IsEntityValid(object entity)
        {
            if (entity == null)
                throw new NullReferenceException("Entity can not be null");
        }
    }
}
