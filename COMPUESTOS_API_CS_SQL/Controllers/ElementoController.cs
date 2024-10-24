using COMPUESTOS_API_CS_SQL.Exceptions;
using COMPUESTOS_API_CS_SQL.Models;
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
            var losElementos = await _elementoService
                .GetAllAsync();

            return Ok(losElementos);
        }

        [HttpGet("{elemento_guid:Guid}")]
        public async Task<IActionResult> GetByGuidAsync(Guid elemento_guid)
        {
            try
            {
                var unElemento = await _elementoService
                    .GetByGuidAsync(elemento_guid);

                return Ok(unElemento);
            }
            catch (AppValidationException error)
            {
                return NotFound(error.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Elemento unElemento)
        {
            try
            {
                var elementoCreado = await _elementoService
                    .CreateAsync(unElemento);

                return Ok(elementoCreado);
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
        //[HttpPut]
        //public async Task<IActionResult> UpdateAsync(Pais unPais)
        //{
        //    try
        //    {
        //        var paisActualizado = await _paisService
        //            .UpdateAsync(unPais);

        //        return Ok(unPais);
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

        //[HttpDelete]
        //public async Task<IActionResult> RemoveAsync(Guid pais_guid)
        //{
        //    try
        //    {
        //        var paisEliminado = await _paisService
        //            .RemoveAsync(pais_guid);

        //        return Ok(paisEliminado);
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
