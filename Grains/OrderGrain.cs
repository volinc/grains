using System.Threading;

namespace Grains;

public class OrderGrain : IGrainBase, IOrderGrain
{
    private readonly IGrainFactory _grainFactory;
    private readonly IHostApplicationLifetime _hostAppLifetime;
    private readonly ILogger<OrderGrain> _logger;
    private readonly IPersistentState<OrderState> _order;
    private Guid _key;

    public OrderGrain(IGrainContext grainContext,
        [PersistentState(nameof(OrderGrain))] IPersistentState<OrderState> order,
        IGrainFactory grainFactory,
        ILogger<OrderGrain> logger,
        IHostApplicationLifetime hostAppLifetime)
    {
        GrainContext = grainContext;
        _order = order;
        _grainFactory = grainFactory;
        _logger = logger;
        _hostAppLifetime = hostAppLifetime;
    }

    private OrderState State => _order.State;

    public IGrainContext GrainContext { get; }

    public async Task<Guid> CreateAsync()
    {
        State.Created = true;
        await _order.WriteStateAsync();
        _logger.LogInformation($"Order {_key} created");
        return _key;
    }

    public async Task StartSearchAsync()
    {
        ThrowIfNotExists();
        var search = _grainFactory.GetGrain<ISearchGrain>(_key);
        await search.StartAsync(this, BuildSearchParameters());
        _logger.LogInformation($"Order {_key} searching started");
    }

    public async Task StopSearchAsync()
    {
        ThrowIfNotExists();
        var search = _grainFactory.GetGrain<ISearchGrain>(_key);
        await search.StopAsync();
        _logger.LogInformation($"Order {_key} searching stopped");
    }

    public Task AcceptAsync()
    {
        ThrowIfNotExists();
        _logger.LogInformation($"Order {_key} accepted");
        return Task.CompletedTask;
    }

    public Task StopProcessAsync()
    {
        _hostAppLifetime.StopApplication();
        return Task.CompletedTask;
    }

    public async Task<long> GetSearchValueAsync()
    {
        ThrowIfNotExists();
        var search = _grainFactory.GetGrain<ISearchGrain>(_key);
        return await search.GetValueAsync();
    }

    private static SearchParameters BuildSearchParameters()
    {
        return new SearchParameters(DateTimeOffset.Now.AddMinutes(4));
    }

    private void ThrowIfNotExists()
    {
        if (!State.Created)
            throw new InvalidOperationException($"Order {_key} does not exist");
    }

    public Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _key = this.GetPrimaryKey();
        _hostAppLifetime.ApplicationStopping.Register(() =>
        {
            _order.WriteStateAsync().ConfigureAwait(false).GetAwaiter();
        });
        _logger.LogInformation($"Order {_key} activated");
        return Task.CompletedTask;
    }

    public Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Order {_key} deactivated");
        return Task.CompletedTask;
    }

    [Serializable]
    public class OrderState
    {
        public bool Created { get; set; }
    }
}