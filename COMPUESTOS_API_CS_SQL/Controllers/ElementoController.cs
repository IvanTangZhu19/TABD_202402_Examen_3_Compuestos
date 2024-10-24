using COMPUESTOS_API_CS_SQL.Services;
using Microsoft.AspNetCore.Mvc;

namespace COMPUESTOS_API_CS_SQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElementoController(ElementoService elementoService) : Controller
    {
        private readonly ElementoService _elementoService = elementoService;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var losPaises = await _elementoService
                .GetAllAsync();

            return Ok(losPaises);
        }
    }
}
