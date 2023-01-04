using Grains.Interfaces;
using Orleans.Configuration;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

try
{ 
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        })
        .Build();

    await Task.Delay(15000);
    await host.RunAsync();
}
catch (Exception exception)
{
    Console.WriteLine(exception);
    throw;
}
