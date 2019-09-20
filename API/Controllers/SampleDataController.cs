using System.Linq;
using API.DataAccess;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        [HttpGet("[action]")]
        public WorkItem[] WeatherForecasts(int offset, int limit)
        {
            var db = new DataBase();
            var result = db.Select(offset, limit);
            return result.ToArray<WorkItem>();
        }
    }
}