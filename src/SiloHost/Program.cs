namespace SiloHost
{
    using System.Net;
    using System.Threading.Tasks;
    using HelloWorld.Grains;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Orleans;
    using Orleans.Configuration;
    using Orleans.Hosting;

    public class Program
    {
        private static string AzureStorageConnectionString = "<replace_me>";

        public static Task Main(string[] args) =>
            new HostBuilder()
                .UseOrleans(builder =>
                {
                    builder
                        .UseAzureStorageClustering(c => c.ConnectionString = AzureStorageConnectionString)
                        .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "HelloWorldApp";
                        })
                        .Configure<EndpointOptions>(options =>
                            options.AdvertisedIPAddress = Dns.GetHostAddresses("silo")[0])
                        .ConfigureApplicationParts(parts =>
                            parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
                        .AddMemoryGrainStorage("ArchiveStorage");
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