using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReftLabsTask.Internal.Core.Models;
using ReftLabsTask.Internal.Core;
using ReftLabsTask.Internal.Infrastructure.Configuration;
using ReftLabsTask.Internal.Infrastructure.Services;
using System.Net;
using Moq;
using RichardSzalay.MockHttp;

namespace ReftLabsTask.Tests;

public class ExternalUserServiceTests
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IExternalUserService _service;

    public ExternalUserServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();

        var client = new HttpClient(_mockHttp)
        {
            BaseAddress = new Uri("https://reqres.in/api/")
        };

        var options = Options.Create(new ReqResApiOptions
        {
            BaseUrl = "https://reqres.in/api/",
            CacheDurationSeconds = 60
        });

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var logger = Mock.Of<ILogger<ExternalUserService>>();

        _service = new ExternalUserService(client, memoryCache, options, logger);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        _mockHttp.When($"https://reqres.in/api/users/{userId}")
                 .Respond("application/json", @"
                     {
                         ""data"": {
                             ""id"": 1,
                             ""email"": ""george.bluth@reqres.in"",
                             ""first_name"": ""George"",
                             ""last_name"": ""Bluth""
                         }
                     }");

        // Act
        var user = await _service.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(1, user.Id);
        Assert.Equal("George", user.FirstName);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsNull_WhenUserNotFound()
    {
        // Arrange
        var userId = 999;
        _mockHttp.When($"https://reqres.in/api/users/{userId}")
                 .Respond(HttpStatusCode.NotFound);

        // Act
        var user = await _service.GetUserByIdAsync(userId);

        // Assert
        Assert.Null(user);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        _mockHttp.When("https://reqres.in/api/users?page=1")
                 .Respond("application/json", @"
                     {
                         ""page"": 1,
                         ""total_pages"": 2,
                         ""data"": [
                             { ""id"": 1, ""email"": ""a@a.com"", ""first_name"": ""A"", ""last_name"": ""One"" }
                         ]
                     }");

        _mockHttp.When("https://reqres.in/api/users?page=2")
                 .Respond("application/json", @"
                     {
                         ""page"": 2,
                         ""total_pages"": 2,
                         ""data"": [
                             { ""id"": 2, ""email"": ""b@b.com"", ""first_name"": ""B"", ""last_name"": ""Two"" }
                         ]
                     }");

        // Act
        var users = await _service.GetAllUsersAsync();

        // Assert
        Assert.NotNull(users);
        var list = new List<User>(users);
        Assert.Equal(2, list.Count);
        Assert.Contains(list, u => u.Id == 1);
        Assert.Contains(list, u => u.Id == 2);
    }
}
