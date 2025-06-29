﻿using COMPUESTOS_API_CS_SQL.Contexts;
using COMPUESTOS_API_CS_SQL.Exceptions;
using COMPUESTOS_API_CS_SQL.Interfaces;
using COMPUESTOS_API_CS_SQL.Models;
using Dapper;
using Npgsql;
using System.Data;

namespace COMPUESTOS_API_CS_SQL.Repositories
{
    public class ElementoRepository(PgsqlContext unContexto) : IElementoRepository
    {
        private readonly PgsqlContext contextoDB = unContexto;

        //Trae todos los elementos de la base de datos
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

        //Trae un elemento de la base de datos por Guid
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
        //Crea un nuevo elemento
        public async Task<bool> CreateAsync(Elemento unElemento)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB.CreateConnection();

                string procedimiento = "core.p_insertar_elemento";

                var parametros = new
                {
                    p_nombre = unElemento.Nombre,
                    p_simbolo = unElemento.Simbolo,
                    p_numero_atomico = unElemento.Numero_atomico,
                    p_configuracion = unElemento.Configuracion
                };

                var cantidadFilas = await conexion
                    .ExecuteAsync(
                        procedimiento,
                        parametros,
                        commandType: CommandType.StoredProcedure);

                if (cantidadFilas != 0)
                    resultadoAccion = true;
            }
            catch (NpgsqlException error)
            {
                throw new DbOperationException(error.Message);
            }

            return resultadoAccion;
        }


        //Actualiza un elemento
        public async Task<bool> UpdateAsync(Elemento unElemento)
        {
            bool resultadoAccion = false;

            //Se verifica se existe el elemento
            var elementoExistente = await GetByGuidAsync(unElemento.Uuid);

            if (elementoExistente.Uuid == Guid.Empty)
                throw new DbOperationException($"No se puede actualizar. No existe el elemento {unElemento.Nombre!}.");

            try
            {
                var conexion = contextoDB.CreateConnection();

                string procedimiento = "core.p_actualizar_elemento";
                var parametros = new
                {
                    p_uuid = unElemento.Uuid,
                    p_nombre = unElemento.Nombre,
                    p_simbolo = unElemento.Simbolo,
                    p_numero_atomico = unElemento.Numero_atomico,
                    p_configuracion = unElemento.Configuracion
                };

                var cantidad_filas = await conexion.ExecuteAsync(
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

        //Se elimina un elemento
        public async Task<bool> DeleteAsync(Guid elemento_guid)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB.CreateConnection();

                string procedimiento = "core.p_eliminar_elemento";
                var parametros = new
                {
                    p_uuid = elemento_guid
                };

                var cantidad_filas = await conexion.ExecuteAsync(
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

        //Devuelve un int del numero de compuestos que contienen un elemento por guid
        public async Task<int> GetTotalAssociatedCompoundsByElementGuidAsync(Guid elemento_guid)
        {
            var conexion = contextoDB.CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@elemento_guid", elemento_guid,
                                    DbType.Guid, ParameterDirection.Input);

            
            string sentenciaSQL = "SELECT count(*) totalRegistros " +
                "FROM core.v_info_compuestos " +
                "WHERE elemento_uuid = @elemento_guid";

            var totalRegistros = await conexion
                .QueryAsync<int>(sentenciaSQL, parametrosSentencia);

            return totalRegistros.FirstOrDefault();
        }

        //Se verifica que los valores únicos nombre, simbolo y numero_atomico no tengan
        //registros en la base de datos
        public async Task<Elemento> checkUniqueValuesAsync(Elemento unElemento)
        {
            var conexion = contextoDB.CreateConnection();
            Elemento elemento = new();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@nombre", unElemento.Nombre,
                                    DbType.String, ParameterDirection.Input);
            parametrosSentencia.Add("@simbolo", unElemento.Simbolo,
                                    DbType.String, ParameterDirection.Input);
            parametrosSentencia.Add("@numero_atomico", unElemento.Numero_atomico,
                                    DbType.Int32, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT id " +
                "FROM core.elementos " +
                "WHERE nombre = @nombre OR simbolo = @simbolo OR numero_atomico = @numero_atomico";


            var resultado = await conexion.QueryAsync<Elemento>(sentenciaSQL,
                parametrosSentencia);

            if(resultado.Any()) elemento = resultado.First();
            //count = resultado.Count();

            //if (count > 0) return true;
            //else return false;

            return elemento;

        }

        //Trae un elemento dado un nombre
        public async Task<Elemento> GetByNameAsync(string nombre)
        {
            Elemento unElemento = new();

            var conexion = contextoDB.CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@nombre", nombre,
                                    DbType.String, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT elemento_uuid uuid, nombre, simbolo, numero_atomico, configuracion  " +
                "FROM core.elementos " +
                "WHERE nombre = @nombre ";

            var resultado = await conexion.QueryAsync<Elemento>(sentenciaSQL,
                parametrosSentencia);

            if (resultado.Any())
                unElemento = resultado.First();

            return unElemento;
        }

    }
}
