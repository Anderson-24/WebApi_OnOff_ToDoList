
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
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<UsersController> _logger;

        public UsersController(AppDbContext db, ILogger<UsersController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _db.TblUsers
                    .AsNoTracking()
                    .Where(u => u.isActive)
                    .OrderBy(u => u.fullName)
                    .Select(u => new {
                        id = u.id,
                        fullName = u.fullName,
                        email = u.email
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar usuarios");
                return StatusCode(500, new { message = "Error interno listando usuarios" });
            }
        }
    }
}
