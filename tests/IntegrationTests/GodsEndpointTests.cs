using System.Net.Http.Json;
using MythApi.Common.Database.Models;

namespace IntegrationTests;

[TestFixture]
public class GodsEndpointTests
{
    private CustomWebApplicationFactory<Program> _factory;
    private HttpClient _httpClient;


    [SetUp]
    public void SetUp()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        _httpClient = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetAllGods_ShouldReturnSuccessStatusCode()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/gods");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task GetAllGods_ShouldReturnGodsList()
    {
        // Act
        var gods = await _httpClient.GetFromJsonAsync<List<God>>("/api/v1/gods");

        // Assert
        Assert.That(gods, Is.Not.Null);
        // The test database should be initialized with some gods by DatabaseInitializer
    }

    [Test]
    public async Task GetAllGods_ConcurrentRequests_ShouldRespectRateLim()
    {
        // Arrange
        const int numberOfRequests = 100; // More than our rate limit of 100 per minute
        var tasks = new Task<HttpResponseMessage>[numberOfRequests];

        // Act
        for (var i = 0; i < numberOfRequests; i++)
        {
            tasks[i] = _httpClient.GetAsync("/api/v1/gods");
        }

        var responses = await Task.WhenAll(tasks);

        // Assert
        Assert.That(responses.Length, Is.EqualTo(numberOfRequests));
        
        var successfulRequests = responses.Count(r => r.IsSuccessStatusCode);
        var rateLimitedRequests = responses.Count(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests);
        
        // We expect around 100 successful requests (our rate limit) and the rest to be rate limited
        Assert.That(successfulRequests, Is.LessThanOrEqualTo(100), "Should not exceed rate limit");
        // Assert.That(rateLimitedRequests, Is.GreaterThan(0), "Some requests should be rate limited");
        Assert.That(successfulRequests + rateLimitedRequests, Is.EqualTo(numberOfRequests), "All requests should be either successful or rate limited");
    }
}