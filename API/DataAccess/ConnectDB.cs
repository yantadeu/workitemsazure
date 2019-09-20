using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using API.Models;

namespace API.DataAccess
{
    public class DataBase
    {
        private readonly string _databaseName;
        private readonly string _serverName;
        private readonly string _userName;
        private readonly string _password;

        public DataBase()
        {
            _serverName = Environment.GetEnvironmentVariable("DB_SERVER"); //"localhost";
            _databaseName = Environment.GetEnvironmentVariable("DB_NAME"); //"AzureIntegration";
            _userName = Environment.GetEnvironmentVariable("DB_USER"); //"sa";
            _password = Environment.GetEnvironmentVariable("DB_PASSWORD"); //"Password1";
        }

        public SqlConnection Connect()
        {
            var connection = new SqlConnection();
            connection.ConnectionString =
                "Server=" + _serverName + 
                "; Database =" + _databaseName + 
                "; User ID =" + _userName + 
                "; Password =" + _password + 
                ";Trusted_Connection=true";
            connection.Open();
            Console.WriteLine("Banco conectado");
            return connection;
        }

        public List<WorkItem> Select(int offset, int limit)
        {
            var connect = Connect();
            var sql = "SELECT * FROM [Table] ORDER BY ID OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY";
            try
            {
                var comando = new SqlCommand(sql, connect);
                //Adicionando o valor das textBox nos parametros do comando
                comando.Parameters.Add(new SqlParameter("@offset", offset));
                comando.Parameters.Add(new SqlParameter("@limit", limit));
                //executa o comando com os parametros que foram adicionados acima
                var reader = comando.ExecuteReader();
                var result = new List<WorkItem>();
                while (reader.Read())
                {
                    result.Add(
                        new WorkItem
                        {
                            Id = (int) reader["Id"],
                            Type = (string) reader["type"],
                            Title = (string) reader["title"],
                            Created_at = (DateTime) reader["create_at"]
                        }
                    );
                    ;
                }

                //fecha a conexao
                connect.Close();
                return result;
            }
            catch (Exception dbex)
            {
                Console.WriteLine("Erro ao buscar!" + dbex);
                return null;
            }
            finally
            {
                connect.Close();
            }
        }
    }
}