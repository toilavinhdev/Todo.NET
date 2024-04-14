using Microsoft.Extensions.DependencyInjection;

namespace Todo.NET.Mongo;

public static class MongoExtensions
{
    public static IServiceCollection AddMongoContext<TContext>(this IServiceCollection services) where TContext : MongoContext
    {
        
        return services;
    }
}