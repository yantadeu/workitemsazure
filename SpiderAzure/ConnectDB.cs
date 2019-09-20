using System;
using System.Data.SqlClient;

namespace SpiderAzure
{
    public class DataBase
    {
        private readonly string _databaseName;
        private readonly string _serverName;
        private readonly string _userName;
        private readonly string _password;

        public DataBase(string host, string db, string user, string password)
        {
            _serverName = host;
            _databaseName = db;
            _userName = user;
            _password = password;
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

        public void Save(int? id, object type, object title, object createdAt)
        {
            var connect = Connect();
            var sql = "INSERT INTO [Table] VALUES(@Id, @type, @title, @created_at)";
            try
            {
                var comando = new SqlCommand(sql, connect);
                //Adicionando o valor das textBox nos parametros do comando
                comando.Parameters.Add(new SqlParameter("@Id", id.ToString()));
                comando.Parameters.Add(new SqlParameter("@type", type.ToString()));
                comando.Parameters.Add(new SqlParameter("@title", title.ToString()));
                comando.Parameters.Add(new SqlParameter("@created_at", createdAt.ToString()));
                //abre a conexao
                //conn.Open();
                //executa o comando com os parametros que foram adicionados acima
                comando.ExecuteNonQuery();
                //fecha a conexao
                connect.Close();
                Console.WriteLine("Workitem salvo!");
            }
            catch (Exception dbex)
            {
                Console.WriteLine("Erro ao salvar!" + dbex);
            }
            finally
            {
                connect.Close();
            }
        }

        public DateTime GetLastUpdate()
        {
            var connect = Connect();
            var sql = "SELECT TOP 1 create_at FROM [Table] ORDER BY create_at DESC";
            try
            {
                var comando = new SqlCommand(sql, connect);
                var reader = comando.ExecuteReader();
                var lastData = DateTime.Parse("1/1/1753 12:00:00 AM");
                while (reader.Read()) lastData = (DateTime) reader["create_at"];
                //fecha a conexao
                connect.Close();
                return lastData;
            }
            catch (Exception dbex)
            {
                Console.WriteLine("Erro ao buscar!" + dbex);
                return new DateTime();
            }
            finally
            {
                connect.Close();
            }
        }
    }
}