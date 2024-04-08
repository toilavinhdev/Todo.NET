namespace Todo.NET.Hangfire;

public class HangfireAttribute(string cron, string description) : Attribute
{
    public string Cron { get; set; } = cron;

    public string Description { get; set; } = description;
}