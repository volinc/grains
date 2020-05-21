namespace TestWebAPI
{
    using Grains.Interfaces;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;
    using Orleans;
    using Orleans.Configuration;
    using Orleans.Hosting;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddLogging(builder => builder.AddConsole());
            services.AddSingleton(sp =>
            {                
                var clientBuilder = new ClientBuilder();
                
                clientBuilder.UseAdoNetClustering(options =>
                {
                    options.Invariant = "System.Data.SqlClient";                    
                    options.ConnectionString = Configuration.GetConnectionString("Clustering");
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "Grains";
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IOrder).Assembly))
                .ConfigureLogging(builder =>
                {
                    var loggerProvider = sp.GetRequiredService<ILoggerProvider>();
                    builder.AddProvider(loggerProvider);
                });

                return clientBuilder.Build();
            });
            services.AddSingleton<ClusterClientHostedService>();
            services.AddSingleton<IHostedService>(_ => _.GetService<ClusterClientHostedService>());
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API V1");
            });

            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}