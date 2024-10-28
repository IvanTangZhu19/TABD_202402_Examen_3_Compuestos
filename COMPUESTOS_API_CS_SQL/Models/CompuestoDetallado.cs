using System.Text.Json.Serialization;

namespace COMPUESTOS_API_CS_SQL.Models
{
    public class CompuestoDetallado: Compuesto
    {
        [JsonPropertyName("elementos")]
        public List<ElementoSimplificado>? Elementos { get; set; } = null;


    }
}
