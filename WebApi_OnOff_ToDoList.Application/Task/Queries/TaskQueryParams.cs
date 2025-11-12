using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi_OnOff_ToDoList.Application.Task.Queries
{
    public class TaskQueryParams
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string? SortField { get; set; }
        public int? SortOrder { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Title { get; set; }
        public bool? GlobalUser { get; set; }
    }
}
