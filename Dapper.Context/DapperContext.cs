using Dapper.Context.Configuration;
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

        private bool isCommit = false;

        public DapperContext(BaseConnection baseConnection) => _connection = new SqlConnection(baseConnection.GetConnectionString());        

        public bool Save<T>(T entity, string sql)
        {
            try
            {
                IsEntityValid(entity);
                IsCommandSQlValid(sql);
                Open();

                return SaveExec<T>(sql, entity);
            }
            catch (Exception ex)
            {
                Dispose();
            }

            return false;
        }

        public T GetSave<T>(T entity, string sql)
        {
            try
            {
                IsEntityValid(entity);
                IsCommandSQlValid(sql);
                Open();

                return Save<T>(sql, entity);
            }
            catch (Exception ex)
            {
                Dispose();
            }

            return default(T);
        }

        public bool Save<T>(IEnumerable<T> entity, string sql)
        {
            try
            {
                IsEntityValid(entity);
                IsCommandSQlValid(sql);
                Open();

                return SaveExec<T>(sql, entity);
            }
            catch (Exception ex)
            {
                Dispose();
            }

            return false;
        }

        private T Save<T>(string sql, object entity) => _connection.Query<T>(sql, entity, _transaction).SingleOrDefault();
        private bool SaveExec<T>(string sql, object entity) => _connection.Execute(sql, entity, _transaction) > 0;

        public T Find<T>(string sql, object where = null)
        {
            try
            {
                IsCommandSQlValid(sql);
                Open();

                return _connection.Query<T>(sql, where, _transaction).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Dispose();
            }
            finally
            {
                Dispose();
            }

            return default(T);
        }

        public IEnumerable<T> FindAll<T>(string sql, object where = null)
        {
            try
            {
                IsCommandSQlValid(sql);
                Open();

                return _connection.Query<T>(sql, where, _transaction).ToList();
            }
            catch (Exception ex)
            {
                Dispose();
            }
            finally
            {
                Dispose();
            }

            return null;
        }

        public void Join<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object where = null, string indexs = "Id")
        {
            try
            {
                IsCommandSQlValid(sql);
                Open();

                _connection.Query<TFirst, TSecond, TReturn>(sql, map, param: where, transaction: _transaction, splitOn: indexs).ToList();
            }
            catch (Exception ex)
            {
                Dispose();
            }
            finally
            {
                Dispose();
            }
        }

        public void Join<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object where = null, string indexs = "Id")
        {
            try
            {
                IsCommandSQlValid(sql);
                Open();

                _connection.Query(sql, map, param: where, transaction: _transaction, splitOn: indexs).ToList();
            }
            catch (Exception ex)
            {
                Dispose();
            }
            finally
            {
                Dispose();
            }
        }

        public bool Delete(string sql, object where = null)
        {
            try
            {
                IsCommandSQlValid(sql);
                Open();

                return _connection.Execute(sql, where, _transaction) > 0;
            }
            catch (Exception ex)
            {
                Dispose();
            }
            finally
            {
                Dispose();
            }

            return false;
        }

        public void Commit()
        {
            if (_connection.State == ConnectionState.Open && _transaction != null)
            {
                _transaction.Commit();
                isCommit = true;
                Dispose();
            }
        }

        public void Dispose()
        {
            if (_connection.State == ConnectionState.Open)
            {
                if (_transaction != null && !isCommit)
                    _transaction.Rollback();

                _connection.Close();
            }

            _transaction.Dispose();
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

        public T ExecFunction<T>(string function)
        {
            try
            {
                IsCommandSQlValid(function);
                Open();

                return _connection.Query<T>(function, transaction: _transaction).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Dispose();
            }
            finally
            {
                Dispose();
            }

            return default(T);
        }
    }
}
