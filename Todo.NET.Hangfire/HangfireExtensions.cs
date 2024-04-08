﻿using System.Reflection;
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
    public static IServiceCollection AddHangfireMongo(this IServiceCollection services,
                                                              string prefix,
                                                              Action<HangfireMongoConfig> databaseConfig)
    {
        var mongoConfig = new HangfireMongoConfig();
        databaseConfig(mongoConfig);
        
        services
            .AddHangfire(
                config =>
                {
                    config
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseMongoStorage(
                            new MongoClient(mongoConfig.ConnectionString),
                            mongoConfig.DatabaseName,
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
    
    public static WebApplication UseHangfireManagement(this WebApplication app, Action<HangfireConfig> config)
    {
        var hangfireConfig = new HangfireConfig();
        config(hangfireConfig);
        
        app.UseHangfireDashboard(
            hangfireConfig.DashboardPath,
            new DashboardOptions
            {
                DashboardTitle = hangfireConfig.Title,
                Authorization = new[]
                {
                    new HangfireCustomBasicAuthenticationFilter
                    {
                        User = hangfireConfig.Authentication.UserName,
                        Pass = hangfireConfig.Authentication.Password
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