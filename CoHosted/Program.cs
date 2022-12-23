using Microsoft.Extensions.Hosting;
using Orleans.Serialization;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var host = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, siloBuilder) =>
    {
        var clusterId = context.Configuration.GetValue<string>(Constants.ClusterIdKey);
        var serviceId = context.Configuration.GetValue<string>(Constants.ServiceIdKey);
        var connectionString = context.Configuration.GetConnectionString(Constants.ConnectionStringKey);
        var isKubernetesHosting = context.Configuration.GetValue<bool>(Constants.IsKubernetesHostingKey);

        siloBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = clusterId;
            options.ServiceId = serviceId;
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
        .ConfigureEndpoints(siloPort: 11112, gatewayPort: 30001)
        .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Debug).AddConsole());

        if (isKubernetesHosting)
            siloBuilder.UseKubernetesHosting();
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureServices(services =>
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        });
        webBuilder.Configure((ctx, app) =>
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        });
    })
    .Build();

await host.RunAsync();