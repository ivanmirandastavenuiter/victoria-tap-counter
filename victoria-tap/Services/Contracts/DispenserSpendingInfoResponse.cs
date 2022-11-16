using System.Text.Json.Serialization;

namespace victoria_tap.Services.Contracts
{
    public record DispenserSpendingInfoResponse
    {
        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("usages")]
        public List<UsagesResponse> Usages { get; set; }
    }
}
