using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;

namespace SpiderAzure
{
    public class ExecuteQuery
    {
        readonly string _uri;
        readonly string _personalAccessToken;
        readonly string _project;

        /// <summary>
        /// Constructor. Manually set values to match yourorganization. 
        /// </summary>
        public ExecuteQuery(string uri, string personalAccessToken, string project)
        {
            _uri = uri;
            _personalAccessToken = personalAccessToken;
            _project = project;
        }

        /// <summary>
        /// Execute a WIQL query to return a list of bugs using the .NET client library
        /// </summary>
        /// <returns>List of Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem</returns>
        public async Task<List<WorkItem>> RunGetBugsQueryUsingClientLib(DataBase db)
        {
            Uri uri = new Uri(_uri);
            string personalAccessToken = _personalAccessToken;
            string project = _project;
            DateTime lastData = db.GetLastUpdate().AddSeconds(1);
            Console.WriteLine(lastData);
            VssBasicCredential credentials = new VssBasicCredential("", personalAccessToken);

            //create a wiql object and build our query
            Wiql wiql = new Wiql()
            {
                Query = "Select [State], [Title] " +
                        "From WorkItems " +
                        //"Where [Work Item Type] = 'Task' " +
                        "Where [System.TeamProject] = '" + project + "' " +
                        //"And [System.State] <> 'Closed' " +
                        "And [System.CreatedDate] > '"+lastData+"' "+
                        "Order By [State] Asc, [Changed Date] Desc"
            };

            //create instance of work item tracking http client
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(uri, credentials))
            {
                //execute the query to get the list of work items in the results
                WorkItemQueryResult workItemQueryResult = await workItemTrackingHttpClient.QueryByWiqlAsync(wiql, true);

                //some error handling                
                if (workItemQueryResult.WorkItems.Count() != 0)
                {
                    //need to get the list of our work item ids and put them into an array
                    List<int> list = new List<int>();
                    foreach (var item in workItemQueryResult.WorkItems)
                    {
                        list.Add(item.Id);
                    }
                    int[] arr = list.ToArray();

                    //build a list of the fields we want to see
                    string[] fields = new string[5];
                    fields[0] = "System.Id";
                    fields[1] = "System.Title";
                    fields[2] = "System.WorkItemType";
                    fields[3] = "System.State";
                    fields[4] = "System.CreatedDate";

                    //get work items for the ids found in query
                    var workItems = await workItemTrackingHttpClient.GetWorkItemsAsync(arr, fields, workItemQueryResult.AsOf);

                    Console.WriteLine("Query Results: {0} items found", workItems.Count);

                    //loop though work items and write to console
                    foreach (var workItem in workItems)
                    {
                        Console.WriteLine("{0}          {1}        {2}        {3}   {4}", workItem.Id, workItem.Fields["System.Title"], workItem.Fields["System.WorkItemType"], workItem.Fields["System.State"], workItem.Fields["System.CreatedDate"]);
                        db.Save(workItem.Id, workItem.Fields["System.WorkItemType"], workItem.Fields["System.Title"], workItem.Fields["System.CreatedDate"]);
                    }

                    return workItems;
                }

                return null;
            }
        }
    }
}