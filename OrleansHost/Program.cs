AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var hostBuilder = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, siloBuilder) =>
    {
        var connectionString = context.Configuration.GetConnectionString("Clustering");

        siloBuilder
            .Configure<ClusterOptions>(options =>
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
            .ConfigureEndpoints(11111, 30000)
            .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Debug).AddConsole());

        //siloBuilder.Services.AddSerializer(serializerBuilder =>
        //{
        //    serializerBuilder.AddNewtonsoftJsonSerializer(
        //        isSupported: type => type.Namespace!.StartsWith("System"));
        //});
    });

await hostBuilder.RunConsoleAsync();