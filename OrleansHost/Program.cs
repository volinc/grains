AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var hostBuilder = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, siloBuilder) =>
    {
        var connectionString = context.Configuration.GetConnectionString("Clustering");
        var instanceId = context.Configuration.GetValue<int>("InstanceId");

        siloBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = Constants.ClusterId;
            options.ServiceId = Constants.ServiceId;
        })
        .UseAdoNetClustering(options =>
        {
            options.Invariant = Constants.Invariant;
            options.ConnectionString = connectionString;
        })
        .AddAdoNetGrainStorageAsDefault(options =>
        {
            options.Invariant = Constants.Invariant;
            options.ConnectionString = connectionString;
        })
        .UseAdoNetReminderService(options =>
        {
            options.Invariant = Constants.Invariant;
            options.ConnectionString = connectionString;
        })
        .ConfigureEndpoints(siloPort: 11111 + instanceId, gatewayPort: 30000 + instanceId)
        .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Debug).AddConsole());
    });

await hostBuilder.RunConsoleAsync();