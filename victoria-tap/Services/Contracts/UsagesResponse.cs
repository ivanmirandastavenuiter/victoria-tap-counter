using System.Text.Json.Serialization;

namespace victoria_tap.Services.Contracts
{
    public record UsagesResponse
    {
        [JsonPropertyName("opened_at")]
        public string OpenedAt { get; set; }
        [JsonPropertyName("closed_at")]
        public string ClosedAt { get; set; }
        [JsonPropertyName("flow_volume")]

        public float FlowVolume { get; set; }
        [JsonPropertyName("total_spent")]
        public double TotalSpent { get; set; }
    }
}
