namespace OrleansClient
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

            services.AddSingleton(sp =>
            {
                return new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = Constants.ClusterId;
                    options.ServiceId = Constants.ServiceId;
                })
                .UseAdoNetClustering(options =>
                {
                    options.Invariant = Constants.Invariant;
                    options.ConnectionString = Configuration.GetConnectionString("Clustering");
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IOrder).Assembly))
                .ConfigureLogging(builder =>
                {
                    var loggerProvider = sp.GetRequiredService<ILoggerProvider>();
                    builder.AddProvider(loggerProvider);
                })
                .Build();
            });
            services.AddSingleton<ClusterClientHostedService>();
            services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<ClusterClientHostedService>());
            
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