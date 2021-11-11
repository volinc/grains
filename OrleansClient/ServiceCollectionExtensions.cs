using Grains.Interfaces;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using OrleansClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClusterClient(this IServiceCollection services, string connectionString)
    {
        var clusterClient = new ClientBuilder()
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
            .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IOrderGrain).Assembly))
            .Build();
            
        services.AddSingleton(clusterClient);

        services.AddSingleton<ClusterClientHostedService>();
        services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<ClusterClientHostedService>());

        return services;
    }
}
