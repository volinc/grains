var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Clustering");
builder.Services.AddClusterClient(connectionString);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
    builder.Logging.AddDebug();

builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();