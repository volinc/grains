using Grains.Shared;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddKeyedRedisClient("grains-redis");
builder.UseOrleans();

await using var app = builder.Build();
await app.RunAsync();
