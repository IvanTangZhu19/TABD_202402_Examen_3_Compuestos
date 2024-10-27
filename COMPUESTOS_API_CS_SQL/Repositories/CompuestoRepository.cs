using COMPUESTOS_API_CS_SQL.Interfaces;
using COMPUESTOS_API_CS_SQL.Models;

namespace COMPUESTOS_API_CS_SQL.Repositories
{
    public class CompuestoRepository : ICompuestoRepository
    {
        public Task<List<Compuesto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Compuesto> GetByGuidAsync(Guid compuesto_guid)
        {
            throw new NotImplementedException();
        }

        public Task<Compuesto> GetByNameAsync(string compuesto_nombre)
        {
            throw new NotImplementedException();
        }
        public Task<bool> CreateAsync(Compuesto unaRaza)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid elemento_guid)
        {
            throw new NotImplementedException();
        }


        public Task<bool> UpdateAsync(Compuesto unElemento)
        {
            throw new NotImplementedException();
        }
    }
}
