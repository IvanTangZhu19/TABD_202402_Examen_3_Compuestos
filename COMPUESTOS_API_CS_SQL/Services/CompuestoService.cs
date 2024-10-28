using COMPUESTOS_API_CS_SQL.Exceptions;
using COMPUESTOS_API_CS_SQL.Interfaces;
using COMPUESTOS_API_CS_SQL.Models;
using COMPUESTOS_API_CS_SQL.Repositories;

namespace COMPUESTOS_API_CS_SQL.Services
{
    public class CompuestoService(ICompuestoRepository compuestoRepository)
    {
        private readonly ICompuestoRepository _compuestoRepository = compuestoRepository;

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
    }
}
