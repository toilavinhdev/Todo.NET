using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Todo.NET.Extensions;

public static class SerilogExtensions
{
    public static void SetupSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();
        builder.Host.UseSerilog();
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();
    }
}