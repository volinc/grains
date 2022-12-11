using OrleansClient;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

await Host.CreateDefaultBuilder(args)
    .UseOrleansClient((ctx, clientBuilder) =>
    {
        var connectionString = ctx.Configuration.GetConnectionString("Clustering")
            ?? "Server=sql;Database=grains;Username=postgres;Password=pass;";

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
    .RunConsoleAsync();