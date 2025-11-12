using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi_OnOff_ToDoList.Application.DTOs
{
    public class TaskMetricsDto
    {
        public int Total { get; set; }
        public int Completadas { get; set; }
        public int Pendientes { get; set; }
        public int Bloqueadas { get; set; }
        public int PorHacer { get; set; }
        public int EnCurso { get; set; }
        public int QA { get; set; }
    }
}
