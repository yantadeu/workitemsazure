using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.DataAccess;
using API.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        [HttpGet("[action]")]
        public WorkItem[] WeatherForecasts(int offset, int limit)
        {
            DataBase db = new DataBase();
            List<WorkItem> result = db.Select(offset, limit);
            return result.ToArray<WorkItem>();
        }
    }
}
