using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Todo.NET.Extensions;

public static class EnvironmentExtensions
{
    public static void SetupEnvironment<T>(this WebApplicationBuilder builder, 
                                           string? external, 
                                           out T configs) where T : class, new()
    {
        configs = new T();
        var environment = builder.Environment;
        var json = $"appsettings.{environment.EnvironmentName}.json";
        var path = string.IsNullOrEmpty(external) ? json : Path.Combine(external, json);
        
        new ConfigurationBuilder()
            .SetBasePath(environment.ContentRootPath)
            .AddJsonFile(path)
            .AddEnvironmentVariables()
            .Build()
            .Bind(configs);

        builder.Services.AddSingleton(configs);
    }
}