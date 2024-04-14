using System.Reflection;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Todo.NET.Hangfire;

public static class HangfireExtensions
{
    public static IServiceCollection AddHangfireMongo(this IServiceCollection services, Action<HangfireMongoOptions> options)
    {
        var mongo = new HangfireMongoOptions();
        options.Invoke(mongo);
        services.AddHangfireMongo(mongo.Prefix, mongo.ConnectionString, mongo.DatabaseName);
        return services;
    }
    
    public static IServiceCollection AddHangfireMongo(this IServiceCollection services, HangfireMongoOptions options)
    {
        services.AddHangfireMongo(options.Prefix, options.ConnectionString, options.DatabaseName);
        return services;
    }
    
    
    public static IServiceCollection AddHangfireMongo(this IServiceCollection services,
                                                      string prefix,
                                                      string connectionString,
                                                      string databaseName)
    {
        services
            .AddHangfire(
                config =>
                {
                    config
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseMongoStorage(
                            new MongoClient(connectionString),
                            databaseName,
                            new MongoStorageOptions
                            {
                                MigrationOptions = new MongoMigrationOptions
                                {
                                    MigrationStrategy = new MigrateMongoMigrationStrategy(),
                                    BackupStrategy = new CollectionMongoBackupStrategy()
                                },
                                Prefix = prefix,
                                CheckConnection = true,
                                CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
                            });
                })
            .AddHangfireServer();
        return services;
    }
    
    public static WebApplication UseHangfireManagement(this WebApplication app, Action<HangfireDashboardConfig> config)
    {
        var hangfire = new HangfireDashboardConfig();
        config.Invoke(hangfire);
        app.UseHangfireManagement(hangfire.Path, hangfire.Title, hangfire.UserName, hangfire.Password);
        return app;
    }

    public static WebApplication UseHangfireManagement(this WebApplication app, HangfireDashboardConfig config)
    {
        app.UseHangfireManagement(config.Path, config.Title, config.UserName, config.Password);
        return app;
    }
    
    public static WebApplication UseHangfireManagement(this WebApplication app, 
                                                       string path,
                                                       string title, 
                                                       string userName, 
                                                       string password)
    {
        app.UseHangfireDashboard(
            path,
            new DashboardOptions
            {
                DashboardTitle = title,
                Authorization = new[]
                {
                    new HangfireCustomBasicAuthenticationFilter
                    {
                        User = userName,
                        Pass = password
                    }
                },
                IgnoreAntiforgeryToken = true
            });
        return app;
    }
    
    public static WebApplication UseHangfireRecurringJobs(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var jobs = scope.ServiceProvider.GetServices<IHangfireCronJob>();
        
        foreach (var recurringJob in jobs)
        {
            var name = recurringJob.GetType().Name;
            var attribute = recurringJob.GetType().GetCustomAttribute<HangfireAttribute>();
            if (attribute is null)
                throw new ArgumentNullException($"{name} can be not null ${nameof(HangfireAttribute)}");
            var cron = attribute.Cron;
            
            RecurringJob.AddOrUpdate(name, () => recurringJob.Run(), cron);
        }

        return app;
    }
}