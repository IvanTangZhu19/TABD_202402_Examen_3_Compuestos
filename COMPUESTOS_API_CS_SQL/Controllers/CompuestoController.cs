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

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(CompuestoDetallado unCompuestoDetallado)
        {
            try
            {
                var compuestoActualizado = await _compuestoService
                    .UpdateAsync(unCompuestoDetallado);

                return Ok(unCompuestoDetallado);
            }
            catch (AppValidationException error)
            {
                return BadRequest($"Error de validación: {error.Message}");
            }
            catch (DbOperationException error)
            {
                return BadRequest($"Error de operacion en DB: {error.Message}");
            }
        }

        //[HttpDelete]
        //public async Task<IActionResult> RemoveAsync(Guid compuesto_guid)
        //{
        //    try
        //    {
        //        var compuestoEliminado = await _compuestoService
        //            .RemoveAsync(compuesto_guid);

        //        return Ok(compuestoEliminado);
        //    }
        //    catch (AppValidationException error)
        //    {
        //        return BadRequest($"Error de validación: {error.Message}");
        //    }
        //    catch (DbOperationException error)
        //    {
        //        return BadRequest($"Error de operacion en DB: {error.Message}");
        //    }
        //}

    }
}
