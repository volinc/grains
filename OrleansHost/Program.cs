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
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "devCluster";
                            options.ServiceId = "devService";                            
                        })                        
                        .AddAdoNetGrainStorage("OrleansStorage", options =>
                        {
                            options.Invariant = invariant;
                            options.ConnectionString = connectionString;
                            options.UseJsonFormat = true;
                        })
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
                        .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
                        .ConfigureApplicationParts(parts =>
                        {
                            parts.AddApplicationPart(typeof(OrderGrain).Assembly).WithReferences();
                        })
                        //.UseDashboard(options => { options.Port = 10000; });
                });
    }
}