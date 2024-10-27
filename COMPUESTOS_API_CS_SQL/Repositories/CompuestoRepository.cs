using COMPUESTOS_API_CS_SQL.Contexts;
using COMPUESTOS_API_CS_SQL.Interfaces;
using COMPUESTOS_API_CS_SQL.Models;
using Dapper;

namespace COMPUESTOS_API_CS_SQL.Repositories
{
    public class CompuestoRepository(PgsqlContext unContexto) : ICompuestoRepository
    {
        private readonly PgsqlContext contextoDB = unContexto;
        public async Task<List<Compuesto>> GetAllAsync()
        {
            var conexion = contextoDB.CreateConnection();

            string sentenciaSQL =
                "SELECT compuesto_uuid uuid, nombre, formula, masa_molar, estado_agregacion " +
                "FROM core.compuestos ORDER BY nombre";

            var resultadoCompuestos = await conexion
                .QueryAsync<Compuesto>(sentenciaSQL, new DynamicParameters());

            return resultadoCompuestos.ToList();
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
