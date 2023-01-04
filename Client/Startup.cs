using Grains.Interfaces;
using Orleans;
using Orleans.Configuration;

namespace Client;

public class Startup
{
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        _env = env;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddHealthChecks();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        var clusterId = Configuration.GetValue<string>(Constants.ClusterIdKey);
        var serviceId = Configuration.GetValue<string>(Constants.ServiceIdKey);
        var connectionString = Configuration.GetConnectionString(Constants.ConnectionStringKey);

        services.AddOrleansClient(builder =>
        {
            builder.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = clusterId;
                options.ServiceId = serviceId;
            })
            .UseAdoNetClustering(options =>
            {
                options.Invariant = Constants.Invariant;
                options.ConnectionString = connectionString;
            });
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("health");
            endpoints.MapControllers();
        });
    }
}

