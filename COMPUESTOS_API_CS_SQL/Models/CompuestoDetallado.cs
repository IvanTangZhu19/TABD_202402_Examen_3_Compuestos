using System.Text.Json.Serialization;

namespace COMPUESTOS_API_CS_SQL.Models
{
    public class CompuestoDetallado
    {
        [JsonPropertyName("elementos")]
        public List<ElementoSimplificado>? Elementos { get; set; } = null;


    }
}
