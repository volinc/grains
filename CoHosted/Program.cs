﻿using Grains.Interfaces;
using Orleans.Configuration;
using Orleans.Runtime;
using Orleans.Runtime.Placement;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

try
{
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            services.AddSingletonNamedService<PlacementStrategy, FixedPlacementStrategy>(nameof(FixedPlacementStrategy));
            services.AddSingletonKeyedService<Type, IPlacementDirector, FixedPlacementDirector>(typeof(FixedPlacementStrategy));
        })
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
            .ConfigureEndpoints(hostname: "cohosted", siloPort: 11112, gatewayPort: 30002, listenOnAnyHostAddress: true)
            .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Information).AddConsole());

            if (isKubernetesHosting)
                siloBuilder.UseKubernetesHosting();
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureServices(services =>
            {
                services.AddControllers();
                services.AddHealthChecks();
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen();
            })
            .Configure((ctx, app) =>
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapHealthChecks("health");
                    endpoints.MapControllers();
                });
            });
        })
        .Build();

    await host.RunAsync();
}
catch (Exception exception)
{
    Console.WriteLine(exception);
    throw;
}
