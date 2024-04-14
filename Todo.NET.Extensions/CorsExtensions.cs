using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Todo.NET.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddPolicyCors(this IServiceCollection services, 
                                                   string policyName, 
                                                   Action<CorsOptions>? options = null)
    {
        services.AddCors(o =>
        {
            o.AddPolicy(policyName, b =>
            {
                b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                b.SetIsOriginAllowed(_ => true);
            });
            
            options?.Invoke(o);
        });
        return services;
    }

    public static IApplicationBuilder UsePolicyCors(this IApplicationBuilder app, string name)
    {
        app.UseCors(name);
        return app;
    }
}