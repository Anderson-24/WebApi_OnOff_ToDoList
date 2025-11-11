using Microsoft.AspNetCore.Mvc;
using WebApi_OnOff_ToDoList.Infrastructure.Context;

namespace WebApi_OnOff_ToDoList.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectionTestController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public ConnectionTestController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("check")]
        public IActionResult CheckConnection()
        {
            try
            {
                bool canConnect = _context.Database.CanConnect();
                string? connectionString = _configuration.GetConnectionString("ToDoConnectionString");

                return Ok(new { Connected = canConnect, ConnectionString = connectionString });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
