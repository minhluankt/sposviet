using Application.Hepers;
using Application.Interfaces.Repositories;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemVariable;

namespace Infrastructure.Infrastructure.Repositories
{
   
    public class DapperBaseRepository : IDapperRepository
    {
        private readonly IConfiguration _config;
       //private string Connectionstring = EncryptionHelper.Decrypt(SystemVariableHelper.ConnectionString, SystemVariableHelper.publicKey);
        private string Connectionstring = SystemVariableHelper.ConnectionString;

        public DapperBaseRepository(IConfiguration config)
        {
            _config = config;
        }
        public void Dispose()
        {

        }

        public int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            //using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
            using IDbConnection db = new SqlConnection(Connectionstring);
            return db.Query<T>(sp, parms, commandType: commandType).FirstOrDefault();
        }
           public async Task<T> GetAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            //using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
            using IDbConnection db = new SqlConnection(Connectionstring);
            var data = await db.QueryAsync<T>(sp, parms, commandType: commandType);
            return data.FirstOrDefault();
        }

        public List<T> GetAll<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = new SqlConnection(Connectionstring);
            return db.Query<T>(sp, parms, commandType: commandType).ToList();
        }
        public IEnumerable<T> GetAllIEnumerable<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = new SqlConnection(Connectionstring);
            return db.Query<T>(sp, parms, commandType: commandType);
        }

        public DbConnection GetDbconnection()
        {
            return new SqlConnection(Connectionstring);
        }

        public T Insert<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            T result;
            using IDbConnection db = new SqlConnection(Connectionstring);
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = db.Query<T>(sp, parms, commandType: commandType, transaction: tran).FirstOrDefault();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }

            return result;
        }

        public T Update<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            T result;
            using IDbConnection db = new SqlConnection(Connectionstring);
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = db.Query<T>(sp, parms, commandType: commandType, transaction: tran).FirstOrDefault();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }

            return result;
        }
    }
}
