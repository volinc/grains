using Orleans;
using Orleans.Runtime;

namespace OrleansClient
{
    public class ClusterClientHostedService : IHostedService
    {
        private readonly IClusterClient clusterClient;
        private readonly ILogger<ClusterClientHostedService> logger;
        private readonly Random random;

        public ClusterClientHostedService(IClusterClient clusterClient,
            ILogger<ClusterClientHostedService> logger)
        {
            this.clusterClient = clusterClient;
            this.logger = logger;
            random = new Random();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var attempt = 0;
            const int maxAttempts = 100;
            
            clusterClient.Connect(async error =>
            {
                if (cancellationToken.IsCancellationRequested)                
                    return false;                

                if (++attempt < maxAttempts)
                {                    
                    try
                    {                        
                        var delay = TimeSpan.FromSeconds(random.Next(3,7));
                        await Task.Delay(delay, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return false;
                    }

                    return true;
                }

                logger.LogError(error, $"Failed to connect to Orleans cluster on attempt {attempt} of {maxAttempts}.");
                return false;

            }).Ignore();

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await clusterClient.Close();
            }
            catch (OrleansException error)
            {
                logger.LogWarning(error,
                    "Error while gracefully disconnecting from Orleans cluster. Will ignore and continue to shutdown.");
            }
        }
    }
}