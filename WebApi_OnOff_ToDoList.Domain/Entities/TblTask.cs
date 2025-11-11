using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi_OnOff_ToDoList.Domain.Entities
{
    public class TblTask
    {
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string? description { get; set; }

        public int idStatus { get; set; }
        public TblStatusTask? status { get; set; }

        public int idUser { get; set; }
        public TblUser? user { get; set; }

        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime? updatedAt { get; set; }
    }
}
