using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebParfum.API.Data; // Ajusta según tu organización de carpetas

namespace WebParfum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Realiza una consulta simple, por ejemplo, contar cuántos perfumes hay.
            var count = await _context.Perfumes.CountAsync();
            return Ok(new { Message = "Conexión exitosa a la base de datos.", PerfumeCount = count });
        }
    }
}
