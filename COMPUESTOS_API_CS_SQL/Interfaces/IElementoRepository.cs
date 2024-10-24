using COMPUESTOS_API_CS_SQL.Models;

namespace COMPUESTOS_API_CS_SQL.Interfaces
{
    public interface IElementoRepository
    {
        public Task<List<Elemento>> GetAllAsync();
        public Task<Elemento> GetByGuidAsync(Guid elemento_guid);
        public Task<bool> CreateAsync(Elemento unElemento);
        public Task<bool> UpdateAsync(Elemento unElemento);
        public Task<bool> DeleteAsync(Guid elemento_guid);
    }
}
