using COMPUESTOS_API_CS_SQL.Contexts;
using COMPUESTOS_API_CS_SQL.Interfaces;
using COMPUESTOS_API_CS_SQL.Models;
using Dapper;
using System.Data;

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

        public async Task<Compuesto> GetByGuidAsync(Guid compuesto_guid)
        {
            Compuesto unCompuesto = new();
            var conexion = contextoDB.CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@compuesto_guid", compuesto_guid,
                                    DbType.Guid, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT compuesto_uuid uuid, nombre, formula, masa_molar, estado_agregacion " +
                "FROM core.compuestos " +
                "WHERE compuesto_uuid = @compuesto_guid";

            var resultadoCompuesto = await conexion
                .QueryAsync<Compuesto>(sentenciaSQL, parametrosSentencia);

            if(resultadoCompuesto.Any()) unCompuesto = resultadoCompuesto.First();

            return unCompuesto;
        }

        public async Task<CompuestoDetallado> GetDetailedCompoundByGuidAsync(Guid compuesto_guid)
        {
            Compuesto unCompuesto = await GetByGuidAsync(compuesto_guid);

            CompuestoDetallado unCompuestoDetallado = new()
            {
                Uuid = unCompuesto.Uuid,
                Nombre = unCompuesto.Nombre,
                Formula = unCompuesto.Formula,
                Masa_molar = unCompuesto.Masa_molar,
                Estado_agregacion = unCompuesto.Estado_agregacion,
                Elementos = await GetElementsDetailsAsync(compuesto_guid)
            };

            return unCompuestoDetallado;
        }

        public async Task<List<ElementoSimplificado>> GetElementsDetailsAsync(Guid compuesto_guid)
        {
            List<ElementoSimplificado> infoElementos = [];

            var conexion = contextoDB.CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@compuesto_guid", compuesto_guid,
                                    DbType.Guid, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT DISTINCT elemento nombre, cantidad " +
                "FROM v_info_compuestos " +
                "WHERE compuesto_uuid = @compuesto_guid " +
                "ORDER BY elemento ";

            var resultado = await conexion
                .QueryAsync<ElementoSimplificado>(sentenciaSQL, parametrosSentencia);

            if (resultado.Any())
                infoElementos = resultado.ToList();

            return infoElementos;
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
