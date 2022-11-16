using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace victoria_tap.Controllers.Contracts
{
    public record ChangeDispenserStatusApiRequest
    {
        [JsonPropertyName("status")]
        [Required]
        [FromBody]
        public string Status { get; set; }
        [JsonPropertyName("updated_at")]
        [Required]
        [FromBody]
        public string UpdatedAt { get; set; }
    }
}
