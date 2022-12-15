AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var hostBuilder = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, siloBuilder) =>
    {
        var connectionString = context.Configuration.GetConnectionString("Clustering");

        siloBuilder.Configure<ClusterOptions>(options =>
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
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        });
    });

await hostBuilder.RunConsoleAsync();
