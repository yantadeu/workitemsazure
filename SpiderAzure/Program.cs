using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SpiderAzure
{
    class Program
    {
        static void Main(string[] args)
        {

            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json");

             var config = builder.Build();
             var hostName = config.GetSection("DB_HOST").Get<string>();
             var dbName = config.GetSection("DB_NAME").Get<string>();
             var uriAzure = config.GetSection("URI_AZURE").Get<string>();
             var personalAccessToken = config.GetSection("PERSONAL_ACCESS_TOKEN").Get<string>();
             var project = config.GetSection("PROJECT").Get<string>();
            Console.WriteLine(dbName);
            Console.WriteLine(hostName);
            Console.WriteLine(uriAzure);
            Console.WriteLine(personalAccessToken);
            Console.WriteLine(project);

            DataBase db = new DataBase(hostName, dbName);
            //db.Connect();

            ExecuteQuery query = new ExecuteQuery(uriAzure, personalAccessToken, project);

            var resultado =  query.RunGetBugsQueryUsingClientLib(db);
            Console.WriteLine(resultado.Result);

        }
    }
}
