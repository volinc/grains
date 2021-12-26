namespace OrleansClient;

public class ClusterClientHostedService : IHostedService
{
    private readonly IClusterClient _clusterClient;
    private readonly ILogger<ClusterClientHostedService> _logger;
    private readonly Random _random;

    public ClusterClientHostedService(IClusterClient clusterClient,
        ILogger<ClusterClientHostedService> logger)
    {
        _clusterClient = clusterClient;
        _logger = logger;
        _random = new Random();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var attempt = 0;
        const int maxAttempts = 100;

        _clusterClient.Connect(async error =>
        {
            if (cancellationToken.IsCancellationRequested)
                return false;

            if (++attempt < maxAttempts)
            {
                try
                {
                    var delay = TimeSpan.FromSeconds(_random.Next(3, 7));
                    await Task.Delay(delay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return false;
                }

                return true;
            }

            _logger.LogError(error, $"Failed to connect to Orleans cluster on attempt {attempt} of {maxAttempts}.");
            return false;
        }).Ignore();

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _clusterClient.Close();
        }
        catch (OrleansException error)
        {
            _logger.LogWarning(error,
                "Error while gracefully disconnecting from Orleans cluster. Will ignore and continue to shutdown.");
        }
    }
}