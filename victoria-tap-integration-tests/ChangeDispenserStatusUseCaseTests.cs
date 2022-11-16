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
    // Tests are the perfect spot to implement builder pattern
    // Objects that needs different setups depending on the use case tested
    public sealed class ChangeDispenserStatusUseCaseTests
    {
        private readonly HttpClient _httpClient;
        private string _createdDispenserId { get; set; }

        public ChangeDispenserStatusUseCaseTests()
        {
            var webAppFactory = new WebApplicationFactory<Program>();

            _httpClient = webAppFactory.CreateDefaultClient();
        }

        [Fact]
        public async void ChangeDispenserStatus_ToOpen_WithValidData_ShouldReturn202AcceptedStatusCode()
        {
            await WithCreatedDispenser();

            var request = new ChangeDispenserStatusApiRequest 
            { 
                Status = "open", 
                UpdatedAt = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ssZ") 
            };

            string json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/dispenser/{_createdDispenserId}/status", httpContent);

            var result = await response.Content.ReadFromJsonAsync(typeof(bool));

            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.True((bool)result!);
        }

        [Fact]
        public async void ChangeDispenserStatus_ToClose_WithValidData_ShouldReturn202AcceptedStatusCode()
        {
            await WithCreatedDispenser();
            await WithOpenDispenser();

            var request = new ChangeDispenserStatusApiRequest { 
                Status = "closed", 
                UpdatedAt = DateTimeOffset.Now.AddSeconds(10).ToString("yyyy-MM-ddTHH:mm:ssZ") 
            };

            string json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/dispenser/{_createdDispenserId}/status", httpContent);

            var result = await response.Content.ReadFromJsonAsync(typeof(bool));

            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.True((bool)result!);
        }

        [Fact]
        public async void ChangeDispenserStatus_ToOpen_WithAlreadyOpenedTap_ShouldReturn409ConflictStatusCode()
        {
            await WithCreatedDispenser();
            await WithOpenDispenser();

            var request = new ChangeDispenserStatusApiRequest
            {
                Status = "open",
                UpdatedAt = DateTimeOffset.Now.AddSeconds(10).ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            string json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/dispenser/{_createdDispenserId}/status", httpContent);

            var result = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.Contains("Invalid status for current state of dispenser", result);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        [InlineData(null, null)]
        public async void ChangeDispenserStatus_WithEmptyStatusOrDate_ShouldReturn400BadRequestStatusCode(string status, string updatedAt)
        {
            await WithCreatedDispenser();
            await WithOpenDispenser();

            var request = new ChangeDispenserStatusApiRequest
            {
                Status = status,
                UpdatedAt = updatedAt
            };

            string json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/dispenser/{_createdDispenserId}/status", httpContent);

            var result = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void ChangeDispenserStatus_WithInvalidStatus_ShouldReturn400BadRequestStatusCode()
        {
            await WithCreatedDispenser();
            await WithOpenDispenser();

            var request = new ChangeDispenserStatusApiRequest
            {
                Status = "invalid",
                UpdatedAt = DateTimeOffset.Now.AddSeconds(10).ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            string json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/dispenser/{_createdDispenserId}/status", httpContent);

            var result = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("Invalid status. Status must be 'open' or 'closed'", result);
        }

        [Fact]
        public async void ChangeDispenserStatus_WithInvalidDateFormat_ShouldReturn400BadRequestStatusCode()
        {
            await WithCreatedDispenser();
            await WithOpenDispenser();

            var request = new ChangeDispenserStatusApiRequest
            {
                Status = "closed",
                UpdatedAt = "invalid"
            };

            string json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/dispenser/{_createdDispenserId}/status", httpContent);

            var result = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("Invalid ISO date format", result);
        }

        [Fact]
        public async void ChangeDispenserStatus_WithInvalidClosingDate_ShouldReturn400BadRequestStatusCode()
        {
            await WithCreatedDispenser();
            await WithOpenDispenser();

            var request = new ChangeDispenserStatusApiRequest
            {
                Status = "closed",
                UpdatedAt = DateTimeOffset.Now.AddMinutes(-5).ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            string json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/dispenser/{_createdDispenserId}/status", httpContent);

            var result = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("Invalid closing date for opened dispenser", result);
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
