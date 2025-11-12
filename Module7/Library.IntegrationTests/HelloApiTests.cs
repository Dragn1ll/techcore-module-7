using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Library.IntegrationTests;

public class HelloApiTests  : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HelloApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task GetHello_ReturnsSuccess()
    {
        // Act
        var response = await _client.GetAsync("/api/hello");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }
}