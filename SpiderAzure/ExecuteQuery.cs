using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

namespace SpiderAzure
{
    public class ExecuteQuery
    {
        private readonly string _personalAccessToken;
        private readonly string _project;
        private readonly string _uri;

        public ExecuteQuery(string uri, string personalAccessToken, string project)
        {
            _uri = uri;
            _personalAccessToken = personalAccessToken;
            _project = project;
        }

        
        public async Task<List<WorkItem>> RunGetBugsQueryUsingClientLib(DataBase db)
        {
            var uri = new Uri(_uri);
            var personalAccessToken = _personalAccessToken;
            var project = _project;
            var lastData = db.GetLastUpdate().AddSeconds(1);
            Console.WriteLine(lastData);
            var credentials = new VssBasicCredential("", personalAccessToken);

            var wiql = new Wiql
            {
                Query = "Select [State], [Title] " +
                        "From WorkItems " +
                        "Where [System.TeamProject] = '" + project + "' " +
                        "And [System.CreatedDate] > '" + lastData + "' " +
                        "Order By [State] Asc, [Changed Date] Desc"
            };

            using (var workItemTrackingHttpClient = new WorkItemTrackingHttpClient(uri, credentials))
            {
                var workItemQueryResult = await workItemTrackingHttpClient.QueryByWiqlAsync(wiql, true);

                if (workItemQueryResult.WorkItems.Count() != 0)
                {
                    var list = new List<int>();
                    foreach (var item in workItemQueryResult.WorkItems) list.Add(item.Id);
                    var arr = list.ToArray();

                    var fields = new string[5];
                    fields[0] = "System.Id";
                    fields[1] = "System.Title";
                    fields[2] = "System.WorkItemType";
                    fields[3] = "System.State";
                    fields[4] = "System.CreatedDate";

                    var workItems =
                        await workItemTrackingHttpClient.GetWorkItemsAsync(arr, fields, workItemQueryResult.AsOf);

                    Console.WriteLine("Query Results: {0} items found", workItems.Count);

                    foreach (var workItem in workItems)
                    {
                        Console.WriteLine("{0}          {1}        {2}        {3}   {4}", workItem.Id,
                            workItem.Fields["System.Title"], workItem.Fields["System.WorkItemType"],
                            workItem.Fields["System.State"], workItem.Fields["System.CreatedDate"]);
                        db.Save(workItem.Id, workItem.Fields["System.WorkItemType"], workItem.Fields["System.Title"],
                            workItem.Fields["System.CreatedDate"]);
                    }

                    return workItems;
                }

                return null;
            }
        }
    }
}