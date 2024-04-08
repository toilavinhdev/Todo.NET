using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace Todo.NET.Extensions;

public static class StaticFilesExtensions
{
    public static IApplicationBuilder UsePhysicalStaticFiles(this IApplicationBuilder app, string location, string? external)
    {
        if (!Directory.Exists(location)) Directory.CreateDirectory(location);
        app.UseStaticFiles(
            new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(location),
                RequestPath = new PathString(external)
            });
        return app;
    }
}