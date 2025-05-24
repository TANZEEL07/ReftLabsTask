using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using ReftLabsTask.Internal.Core;
using ReftLabsTask.Internal.Infrastructure.Configuration;
using ReftLabsTask.Internal.Infrastructure.Services;

namespace ReftLabsTask.Internal.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReftLabsTask(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ReqResApiOptions>(config.GetSection("ReqResApi"));
        services.AddMemoryCache();

        services.AddHttpClient<IExternalUserService, ExternalUserService>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ReqResApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
        })
        .AddPolicyHandler(GetRetryPolicy());

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));
    }
}
