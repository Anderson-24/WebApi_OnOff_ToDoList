using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi_OnOff_ToDoList.Domain.Entities
{
    public class TblUser
    {
        public int id { get; set; }
        public string fullName { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string passwordHash { get; set; } = string.Empty;
        public DateTime createdAt { get; set; } = DateTime.Now;
        public bool isActive { get; set; } = true;

        public ICollection<TblTask> tasks { get; set; } = new List<TblTask>();
    }
}
