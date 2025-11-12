using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi_OnOff_ToDoList.Application.DTOs
{
    public class TaskListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public MiniUserDto User { get; set; } = default!;
        public MiniStatusDto Status { get; set; } = default!;
    }

    public class MiniUserDto { public int Id { get; set; } public string FullName { get; set; } = default!; public string Email { get; set; } = default!; }
    public class MiniStatusDto { public int Id { get; set; } public string Name { get; set; } = default!; }

    public class PagedResponse<T> { public IReadOnlyList<T> Data { get; set; } = Array.Empty<T>(); public int Total { get; set; } }
}
