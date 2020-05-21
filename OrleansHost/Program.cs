namespace Silo
{
    using Grains;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Orleans;
    using Orleans.Configuration;
    using Orleans.Hosting;
    using System.Net;

    public static class Program
    {
        private static readonly string invariant = "System.Data.SqlClient";

        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .ConfigureLogging((context, builder) => { builder.AddConsole(); })
                .UseOrleans((context, builder) =>
                {
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        //builder//.UseLocalhostClustering()
                        //.UseInMemoryReminderService();                                                                
                    }
                    
                    var connectionString = context.Configuration.GetConnectionString("Clustering");
                    builder
                        .UseAdoNetClustering(options =>
                        {
                            options.Invariant = invariant;
                            options.ConnectionString = connectionString;                             
                        })                        
                        .UseAdoNetReminderService(options =>
                        {
                            options.Invariant = invariant;
                            options.ConnectionString = connectionString;
                        })
                        .Configure<EndpointOptions>(options =>
                        {
                            options.AdvertisedIPAddress = Dns.GetHostAddresses(Dns.GetHostName())[0];
                            options.SiloPort = 11111;
                            options.GatewayPort = 30000;
                        })
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "Grains";
                        })                        
                        .ConfigureApplicationParts(parts =>
                        {
                            parts.AddApplicationPart(typeof(OrderGrain).Assembly).WithReferences();
                        })
                        .UseDashboard(options => { options.Port = 10000; });
                });
    }
}