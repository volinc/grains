using Microsoft.Extensions.Hosting;
using Orleans.Serialization;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

using var host = new HostBuilder()
    .ConfigureHostConfiguration(configurationBuilder =>
    {
        configurationBuilder.AddEnvironmentVariables();
    })
    .UseOrleans((context, silBuilder) =>
    {
        var connectionString = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_Clustering");
        //var connectionString = context.Configuration.GetValue<string>("CUSTOMCONNSTR_Clustering");
        silBuilder
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = Constants.ClusterId;
                options.ServiceId = Constants.ServiceId;
            })
            .AddAdoNetGrainStorageAsDefault(options =>
            {
                options.Invariant = Constants.Invariant;
                options.ConnectionString = connectionString;
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
    })
    .Build();

await host.RunAsync();