using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ReftLabsTask.Internal.Core;
using ReftLabsTask.Internal.Core.Models;
using ReftLabsTask.Internal.Infrastructure.Configuration;
using ReftLabsTask.Internal.Infrastructure.Models;

namespace ReftLabsTask.Internal.Infrastructure.Services
{
    public class ExternalUserService : IExternalUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ExternalUserService> _logger;
        private readonly ReqResApiOptions _options;
        public ExternalUserService(HttpClient httpClient, IMemoryCache cache, IOptions<ReqResApiOptions> options, ILogger<ExternalUserService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            const string cacheKey = "all_users";
            if (_cache.TryGetValue(cacheKey, out List<User> cachedUsers))
            {
                return cachedUsers;
            }

            var users = new List<User>();
            int page = 1; // can take as reqeust
            int totalPages; // Can take as request

            do
            {
                var response = await _httpClient.GetAsync($"users?page={page}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<UserListResponseDto>();

                totalPages = result!.Total_Pages;
                users.AddRange(result.Data.Select(dto => new User
                {
                    Id = dto.Id,
                    Email = dto.Email,
                    FirstName = dto.First_Name,
                    LastName = dto.Last_Name
                }));

                page++;
            } while (page <= totalPages);

            _cache.Set(cacheKey, users, TimeSpan.FromSeconds(_options.CacheDurationSeconds));
            return users;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var cacheKey = $"user_{userId}";
            if (_cache.TryGetValue(cacheKey, out User cachedUser))
            {
                return cachedUser;
            }

            try
            {
                var response = await _httpClient.GetAsync($"users/{userId}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new KeyNotFoundException($"User {userId} not found.");

                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<UserResponseDto>();

                var user = new User
                {
                    Id = result!.Data.Id,
                    Email = result.Data.Email,
                    FirstName = result.Data.First_Name,
                    LastName = result.Data.Last_Name
                };

                _cache.Set(cacheKey, user, TimeSpan.FromSeconds(_options.CacheDurationSeconds));
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by ID");
                throw;
            }
        }
    }
}
