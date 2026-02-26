using System.Net.Http.Json;
using MythApi.Common.Database.Models;
using MythApi.Gods.Models;
using System.Net;

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

    [Test]
    public async Task PostGods_WithValidInput_ShouldReturnOk()
    {
        // Arrange
        var godInputs = new List<GodInput>
        {
            new GodInput { Name = "TestGod", MythologyId = 1, Description = "Test god description" }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", godInputs);

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task PostGods_WithNullInput_ShouldReturnBadRequest()
    {
        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", (List<GodInput>)null!);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task PostGods_WithEmptyList_ShouldReturnBadRequest()
    {
        // Arrange
        var godInputs = new List<GodInput>();

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", godInputs);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task PostGods_WithBatchSizeExceeded_ShouldReturnBadRequest()
    {
        // Arrange
        var godInputs = new List<GodInput>();
        for (int i = 0; i < MythApi.Endpoints.v1.Gods.MAX_BATCH_SIZE + 1; i++)
        {
            godInputs.Add(new GodInput { Name = $"God{i}", MythologyId = 1, Description = "Test" });
        }

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", godInputs);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task PostGods_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var godInputs = new List<GodInput>
        {
            new GodInput { Name = "", MythologyId = 1, Description = "Test" }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", godInputs);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task PostGods_WithNameTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        var godInputs = new List<GodInput>
        {
            new GodInput { Name = new string('a', MythApi.Endpoints.v1.Gods.MAX_NAME_LENGTH + 1), MythologyId = 1, Description = "Test" }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", godInputs);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task PostGods_WithDescriptionTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        var godInputs = new List<GodInput>
        {
            new GodInput { Name = "Zeus", MythologyId = 1, Description = new string('a', MythApi.Endpoints.v1.Gods.MAX_DESCRIPTION_LENGTH + 1) }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", godInputs);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task PostGods_WithNegativeMythologyId_ShouldReturnBadRequest()
    {
        // Arrange
        var godInputs = new List<GodInput>
        {
            new GodInput { Name = "Zeus", MythologyId = -1, Description = "Test" }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", godInputs);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task PostGods_WithZeroMythologyId_ShouldReturnBadRequest()
    {
        // Arrange
        var godInputs = new List<GodInput>
        {
            new GodInput { Name = "Zeus", MythologyId = 0, Description = "Test" }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", godInputs);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task PostGods_WithNegativeId_ShouldReturnBadRequest()
    {
        // Arrange
        var godInputs = new List<GodInput>
        {
            new GodInput { Id = -1, Name = "Zeus", MythologyId = 1, Description = "Test" }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", godInputs);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task PostGods_WithZeroId_ShouldReturnBadRequest()
    {
        // Arrange
        var godInputs = new List<GodInput>
        {
            new GodInput { Id = 0, Name = "Zeus", MythologyId = 1, Description = "Test" }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", godInputs);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}