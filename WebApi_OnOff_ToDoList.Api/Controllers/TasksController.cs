using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebApi_OnOff_ToDoList.Infrastructure.Context;
using WebApi_OnOff_ToDoList.Domain.Entities;

namespace WebApi_OnOff_ToDoList.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TasksController> _logger;

        public TasksController(AppDbContext context, ILogger<TasksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idStatus"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? idStatus = null, [FromQuery] int? idUser = null)
        {
            try
            {
                IQueryable<TblTask> query = _context.TblTasks
                    .Include(t => t.status)
                    .Include(t => t.user)
                    .AsNoTracking();

                if (idStatus.HasValue)
                    query = query.Where(t => t.idStatus == idStatus);

                if (idUser.HasValue)
                    query = query.Where(t => t.idUser == idUser);

                var tasks = await query
                .Select(t => new
                {
                    t.id,
                    t.title,
                    t.description,
                    t.createdAt,
                    t.updatedAt,
                    userFullName = t.user!.fullName,
                    userEmail = t.user!.email,
                    statusName = t.status!.name
                })
                .OrderByDescending(t => t.createdAt)
                .ToListAsync();

                _logger.LogInformation("Se obtuvieron {Count} tareas (status: {Status}, user: {User})", tasks.Count, idStatus, idUser);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tareas");
                return StatusCode(500, new { message = "Error interno al obtener tareas", error = ex.Message });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var task = await _context.TblTasks
                    .Include(t => t.status)
                    .Include(t => t.user)
                    .FirstOrDefaultAsync(t => t.id == id);

                if (task == null)
                {
                    _logger.LogWarning("Intento de consulta para tarea inexistente (ID={Id})", id);
                    return NotFound(new { message = "Tarea no encontrada" });
                }

                _logger.LogInformation("Tarea {Id} obtenida correctamente", id);
                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la tarea con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno al consultar la tarea", error = ex.Message });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TblTask model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.title))
                    return BadRequest(new { message = "El título es obligatorio." });

                if (!await _context.TblStatusTasks.AnyAsync(s => s.id == model.idStatus))
                    return BadRequest(new { message = "El estado especificado no existe." });

                if (!await _context.TblUsers.AnyAsync(u => u.id == model.idUser))
                    return BadRequest(new { message = "El usuario especificado no existe." });

                model.createdAt = DateTime.Now;
                _context.TblTasks.Add(model);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Tarea creada correctamente con ID {Id}", model.id);
                return CreatedAtAction(nameof(GetById), new { id = model.id }, model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una nueva tarea");
                return StatusCode(500, new { message = "Error interno al crear la tarea", error = ex.Message });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TblTask updated)
        {
            try
            {
                var existing = await _context.TblTasks.FindAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Intento de actualización de tarea inexistente (ID={Id})", id);
                    return NotFound(new { message = "Tarea no encontrada." });
                }

                if (string.IsNullOrWhiteSpace(updated.title))
                    return BadRequest(new { message = "El título es obligatorio." });

                if (!await _context.TblStatusTasks.AnyAsync(s => s.id == updated.idStatus))
                    return BadRequest(new { message = "El estado especificado no existe." });

                if (!await _context.TblUsers.AnyAsync(u => u.id == updated.idUser))
                    return BadRequest(new { message = "El usuario especificado no existe." });

                existing.title = updated.title;
                existing.description = updated.description;
                existing.idStatus = updated.idStatus;
                existing.idUser = updated.idUser;
                existing.updatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Tarea {Id} actualizada correctamente", id);
                return Ok(existing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar tarea con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno al actualizar la tarea", error = ex.Message });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var task = await _context.TblTasks.FindAsync(id);
                if (task == null)
                {
                    _logger.LogWarning("Intento de eliminación de tarea inexistente (ID={Id})", id);
                    return NotFound(new { message = "Tarea no encontrada." });
                }

                _context.TblTasks.Remove(task);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Tarea {Id} eliminada correctamente", id);
                return Ok(new { message = "Tarea eliminada correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar tarea con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar la tarea", error = ex.Message });
            }
        }
    }
}
