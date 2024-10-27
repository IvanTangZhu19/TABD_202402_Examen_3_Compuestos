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
    }
}
