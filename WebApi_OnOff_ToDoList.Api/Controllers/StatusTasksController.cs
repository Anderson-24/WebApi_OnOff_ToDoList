using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_OnOff_ToDoList.Infrastructure.Context;
using WebApi_OnOff_ToDoList.Domain.Entities;

namespace WebApi_OnOff_ToDoList.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StatusTasksController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<StatusTasksController> _logger;

        public StatusTasksController(AppDbContext db, ILogger<StatusTasksController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var statuses = await _db.TblStatusTasks
                    .AsNoTracking()
                    .OrderBy(s => s.name)
                    .Select(s => new {
                        id = s.id,
                        name = s.name,
                        description = s.description
                    })
                    .ToListAsync();

                return Ok(statuses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar estados de tarea");
                return StatusCode(500, new { message = "Error interno listando estados" });
            }
        }
    }
}