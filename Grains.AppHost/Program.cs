var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("grains-redis")
    //.WithLifetime(ContainerLifetime.Persistent)
    .WithRedisCommander(options =>
    {
        options.WithHostPort(7390);
    });

var orleans = builder.AddOrleans("grains-cluster")
    .WithClustering(redis)
    .WithGrainStorage("grains", redis)
    .WithReminders(redis);

var silo = builder.AddProject<Projects.Grains_Silo>("grains-silo")
    .WithReference(orleans)
    .WaitFor(redis)
    .WithReplicas(3);
    
builder.AddProject<Projects.Grains_Api>("grains-api")
    .WithReference(orleans.AsClient())
    .WaitFor(silo)
    .WithExternalHttpEndpoints();

await using var app = builder.Build();
await app.RunAsync();
