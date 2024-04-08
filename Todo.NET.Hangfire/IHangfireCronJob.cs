using Hangfire;

namespace Todo.NET.Hangfire;

[AutomaticRetry(Attempts = 0)]
public interface IHangfireCronJob
{
    Task<string> Run();
}