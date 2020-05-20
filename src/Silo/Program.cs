namespace Silo
{
    using System.Threading.Tasks;
    using Grains;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Orleans;
    using Orleans.Configuration;
    using Orleans.Hosting;

    public static class Program
    {
        public static Task Main(string[] args) =>
            Host.CreateDefaultBuilder()
                .UseOrleans((context, builder) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("STORAGE_ACCOUNT");
                    
                    builder
                        .UseLocalhostClustering()
                        .UseInMemoryReminderService()
                        //.UseAzureStorageClustering(c => c.ConnectionString = connectionString)
                        //.ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "HelloWorldApp";
                        })
                        //.Configure<EndpointOptions>(options =>
                        //    options.AdvertisedIPAddress = Dns.GetHostAddresses("silo")[0])
                        .ConfigureApplicationParts(parts =>
                            parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
                        .AddMemoryGrainStorage("ArchiveStorage")
                        .UseDashboard(options => { options.Port = 10000; });
                })
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .ConfigureLogging(builder => { builder.AddConsole(); })
                .RunConsoleAsync();
    }
}