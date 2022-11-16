using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using victoria_tap.Controllers.Contracts;
using victoria_tap.Services.Contracts;
using Xunit;

namespace victoria_tap_integration_tests
{
    public sealed class GetDispenserSpendingInfoUseCaseTests
    {
        private readonly HttpClient _httpClient;
        private string _createdDispenserId { get; set; }

        public GetDispenserSpendingInfoUseCaseTests()
        {
            var webAppFactory = new WebApplicationFactory<Program>();

            _httpClient = webAppFactory.CreateDefaultClient();
        }

        [Fact]
        public async void GetDispenserSpendingInfo_WithDispenserCreated_ShouldReturn200OkStatusCode()
        {
            await WithCreatedDispenser();
            await WithOpenDispenser();

            var response = await _httpClient.GetAsync($"/dispenser/{_createdDispenserId}/spending");

            var result = await response.Content.ReadFromJsonAsync(typeof(DispenserSpendingInfoResponse));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.IsType<DispenserSpendingInfoResponse>(result);
        }

        [Fact]
        public async void GetDispenserSpendingInfo_WithNoDispenser_ShouldReturn404NotFoundStatusCode()
        {
            await WithCreatedDispenser();
            await WithOpenDispenser();

            var response = await _httpClient.GetAsync($"/dispenser/000-000-000-000/spending");

            var result = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Contains("Dispenser does not exist", result);
        }

        private async Task WithCreatedDispenser()
        {
            var request = new CreateDispenserRequest { FlowVolume = 0.064f };

            string json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/dispenser", httpContent);

            var result = await response.Content.ReadFromJsonAsync(typeof(CreateDispenserResponse));

            var castedResult = (result as CreateDispenserResponse);
            _createdDispenserId = castedResult!.Id;
        }

        private async Task WithOpenDispenser()
        {
            var request = new ChangeDispenserStatusApiRequest
            {
                Status = "open",
                UpdatedAt = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            string json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            await _httpClient.PutAsync($"/dispenser/{_createdDispenserId}/status", httpContent);
        }
    }
}
