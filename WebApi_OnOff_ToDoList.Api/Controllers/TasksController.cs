using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebApi_OnOff_ToDoList.Infrastructure.Context;
using WebApi_OnOff_ToDoList.Domain.Entities;
using WebApi_OnOff_ToDoList.Application.DTOs;
using WebApi_OnOff_ToDoList.Application.Task.Queries;
using WebApi_OnOff_ToDoList.Infrastructure.Moduls.Raw;


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


        [HttpGet("paged")]
        public async Task<ActionResult<PagedResponse<TaskListItemDto>>> GetPaginatedSp([FromQuery] TaskQueryParams p)
        {
            try
            {
                var rows = await _context.Database
                    .SqlQuery<TaskFlatRow>($@"
                EXEC dbo.SP_TASK
                  @TipoOpcion={1},
                  @Page={p.Page}, @Size={p.Size},
                  @SortField={p.SortField}, @SortOrder={p.SortOrder},
                  @UserName={p.UserName}, @Email={p.Email},
                  @Title={p.Title}, @GlobalUser={p.GlobalUser}
            ")
                    .ToListAsync();

                var total = rows.FirstOrDefault()?.TotalCount ?? 0;

                var data = rows.Select(x => new TaskListItemDto
                {
                    Id = x.id,
                    Title = x.title,
                    Description = x.description,
                    CreatedAt = x.createdAt,
                    UpdatedAt = x.updatedAt,
                    User = new MiniUserDto { Id = x.userId, FullName = x.fullName, Email = x.email },
                    Status = new MiniStatusDto { Id = x.statusId, Name = x.statusName }
                }).ToList();

                return Ok(new PagedResponse<TaskListItemDto> { Data = data, Total = total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error SP_TASK TipoOpcion=1 {@Params}", p);
                return StatusCode(500, new { message = "Error interno al obtener tareas (SP)." });
            }
        }

        [HttpGet("metrics")]
        public async Task<ActionResult<TaskMetricsDto>> GetMetrics([FromQuery] TaskQueryParams p)
        {
            try
            {
                await using var conn = _context.Database.GetDbConnection();
                await conn.OpenAsync();

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "dbo.SP_TASK";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                void Add(string name, object? val)
                {
                    var prm = cmd.CreateParameter();
                    prm.ParameterName = name;
                    prm.Value = val ?? (object)DBNull.Value;
                    cmd.Parameters.Add(prm);
                }

                Add("@TipoOpcion", 2);
                Add("@Page", p.Page);
                Add("@Size", p.Size);
                Add("@SortField", p.SortField);
                Add("@SortOrder", p.SortOrder);
                Add("@UserName", p.UserName);
                Add("@Email", p.Email);
                Add("@Title", p.Title);
                Add("@GlobalUser", p.GlobalUser);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var dto = new TaskMetricsDto
                    {
                        Total = reader.GetInt32(reader.GetOrdinal("Total")),
                        Completadas = reader.GetInt32(reader.GetOrdinal("Completadas")),
                        Pendientes = reader.GetInt32(reader.GetOrdinal("Pendientes")),
                        Bloqueadas = reader.GetInt32(reader.GetOrdinal("Bloqueadas")),
                        PorHacer = reader.GetInt32(reader.GetOrdinal("PorHacer")),
                        EnCurso = reader.GetInt32(reader.GetOrdinal("EnCurso")),
                        QA = reader.GetInt32(reader.GetOrdinal("QA"))
                    };
                    return Ok(dto);
                }

                return Ok(new TaskMetricsDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error SP_TASK TipoOpcion=2 (métricas) {@Params}", p);
                return StatusCode(500, new { message = "Error interno al obtener métricas (SP)." });
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
