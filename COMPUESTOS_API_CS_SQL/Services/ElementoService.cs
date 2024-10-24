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
    }
}
