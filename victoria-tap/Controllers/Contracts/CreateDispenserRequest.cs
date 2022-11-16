using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace victoria_tap.Controllers.Contracts
{
    public record CreateDispenserRequest
    {
        [JsonPropertyName("flow_volume")]
        [Required]
        [FromBody]
        public float FlowVolume { get; set; }
    }
}
