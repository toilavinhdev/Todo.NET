using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddEndpointsApiExplorer().AddSwaggerGen();

var app = builder.Build();
app.UseSwagger().UseSwaggerUI();
app.MapGet("/", () => "Hello World!");

app.MapPost("/redis",
    async () =>
    {
    });

app.Run();