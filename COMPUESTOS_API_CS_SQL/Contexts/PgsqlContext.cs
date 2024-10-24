using Npgsql;
using System.Data;

namespace COMPUESTOS_API_CS_SQL.Contexts
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