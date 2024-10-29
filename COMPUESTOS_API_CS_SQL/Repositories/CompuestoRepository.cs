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
        //Trae todos los compuestos (sin elementos)
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

        //Trae un compuesto por guid
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

        //Trae los elementos de un compuesto dado el guid (incluyendo el compuesto)
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

        //Trae los elementos de un compuesto (en el metodo anterior lo llama)
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

        //Trae un compuesto por nombre
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
        //Crea un compuesto, incluyendo los elementos
        public async Task<bool> CreateAsync(CompuestoDetallado unCompuestoDetallado)
        {
            bool resultadoAccion = false;

            var conexion = contextoDB.CreateConnection();

            try
            {
                int compuestoID, elementoID;
                //Nombre procedimiento para añadir un compuesto
                string procedimientoCompuesto = "core.p_insertar_compuesto";
                //Nombre procedimiento para añadir un elemento al compuesto
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
                    //Metodo que trae el id del compuesto dado el nombre
                    compuestoID = await GetIDCompoundAsync(unCompuestoDetallado.Nombre!);
                    //Se recorre la lista de elementos, para añadir los elementos del compuesto
                    foreach (ElementoSimplificado elementoSimplificado in 
                        unCompuestoDetallado.Elementos!)
                    {
                        //Trae el id del elemento dado el nombre
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

        //Trae el id del compuesto dado el nombre
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
        //Trea el id del elemento dado el nombre
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
        //Actualiza el compuesto, incluyendo los elementos
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
                        //Similar al de crear compuesto
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
        //Elimina un compuesto dado el guid, incluyendo los elementos asociados
        public async Task<bool> DeleteAsync(Guid compuesto_guid)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB.CreateConnection();

                int elementoID, compuestoID, cantidad_filas= 0;
                string procedimiento = "core.p_eliminar_compuesto";
                string procedimientoElementos = "core.p_eliminar_elementos_compuesto";

                var ElementosCompuesto = await GetElementsDetailsAsync(compuesto_guid);

                if(ElementosCompuesto.Count > 0)
                {
                    //Se trae el compuesto por guid, se indica el nombre, el cual es el 
                    //parámetro para traer el id del compuesto
                    compuestoID = await GetIDCompoundAsync((
                        await GetByGuidAsync(compuesto_guid)).Nombre!);
                    //Primero se eliminan las filas de la tabla elementos_compuesto (por dependencia)
                    //Recorre la lista de elementos registrados del compuesto
                    foreach (ElementoSimplificado elementoSimplificado in ElementosCompuesto)
                    {
                        elementoID = await GetIDElementAsync(elementoSimplificado.Nombre!);
                        var parametrosElementos = new
                        {
                            p_compuestoid = compuestoID,
                            p_elementoid = elementoID,
                        };
                        cantidad_filas += await conexion
                            .ExecuteAsync(
                                procedimientoElementos,
                                parametrosElementos,
                                commandType: CommandType.StoredProcedure);
                    }
                }
                
                var parametros = new
                {
                    p_uuid = compuesto_guid
                };
                //Después de eliminar los elementos, se elimina el compuesto
                cantidad_filas = await conexion.ExecuteAsync(
                    procedimiento,
                    parametros,
                    commandType: CommandType.StoredProcedure);

                if (cantidad_filas != 0)
                    resultadoAccion = true;
            }
            catch (NpgsqlException error)
            {
                throw new DbOperationException(error.Message);
            }

            return resultadoAccion;
        }


    }
}
