using COMPUESTOS_API_CS_SQL.Services;
using Microsoft.AspNetCore.Mvc;

namespace COMPUESTOS_API_CS_SQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompuestoController(CompuestoService compuestoService): Controller
    {
        private readonly CompuestoService _compuestoService = compuestoService;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var losCompuestos = await _compuestoService
                .GetAllAsync();

            return Ok(losCompuestos);
        }

    }
}
