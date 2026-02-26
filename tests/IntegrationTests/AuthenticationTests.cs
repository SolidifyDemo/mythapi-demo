using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MythApi.Common.Database.Models;
using MythApi.Gods.Models;

namespace IntegrationTests;

[TestFixture]
public class AuthenticationTests
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
    public async Task GetAllGods_WithoutAuthentication_ShouldSucceed()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/gods");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetGodById_WithoutAuthentication_ShouldSucceed()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/gods/1");

        // Assert - GET endpoints should be public
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task PostGods_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var newGod = new List<GodInput>
        {
            new GodInput
            {
                Name = "TestGod",
                MythologyId = 1,
                Description = "Test Description"
            }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", newGod);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task PostGods_WithUserToken_ShouldReturnForbidden()
    {
        // Arrange
        var token = TestJwtTokenGenerator.GenerateUserToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var newGod = new List<GodInput>
        {
            new GodInput
            {
                Name = "TestGod",
                MythologyId = 1,
                Description = "Test Description"
            }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", newGod);

        // Assert - User role should not have Admin access
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task PostGods_WithAdminToken_ShouldSucceed()
    {
        // Arrange
        var token = TestJwtTokenGenerator.GenerateAdminToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var newGod = new List<GodInput>
        {
            new GodInput
            {
                Name = "TestGod",
                MythologyId = 1,
                Description = "Test Description"
            }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", newGod);

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        
        var gods = await response.Content.ReadFromJsonAsync<List<God>>();
        Assert.That(gods, Is.Not.Null);
        // The endpoint returns all gods, not just the newly created one
        Assert.That(gods, Is.Not.Empty);
        Assert.That(gods!.Any(g => g.Name == "TestGod"), Is.True);
    }

    [Test]
    public async Task PostGods_WithInvalidToken_ShouldReturnUnauthorized()
    {
        // Arrange
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid.token.here");
        
        var newGod = new List<GodInput>
        {
            new GodInput
            {
                Name = "TestGod",
                MythologyId = 1,
                Description = "Test Description"
            }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", newGod);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task PostGods_WithExpiredToken_ShouldReturnUnauthorized()
    {
        // Arrange - Generate a token that expired 10 minutes ago
        var token = TestJwtTokenGenerator.GenerateToken("admin", "Admin", expirationMinutes: -10);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var newGod = new List<GodInput>
        {
            new GodInput
            {
                Name = "TestGod",
                MythologyId = 1,
                Description = "Test Description"
            }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/gods", newGod);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetAllMythologies_WithoutAuthentication_ShouldSucceed()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/mythologies");

        // Assert - GET endpoints should be public
        Assert.That(response.IsSuccessStatusCode, Is.True);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
