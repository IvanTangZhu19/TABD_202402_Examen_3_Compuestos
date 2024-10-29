using COMPUESTOS_API_CS_SQL.Exceptions;
using COMPUESTOS_API_CS_SQL.Interfaces;
using COMPUESTOS_API_CS_SQL.Models;

namespace COMPUESTOS_API_CS_SQL.Services
{
    public class CompuestoService(ICompuestoRepository compuestoRepository,
                                    IElementoRepository elementoRepository)
    {
        private readonly ICompuestoRepository _compuestoRepository = compuestoRepository;
        private readonly IElementoRepository _elementoRepository = elementoRepository;

        public async Task<List<Compuesto>> GetAllAsync()
        {
            return await _compuestoRepository
                .GetAllAsync();
        }

        public async Task<CompuestoDetallado> GetByGuidAsync(Guid compuesto_guid)
        {
            Compuesto unCompuesto = await _compuestoRepository
                .GetByGuidAsync(compuesto_guid);

            if (unCompuesto.Uuid == Guid.Empty)
                throw new AppValidationException($"Compuesto no encontrada con el guid {compuesto_guid}");

            CompuestoDetallado unCompuestoDetallado = await _compuestoRepository
                .GetDetailedCompoundByGuidAsync(compuesto_guid);

            return unCompuestoDetallado;
        }

        public async Task<Compuesto> CreateAsync(CompuestoDetallado unCompuestoDetallado)
        {
            string resultadoValidacionDatos = ValidaDatos(unCompuestoDetallado);

            if (!string.IsNullOrEmpty(resultadoValidacionDatos))
                throw new AppValidationException(resultadoValidacionDatos);

            var compuestoExistente = await _compuestoRepository
                .GetByNameAsync(unCompuestoDetallado.Nombre!);

            if (compuestoExistente.Uuid != Guid.Empty)
                throw new AppValidationException($"Ya existe el compuesto {unCompuestoDetallado.Nombre}");

            Elemento elementoExistente;

            //Se valida cada elemento si está o no registrado en la tabla Elementos
            foreach (var elemento in unCompuestoDetallado.Elementos!)
            {
                elementoExistente = await _elementoRepository.GetByNameAsync(elemento.Nombre!);
                if(elementoExistente.Uuid == Guid.Empty)
                    throw new AppValidationException($"El elemento {elemento.Nombre} no se encuentra registrado");
            }
            try
            {
                bool resultado = await _compuestoRepository
                    .CreateAsync(unCompuestoDetallado);

                if (!resultado)
                    throw new AppValidationException("Operación ejecutada pero no generó cambios");

                compuestoExistente = await _compuestoRepository
                    .GetByNameAsync(unCompuestoDetallado.Nombre!);
            }
            catch (DbOperationException)
            {
                throw;
            }
            return compuestoExistente;
        }

        private static string ValidaDatos(CompuestoDetallado unCompuestoDetallado)
        {
            if (string.IsNullOrEmpty(unCompuestoDetallado.Nombre))
                return ("El nombre del compuesto no puede estar vacío");

            if (string.IsNullOrEmpty(unCompuestoDetallado.Formula))
                return ("La formula del compuesto no puede estar vacío");

            if (string.IsNullOrEmpty(unCompuestoDetallado.Estado_agregacion))
                return ("El estado de agregación no puede estar vacío");
            if (unCompuestoDetallado.Masa_molar < 0)
                return ("La masa molar no puede ser menor a cero");

            return string.Empty;
        }
    }
}
