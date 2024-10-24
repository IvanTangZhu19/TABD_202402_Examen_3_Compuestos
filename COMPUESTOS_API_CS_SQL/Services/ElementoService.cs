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
    }
}
