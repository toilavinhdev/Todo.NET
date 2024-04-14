namespace Todo.NET.Hangfire;

public class HangfireMongoOptions
{
    public string Prefix { get; set; } = default!;

    public string ConnectionString { get; set; } = default!;

    public string DatabaseName { get; set; } = default!;
}