using System.Net;
using System.Text.Json;
using Library.Contracts.Books.Response;

namespace Library.IntegrationTests;

public class BookControllerIntegrationTests : IClassFixture<MyTestFactory>
{
    private readonly HttpClient _client;

    public BookControllerIntegrationTests(MyTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetBooks_ReturnsOkAndList()
    {
        // Act
        using var response = await _client.GetAsync("/api/books");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();

        var books = JsonSerializer.Deserialize<List<GetBookResponse>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(books);
    }
}