﻿using System.Text.Json.Serialization;

namespace COMPUESTOS_API_CS_SQL.Models
{
    public class Elemento
    {
        [JsonPropertyName("uuid")]
        public Guid Uuid { get; set; } = Guid.Empty;

        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; } = string.Empty;

        [JsonPropertyName("simbolo")]
        public string? Simbolo { get; set; } = string.Empty;

        [JsonPropertyName("numero_atomico")]
        public int? Numero_atomico { get; set; } = 0;

        [JsonPropertyName("configuracion")]
        public string? Configuracion { get; set; } = string.Empty;
    }
}