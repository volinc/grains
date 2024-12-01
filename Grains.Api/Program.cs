using Grains.Shared;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddKeyedRedisClient("grains-redis");
builder.UseOrleansClient();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

await using var app = builder.Build();
app.UseCors(cp =>
{
    cp.AllowAnyMethod();
    cp.AllowAnyOrigin();
    cp.AllowAnyHeader();
});
app.MapControllers();
app.MapOpenApi();
app.MapScalarApiReference();
await app.RunAsync();
