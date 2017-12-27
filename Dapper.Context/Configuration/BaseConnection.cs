using System;
using System.Configuration;

namespace Dapper.Context.Configuration
{
    public class BaseConnection
    {
        public BaseConnection(string nameConnectionString)
        {
            NameConnectionString = nameConnectionString;
        }

        public string ConnectionString { private get; set; }
        public string NameConnectionString { private get; set; }

        public string GetConnectionString()
        {
            if (!string.IsNullOrEmpty(ConnectionString) && !string.IsNullOrEmpty(NameConnectionString))
                throw new Exception("Informe somente uma propriedade para conexão. ConnectionString ou NameConnectionString");


            return string.IsNullOrEmpty(ConnectionString) ? ConfigurationManager.ConnectionStrings[NameConnectionString].ConnectionString
                : ConnectionString;
        }
    }
}
