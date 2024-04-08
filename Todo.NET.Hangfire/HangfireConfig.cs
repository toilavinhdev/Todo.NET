namespace Todo.NET.Hangfire;

public class HangfireConfig
{
    public string Title { get; set; } = default!;

    public string? DashboardPath { get; set; }

    public HangfireAuthentication Authentication { get; set; } = default!;
}

public class HangfireAuthentication
{
    public string UserName { get; set; } = default!;

    public string Password { get; set; } = default!;
}

public class HangfireMongoConfig
{
    public string ConnectionString { get; set; } = default!;

    public string DatabaseName { get; set; } = default!;
}