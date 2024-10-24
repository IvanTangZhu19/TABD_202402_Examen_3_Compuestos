using COMPUESTOS_API_CS_SQL.Contexts;
using COMPUESTOS_API_CS_SQL.Interfaces;
using COMPUESTOS_API_CS_SQL.Models;
using Dapper;

namespace COMPUESTOS_API_CS_SQL.Repositories
{
    public class ElementoRepository(PgsqlContext unContexto) : IElementoRepository
    {
        private readonly PgsqlContext contextoDB = unContexto;

        public async Task<List<Elemento>> GetAllAsync()
        {
            var conexion = contextoDB.CreateConnection();

            string sentenciaSQL =
                "SELECT elemento_uuid uuid, nombre, simbolo, numero_atomico, configuracion " +
                "FROM core.elementos ORDER BY numero_atomico, nombre";

            var resultadoElementos = await conexion
                .QueryAsync<Elemento>(sentenciaSQL, new DynamicParameters());

            return resultadoElementos.ToList();
        }


        public Task<Elemento> GetByGuidAsync(Guid elemento_guid)
        {
            throw new NotImplementedException();
        }
        public Task<bool> CreateAsync(Elemento unElemento)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid elemento_guid)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Elemento unElemento)
        {
            throw new NotImplementedException();
        }
    }
}
