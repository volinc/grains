var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureLogging((context, logging) =>
{
    if (context.HostingEnvironment.IsDevelopment())
        logging.AddDebug();

    logging.AddConsole();
});

builder.ConfigureServices(services =>
{
    // https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-11-avoiding-downtime-in-rolling-deployments-by-blocking-sigterm/
    services.Configure<HostOptions>(options => { options.ShutdownTimeout = TimeSpan.FromSeconds(45); });
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.UseOrleans((context, siloBuilder) =>
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
});

builder.Build().Run();