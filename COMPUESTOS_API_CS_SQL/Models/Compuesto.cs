using System.Text.Json.Serialization;

namespace COMPUESTOS_API_CS_SQL.Models
{
    public class Compuesto
    {
        [JsonPropertyName("uuid")]
        public Guid Uuid { get; set; } = Guid.Empty;

        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; } = string.Empty;

        [JsonPropertyName("formula")]
        public string? Formula { get; set; } = string.Empty;

        [JsonPropertyName("masa_molar")]
        public float? Masa_molar { get; set; } = 0;

        [JsonPropertyName("estado_agregacion")]
        public string? Estado_agregacion { get; set; } = string.Empty;


    }
}
