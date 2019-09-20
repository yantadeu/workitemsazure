using System;

namespace API.Models
{
    public class WorkItem
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public DateTime Created_at { get; set; }
    }
}
