using Npgsql;
using System.Data;

namespace Compuestos_SQL_API.Context
{
    public class PgsqlContext(IConfiguration unaConfiguracion)
    {
        private readonly string cadenaConexion = unaConfiguracion.GetConnectionString("CompuestosPL")!;

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(cadenaConexion);
        }
    }
}