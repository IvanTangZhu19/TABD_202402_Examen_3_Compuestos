using COMPUESTOS_API_CS_SQL.Models;

namespace COMPUESTOS_API_CS_SQL.Interfaces
{
    public interface ICompuestoRepository
    {
        public Task<List<Compuesto>> GetAllAsync();
        public Task<Compuesto> GetByGuidAsync(Guid compuesto_guid);
        public Task<Compuesto> GetByNameAsync(string compuesto_nombre);
        public Task<bool> CreateAsync(Compuesto unCompuesto);
        public Task<bool> UpdateAsync(Compuesto unCompuesto);
        public Task<bool> DeleteAsync(Guid compuesto_guid);

        public Task<CompuestoDetallado> GetDetailedCompoundByGuidAsync(Guid compuesto_guid);
    }
}
