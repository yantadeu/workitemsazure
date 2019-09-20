using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SpiderAzure
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build();
            var hostName = config.GetSection("DB_HOST").Get<string>();
            var dbName = config.GetSection("DB_NAME").Get<string>();
            var user = config.GetSection("DB_USER").Get<string>();
            var password = config.GetSection("DB_PASSWORD").Get<string>();
            
            var uriAzure = config.GetSection("URI_AZURE").Get<string>();
            var personalAccessToken = config.GetSection("PERSONAL_ACCESS_TOKEN").Get<string>();
            var project = config.GetSection("PROJECT").Get<string>();
            
            var db = new DataBase(hostName, dbName, user, password);

            var query = new ExecuteQuery(uriAzure, personalAccessToken, project);

            var resultado = query.RunGetBugsQueryUsingClientLib(db);
            Console.WriteLine(resultado.Result);
        }
    }
}