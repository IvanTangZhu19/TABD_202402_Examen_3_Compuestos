using COMPUESTOS_API_CS_SQL.Exceptions;
using COMPUESTOS_API_CS_SQL.Models;
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

        [HttpGet("{compuesto_guid:Guid}")]
        public async Task<IActionResult> GetByGuidAsync(Guid compuesto_guid)
        {
            try
            {
                var unCompuesto = await _compuestoService
                    .GetByGuidAsync(compuesto_guid);

                return Ok(unCompuesto);
            }
            catch (AppValidationException error)
            {
                return NotFound(error.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CompuestoDetallado unCompuestoDetallado)
        {
            try
            {
                var compuestoCreado = await _compuestoService
                    .CreateAsync(unCompuestoDetallado);

                return Ok(compuestoCreado);
            }
            catch (AppValidationException error)
            {
                return BadRequest($"Error en la validación: {error.Message}");
            }
            catch (DbOperationException error)
            {
                return BadRequest($"Error en la operación de la DB {error.Message}");
            }
        }

    }
}
