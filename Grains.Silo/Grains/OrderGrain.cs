namespace Grains.Silo.Grains;

public class OrderGrain(
    [PersistentState(nameof(OrderGrain), "grains")] IPersistentState<OrderGrainState> persistentState,
    IGrainFactory grainFactory,
    ILogger<OrderGrain> logger,
    IHostApplicationLifetime hostAppLifetime)
    : Grain, IOrderGrain
{
    private string? _key;

    private OrderGrainState State => persistentState.State;

    public async Task SetCreatedAsync()
    {
        persistentState.State = OrderGrainState.Create();
        await persistentState.WriteStateAsync();
        logger.LogInformation("Order {Key} created", _key);
    }

    public async Task StartSearchAsync()
    {
        ThrowIfNotExists();
        var search = grainFactory.GetGrain<ISearchGrain>(_key);
        await search.StartAsync(this, BuildSearchParameters());
        logger.LogInformation("Order {Key} searching started", _key);
    }

    public async Task StopSearchAsync()
    {
        ThrowIfNotExists();
        var search = grainFactory.GetGrain<ISearchGrain>(_key);
        await search.StopAsync();
        logger.LogInformation("Order {Key} searching stopped", _key);
    }

    public Task AcceptAsync()
    {
        ThrowIfNotExists();
        logger.LogInformation("Order {Key} accepted", _key);
        return Task.CompletedTask;
    }

    public Task StopProcessAsync()
    {
        hostAppLifetime.StopApplication();
        return Task.CompletedTask;
    }

    public async Task<long> GetSearchValueAsync()
    {
        ThrowIfNotExists();
        var search = grainFactory.GetGrain<ISearchGrain>(_key);
        return await search.GetValueAsync();
    }

    private static SearchParameters BuildSearchParameters()
    {
        return new SearchParameters
        {
            EndAt = DateTimeOffset.Now.AddMinutes(4)
        };
    }

    private void ThrowIfNotExists()
    {
        if (!State.Created)
            throw new InvalidOperationException($"Order {_key} does not exist");
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _key = this.GetPrimaryKeyString();
        hostAppLifetime.ApplicationStopping.Register(() =>
        {
            persistentState.WriteStateAsync().ConfigureAwait(false).GetAwaiter();
        });
        logger.LogInformation("Order {Key} activated", _key);
        return Task.CompletedTask;
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        logger.LogInformation("Order {Key} deactivated", _key);
        return Task.CompletedTask;
    }
}