using Orleans.Hosting;
using OrleansClient;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
Thread.Sleep(5000);

var host = new HostBuilder()
    .ConfigureHostConfiguration(configurationBuilder =>
    {
        configurationBuilder.AddEnvironmentVariables();
    })
    .UseOrleansClient((ctx, clientBuilder) =>
    {
        var connectionString = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_Clustering");
        //var connectionString = context.Configuration.GetValue<string>("CUSTOMCONNSTR_Clustering");
        clientBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = Constants.ClusterId;
            options.ServiceId = Constants.ServiceId;
        })
        .UseAdoNetClustering(options =>
        {
            options.Invariant = Constants.Invariant;
            options.ConnectionString = connectionString;
        });
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
            if (ctx.HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
