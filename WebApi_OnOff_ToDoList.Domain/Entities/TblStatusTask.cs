using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi_OnOff_ToDoList.Domain.Entities
{
    public class TblStatusTask
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? description { get; set; }

        public ICollection<TblTask> tasks { get; set; } = new List<TblTask>();
    }
}
