using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PatientService.API;
using PatientService.API.Models;
using Xunit;

namespace PatientService.Tests.Controllers;

public class MetadataControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MetadataControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            // configure test services if needed
        }).CreateClient();
    }

    [Fact]
    public async Task GetGender_ReturnsSeededValues()
    {
        var response = await _client.GetAsync("/api/metadata/gender");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<MetadataDto>>();
        Assert.Contains(list!, g => g.Name == "Male");
    }
}
