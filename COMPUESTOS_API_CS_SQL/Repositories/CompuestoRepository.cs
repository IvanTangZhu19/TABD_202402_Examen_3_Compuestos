using COMPUESTOS_API_CS_SQL.Contexts;
using COMPUESTOS_API_CS_SQL.Exceptions;
using COMPUESTOS_API_CS_SQL.Interfaces;
using COMPUESTOS_API_CS_SQL.Models;
using Dapper;
using Npgsql;
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

        public async Task<Compuesto> GetByNameAsync(string nombre)
        {
            Compuesto unCompuesto = new();

            var conexion = contextoDB.CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@nombre", nombre,
                                    DbType.String, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT compuesto_uuid uuid, nombre, formula, masa_molar, estado_agregacion  " +
                "FROM core.compuestos " +
                "WHERE nombre = @nombre ";


            var resultado = await conexion.QueryAsync<Compuesto>(sentenciaSQL,
                parametrosSentencia);

            if (resultado.Any())
                unCompuesto = resultado.First();

            return unCompuesto;
        }
        public async Task<bool> CreateAsync(CompuestoDetallado unCompuestoDetallado)
        {
            bool resultadoAccion = false;

            var conexion = contextoDB.CreateConnection();

            try
            {
                int compuestoID, elementoID;
                string procedimientoCompuesto = "core.p_insertar_compuesto";
                string procedimientoElementosCompuesto = "core.p_insertar_elemento_compuesto";

                var parametros = new
                {
                    p_nombre = unCompuestoDetallado.Nombre,
                    p_formula = unCompuestoDetallado.Formula,
                    p_masa_molar = unCompuestoDetallado.Masa_molar,
                    p_estado = unCompuestoDetallado.Estado_agregacion
                };

                var cantidadFilas = await conexion
                    .ExecuteAsync(
                        procedimientoCompuesto,
                        parametros,
                        commandType: CommandType.StoredProcedure);

                if (cantidadFilas != 0)
                {
                    compuestoID = await GetIDCompoundAsync(unCompuestoDetallado.Nombre!);
                    foreach (ElementoSimplificado elementoSimplificado in 
                        unCompuestoDetallado.Elementos!)
                    {
                        elementoID = await GetIDElementAsync(elementoSimplificado.Nombre!);
                        var parametrosElementos = new
                        {
                            p_compuestoid = compuestoID,
                            p_elementoid = elementoID,
                            p_cantidad = elementoSimplificado.Cantidad

                        };
                        cantidadFilas += await conexion
                            .ExecuteAsync(
                                procedimientoElementosCompuesto,
                                parametrosElementos,
                                commandType: CommandType.StoredProcedure);
                    }
                    if (cantidadFilas != 0) 
                        resultadoAccion = true;
                }
            }
            catch (NpgsqlException error)
            {
                throw new DbOperationException(error.Message);
            }

            return resultadoAccion;
        }

        public async Task<int> GetIDCompoundAsync(string nombre)
        {
            var conexion = contextoDB.CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@nombre", nombre,
                                    DbType.String, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT id " +
                "FROM core.compuestos " +
                "WHERE nombre = @nombre";

            var resultado = await conexion.QueryAsync<int>(sentenciaSQL,
                parametrosSentencia);

            return resultado.FirstOrDefault();
        }

        //Devuelve el id del elementos por nombre
        public async Task<int> GetIDElementAsync(string nombre)
        {

            var conexion = contextoDB.CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@elemento_nombre", nombre,
                                    DbType.String, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT id " +
                "FROM core.elementos " +
                "WHERE nombre = @elemento_nombre";

            var resultado = await conexion.QueryAsync<int>(sentenciaSQL,
                parametrosSentencia);

            return resultado.First();
        }

        public async Task<bool> UpdateAsync(CompuestoDetallado unCompuestoDetallado)
        {
            {
                bool resultadoAccion = false;

                try
                {
                    var conexion = contextoDB.CreateConnection();
                    int compuestoID, elementoID;

                    string procedimiento = "core.p_actualizar_compuesto";
                    string procedimientoElementosCompuesto = "core.p_actualizar_insertar_elemento_compuesto";

                    var parametros = new
                    {
                        p_uuid = unCompuestoDetallado.Uuid,
                        p_nombre = unCompuestoDetallado.Nombre,
                        p_formula = unCompuestoDetallado.Formula,
                        p_masa_molar = unCompuestoDetallado.Masa_molar,
                        p_estado = unCompuestoDetallado.Estado_agregacion
                    };

                    var cantidad_filas = await conexion.ExecuteAsync(
                        procedimiento,
                        parametros,
                        commandType: CommandType.StoredProcedure);

                    if (cantidad_filas != 0)
                    {
                        compuestoID = await GetIDCompoundAsync(unCompuestoDetallado.Nombre!);
                        foreach (ElementoSimplificado elementoSimplificado in
                            unCompuestoDetallado.Elementos!)
                        {
                            elementoID = await GetIDElementAsync(elementoSimplificado.Nombre!);
                            var parametrosElementos = new
                            {
                                p_compuestoid = compuestoID,
                                p_elementoid = elementoID,
                                p_cantidad = elementoSimplificado.Cantidad

                            };
                            cantidad_filas += await conexion
                                .ExecuteAsync(
                                    procedimientoElementosCompuesto,
                                    parametrosElementos,
                                    commandType: CommandType.StoredProcedure);
                        }
                        if (cantidad_filas != 0)
                            resultadoAccion = true;
                    }
                }
                catch (NpgsqlException error)
                {
                    throw new DbOperationException(error.Message);
                }

                return resultadoAccion;
            }
        }

        public Task<bool> DeleteAsync(Guid elemento_guid)
        {
            throw new NotImplementedException();
        }
    }
}
