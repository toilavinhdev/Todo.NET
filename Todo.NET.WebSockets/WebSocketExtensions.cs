using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Todo.NET.WebSockets;

public static class WebSocketExtensions
{
    public static IServiceCollection AddWebSocketManager<TAssembly>(this IServiceCollection services)
    {
        services.AddTransient<WebSocketConnectionManager>();
        
        var exportedType = typeof(TAssembly).Assembly.ExportedTypes;
        exportedType = exportedType.Where(x => x.GetTypeInfo().BaseType == typeof(WebSocketHandler));

        foreach (var type in exportedType)
        {
            services.AddSingleton(type);
        }
        
        return services;
    }
    
    public static WebApplication MapWebSocketHandler<THandler>(this WebApplication app, PathString path)
    {
        var handler = app.Services.GetService<THandler>();
        app.Map(path, builder => builder.UseMiddleware<WebSocketMiddleware>(handler));
        return app;
    }
}