using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using victoria_tap.Controllers.Contracts;
using victoria_tap.Services.Contracts;
using Xunit;

namespace victoria_tap_integration_tests
{
    public sealed class CreateDispenserUseCaseTests
    {
        private readonly HttpClient _httpClient;

        public CreateDispenserUseCaseTests()
        {
            var webAppFactory = new WebApplicationFactory<Program>();

            _httpClient = webAppFactory.CreateDefaultClient();
        }

        [Fact]
        public async void CreateDispenser_WithValidData_ShouldReturn200OkStatusCode()
        {
            var request = new CreateDispenserRequest { FlowVolume = 0.064f };

            string json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/dispenser", httpContent);

            var result = await response.Content.ReadFromJsonAsync(typeof(CreateDispenserResponse));

            var castedResult = (result as CreateDispenserResponse);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.IsType<CreateDispenserResponse>(result);
            Assert.Equal(0.064, Math.Round(castedResult!.FlowVolume, 3));
            Assert.NotNull((result as CreateDispenserResponse)!.Id);
        }

        [Fact]
        public async void CreateDispenser_WithInvalidData_ShouldReturn400BadRequestCode()
        {
            var request = new CreateDispenserRequest { FlowVolume = -0.064f };

            string json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/dispenser", httpContent);

            var result = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}