using Microsoft.Extensions.DependencyInjection;

namespace Todo.NET.Redis.Caching;

public static class RedisCacheExtensions
{
    public static IServiceCollection AddRedisCaching(this IServiceCollection services, Action<RedisCacheOptions> options)
    {
        var redis = new RedisCacheOptions();
        options.Invoke(redis);
        services.AddSingleton(redis);
        services.AddSingleton<IRedisCacheService, RedisCacheService>();
        return services;
    }
}