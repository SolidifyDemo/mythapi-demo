using System.Net;
using System.Net.Http.Json;
using MythApi.Common.Database.Models;

namespace IntegrationTests;

[TestFixture]
public class MythologiesEndpointTests
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
    public async Task GetAllMythologies_ShouldReturnSuccessStatusCode()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/mythologies");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task GetAllMythologies_ShouldReturnMythologiesList()
    {
        // Act
        var mythologies = await _httpClient.GetFromJsonAsync<List<Mythology>>("/api/v1/mythologies");

        // Assert
        Assert.That(mythologies, Is.Not.Null);
        Assert.That(mythologies.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetMythologyById_ShouldReturnSuccessStatusCode_WhenMythologyExists()
    {
        // Arrange - First get all mythologies to find a valid ID
        var mythologies = await _httpClient.GetFromJsonAsync<List<Mythology>>("/api/v1/mythologies");
        Assert.That(mythologies, Is.Not.Null);
        Assert.That(mythologies.Count, Is.GreaterThan(0));
        var validId = mythologies[0].Id;

        // Act
        var response = await _httpClient.GetAsync($"/api/v1/mythologies/{validId}");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task GetMythologyById_ShouldReturnMythology_WhenMythologyExists()
    {
        // Arrange - First get all mythologies to find a valid ID
        var mythologies = await _httpClient.GetFromJsonAsync<List<Mythology>>("/api/v1/mythologies");
        Assert.That(mythologies, Is.Not.Null);
        Assert.That(mythologies.Count, Is.GreaterThan(0));
        var validId = mythologies[0].Id;

        // Act
        var mythology = await _httpClient.GetFromJsonAsync<Mythology>($"/api/v1/mythologies/{validId}");

        // Assert
        Assert.That(mythology, Is.Not.Null);
        Assert.That(mythology.Id, Is.EqualTo(validId));
        Assert.That(mythology.Name, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task GetMythologyById_ShouldIncludeGods_WhenMythologyExists()
    {
        // Arrange - First get all mythologies to find a valid ID
        var mythologies = await _httpClient.GetFromJsonAsync<List<Mythology>>("/api/v1/mythologies");
        Assert.That(mythologies, Is.Not.Null);
        Assert.That(mythologies.Count, Is.GreaterThan(0));
        var validId = mythologies[0].Id;

        // Act
        var mythology = await _httpClient.GetFromJsonAsync<Mythology>($"/api/v1/mythologies/{validId}");

        // Assert
        Assert.That(mythology, Is.Not.Null);
        Assert.That(mythology.Gods, Is.Not.Null);
        // Gods collection should be present (may be empty or populated depending on seed data)
    }

    [Test]
    public async Task GetMythologyById_ShouldReturn404_WhenMythologyDoesNotExist()
    {
        // Arrange
        var nonExistentId = 999999;

        // Act
        var response = await _httpClient.GetAsync($"/api/v1/mythologies/{nonExistentId}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetMythologyById_ShouldReturn404_WhenIdIsZero()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/mythologies/0");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetMythologyById_ShouldReturn404_WhenIdIsNegative()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/mythologies/-1");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetMythologyById_ShouldReturn404_WhenIdIsMaxValue()
    {
        // Act
        var response = await _httpClient.GetAsync($"/api/v1/mythologies/{int.MaxValue}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetMythologyById_ShouldHandleMultipleSequentialRequests()
    {
        // Arrange - Get a valid ID
        var mythologies = await _httpClient.GetFromJsonAsync<List<Mythology>>("/api/v1/mythologies");
        Assert.That(mythologies, Is.Not.Null);
        Assert.That(mythologies.Count, Is.GreaterThan(0));
        var validId = mythologies[0].Id;

        // Act - Make multiple sequential requests
        var response1 = await _httpClient.GetAsync($"/api/v1/mythologies/{validId}");
        var response2 = await _httpClient.GetAsync($"/api/v1/mythologies/{validId}");
        var response3 = await _httpClient.GetAsync($"/api/v1/mythologies/{validId}");

        // Assert
        Assert.That(response1.IsSuccessStatusCode, Is.True);
        Assert.That(response2.IsSuccessStatusCode, Is.True);
        Assert.That(response3.IsSuccessStatusCode, Is.True);
    }
}
