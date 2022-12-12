using Microsoft.Extensions.Hosting;
using Orleans.Serialization;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

using var host = new HostBuilder()
    .ConfigureLogging((context, logging) =>
    {
        if (context.HostingEnvironment.IsDevelopment())
            logging.AddDebug();

        logging.AddConsole();
    })
    .ConfigureServices(services =>
    {
        // https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-11-avoiding-downtime-in-rolling-deployments-by-blocking-sigterm/
        services.Configure<HostOptions>(options =>
        {
            options.ShutdownTimeout = TimeSpan.FromSeconds(45);
        });
    })
    .UseOrleans((context, siloBuilder) =>
    {
        var connectionString = context.Configuration.GetConnectionString("Clustering");
        siloBuilder
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = Constants.ClusterId;
                options.ServiceId = Constants.ServiceId;
            })
            .AddAdoNetGrainStorageAsDefault(options =>
            {
                options.Invariant = Constants.Invariant;
                options.ConnectionString = connectionString;
                //options.UseJsonFormat = true;
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
            .ConfigureEndpoints(11111, 30000);

        siloBuilder.Services.AddSerializer(serializerBuilder =>
        {
            serializerBuilder.AddNewtonsoftJsonSerializer(
                isSupported: type => type.Namespace!.StartsWith("Grains.Interfaces"));
        });
    })
    .Build();

await host.RunAsync();