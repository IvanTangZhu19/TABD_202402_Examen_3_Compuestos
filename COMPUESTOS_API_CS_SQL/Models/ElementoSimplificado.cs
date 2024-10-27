using System.Text.Json.Serialization;

namespace COMPUESTOS_API_CS_SQL.Models
{
    public class ElementoSimplificado
    {
        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; } = string.Empty;


        [JsonPropertyName("cantidad")]
        public int? Cantidad { get; set; } = 0;
    }
}
