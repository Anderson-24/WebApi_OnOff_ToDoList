using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi_OnOff_ToDoList.Infrastructure.Moduls.Raw
{
    public class TaskFlatRow
    {
        public int id { get; set; }
        public string title { get; set; } = default!;
        public string description { get; set; } = default!;
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public int userId { get; set; }
        public string fullName { get; set; } = default!;
        public string email { get; set; } = default!;
        public int statusId { get; set; }
        public string statusName { get; set; } = default!;
        public int TotalCount { get; set; }
    }
}
