using System.Text.Json.Serialization;

namespace victoria_tap.Services.Contracts
{
    public record CreateDispenserResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("flow_volume")]
        public float FlowVolume { get; set; }
    }
}
