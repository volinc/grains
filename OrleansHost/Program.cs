using Grains;
using Grains.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureLogging((context, builder) =>
{
    if (context.HostingEnvironment.IsDevelopment()) 
        builder.AddDebug();

    builder.AddConsole();
});

builder.ConfigureServices(services =>
{
    services.Configure<HostOptions>(options =>
    {
        options.ShutdownTimeout = TimeSpan.FromSeconds(5);
    });
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.UseOrleans((context, builder) =>
{    
    var connectionString = context.Configuration.GetConnectionString("Clustering");
    builder
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = Constants.ClusterId;
            options.ServiceId = Constants.ServiceId;
        })
        .AddAdoNetGrainStorageAsDefault(options =>
        {
            options.Invariant = Constants.Invariant;
            options.ConnectionString = connectionString;
            options.UseJsonFormat = true;
        })
        .UseAdoNetClustering(options =>
        {
            options.Invariant = Constants.Invariant;
            options.ConnectionString = connectionString;
        })
        .UseAdoNetReminderService(options =>
        {
            options.Invariant = Constants.Invariant;
            options.ConnectionString = connectionString;
        })
        .ConfigureEndpoints(11111, 30000)
        .ConfigureApplicationParts(parts =>
        {
            parts.AddApplicationPart(typeof(OrderGrain).Assembly).WithReferences();
        });
});

builder.Build().Run();