using COMPUESTOS_API_CS_SQL.Exceptions;
using COMPUESTOS_API_CS_SQL.Interfaces;
using COMPUESTOS_API_CS_SQL.Models;

namespace COMPUESTOS_API_CS_SQL.Services
{
    public class ElementoService(IElementoRepository elementoRepository)
    {
        private readonly IElementoRepository _elementoRepository = elementoRepository;

        public async Task<List<Elemento>> GetAllAsync()
        {
            return await _elementoRepository
                .GetAllAsync();
        }
        public async Task<Elemento> GetByGuidAsync(Guid elemento_guid)
        {
            Elemento unElemento = await _elementoRepository
                .GetByGuidAsync(elemento_guid);

            if (unElemento.Uuid == Guid.Empty)
                throw new AppValidationException($"Elemento no encontrado con el guid {elemento_guid}");

            return unElemento;
        }
        public async Task<Elemento> CreateAsync(Elemento unElemento)
        {
            string resultadoValidacionDatos = ValidaDatos(unElemento);
            Elemento elementoExistente;

            if (!string.IsNullOrEmpty(resultadoValidacionDatos))
                throw new AppValidationException(resultadoValidacionDatos);


            var verificacionUnique = await _elementoRepository
                .checkUniqueValuesAsync(unElemento);

            // True es hay valores para los campos: nombre, simbolo y numero atomico
            if (verificacionUnique.Uuid != Guid.Empty)
                throw new AppValidationException("Uno o varios campos ya existen en la base de datos");

            try
            {
                bool resultado = await _elementoRepository
                    .CreateAsync(unElemento);

                if (!resultado)
                    throw new AppValidationException("Operación ejecutada pero no generó cambios");

                elementoExistente = await _elementoRepository
                    .GetByNameAsync(unElemento.Nombre!);
            }
            catch (DbOperationException)
            {
                throw;
            }
            return elementoExistente;
        }

        public async Task<Elemento> UpdateAsync(Elemento unElemento)
        {
            string resultadoValidacionDatos = ValidaDatos(unElemento);

            if (!string.IsNullOrEmpty(resultadoValidacionDatos))
                throw new AppValidationException(resultadoValidacionDatos);

            var elementoExistente = await _elementoRepository
                .checkUniqueValuesAsync(unElemento);

            if (elementoExistente.Uuid != Guid.Empty && elementoExistente.Uuid != unElemento.Uuid)
                throw new AppValidationException($"Ya existe el elemento {unElemento.Nombre}");

            try
            {
                bool resultadoAccion = await _elementoRepository
                    .UpdateAsync(unElemento);

                if (!resultadoAccion)
                    throw new AppValidationException("Operación ejecutada pero no generó cambios en la DB");

                elementoExistente = await _elementoRepository
                    .GetByGuidAsync(unElemento.Uuid);
            }
            catch (DbOperationException)
            {
                throw;
            }

            return elementoExistente;
        }

        public async Task<Elemento> RemoveAsync(Guid elemento_guid)
        {
            var paisExistente = await _elementoRepository
                .GetByGuidAsync(elemento_guid);

            if (paisExistente.Uuid == Guid.Empty)
                throw new AppValidationException($"No existe un elemento identificado con el Guid {elemento_guid} registrado previamente");

            int totalRazasAsociadas = await _elementoRepository
                .GetTotalAssociatedCompoundsByElementGuidAsync(elemento_guid);

            if (totalRazasAsociadas != 0)
                throw new AppValidationException($"Pais {paisExistente.Nombre} tiene asociado {totalRazasAsociadas} razas. No se puede eliminar.");

            try
            {
                bool resultadoAccion = await _elementoRepository
                    .DeleteAsync(elemento_guid);

                if (!resultadoAccion)
                    throw new AppValidationException("Operación ejecutada pero no generó cambios en la DB");
            }
            catch (DbOperationException)
            {
                throw;
            }

            return paisExistente;

        }

        private static string ValidaDatos(Elemento unElemento)
        {
            if (string.IsNullOrEmpty(unElemento.Nombre))
                return ("El nombre del elemento no puede estar vacío");

            if (string.IsNullOrEmpty(unElemento.Simbolo))
                return ("El simbolo no puede estar vacío");

            if(unElemento.Numero_atomico <= 0 || unElemento.Numero_atomico is null)
                return ("El Numero atómico no puede ser menor o igual a cero o estar vacío");

            if (string.IsNullOrEmpty(unElemento.Configuracion))
                return ("La configuracion electronica no puede estar vacío");

            return string.Empty;
        }
    }
}
