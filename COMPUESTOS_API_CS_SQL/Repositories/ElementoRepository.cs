using COMPUESTOS_API_CS_SQL.Contexts;
using COMPUESTOS_API_CS_SQL.Interfaces;
using COMPUESTOS_API_CS_SQL.Models;
using Dapper;
using System.Data;

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


        public async Task<Elemento> GetByGuidAsync(Guid elemento_guid)
        {
            Elemento unElemento = new();

            var conexion = contextoDB.CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@elemento_guid", elemento_guid,
                                    DbType.Guid, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT elemento_uuid uuid, nombre, simbolo, numero_atomico, configuracion  " +
                "FROM core.elementos " +
                "WHERE elemento_uuid = @elemento_guid ";


            var resultado = await conexion.QueryAsync<Elemento>(sentenciaSQL,
                parametrosSentencia);

            if (resultado.Any())
                unElemento = resultado.First();

            return unElemento;
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
