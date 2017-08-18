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
        private IDbTransaction _transaction;

        public DapperContext()
        {
            _connection = new SqlConnection("connection string");            
        }

        public bool Save<T>(T entity, string sql)
        {
            IsEntityValid(entity);
            IsCommandSQlValid(sql);
            Open();

            return SaveExec<T>(sql, entity);
        }

        public T GetSave<T>(T entity, string sql)
        {
            IsEntityValid(entity);
            IsCommandSQlValid(sql);
            Open();

            return Save<T>(sql, entity);
        }

        public bool Save<T>(IEnumerable<T> entity, string sql)
        {
            IsEntityValid(entity);
            IsCommandSQlValid(sql);
            Open();

            return SaveExec<T>(sql, entity); ;
        }

        private T Save<T>(string sql, object entity) => _connection.Query<T>(sql, entity, _transaction).SingleOrDefault();
        private bool SaveExec<T>(string sql, object entity) => _connection.Execute(sql, entity, _transaction) > 0;

        public T Find<T>(string sql, object where = null)
        {
            IsCommandSQlValid(sql);
            Open();

            return _connection.Query<T>(sql, where, _transaction).SingleOrDefault();
        }

        public IEnumerable<T> FindAll<T>(string sql, object where = null)
        {
            IsCommandSQlValid(sql);
            Open();

            return _connection.Query<T>(sql, where, _transaction).ToList();
        }

        public void FindJoin<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object where = null, string indexs = "Id")
        {
            IsCommandSQlValid(sql);
            Open();

            _connection.Query<TFirst, TSecond, TReturn>(sql, map, param: where, transaction: _transaction, splitOn: indexs).ToList();
        }

        public bool Delete(string sql, object where = null)
        {
            IsCommandSQlValid(sql);
            Open();

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
                _connection.Close();

            _connection.Dispose();
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

        private void Open()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
                _transaction = _connection.BeginTransaction();
            }
        }
    }
}
