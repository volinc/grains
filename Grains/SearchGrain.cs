using System.Threading;

namespace Grains;

public class SearchGrain : Grain, ISearchGrain, IRemindable
{
    private const string ReminderName = "search";
    private readonly IHostApplicationLifetime _hostAppLifetime;
    private readonly ILogger _logger;

    private readonly IPersistentState<SearchState> _search;
    private Guid _key;

    public SearchGrain(IGrainContext grainContext,
        [PersistentState(nameof(SearchGrain))] IPersistentState<SearchState> search,
        ILogger<SearchGrain> logger,
        IHostApplicationLifetime hostAppLifetime)
    {
        GrainContext = grainContext;
        _search = search;
        _logger = logger;
        _hostAppLifetime = hostAppLifetime;
    }

    private SearchState State => _search.State;

    public IGrainContext GrainContext { get; }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _key = this.GetPrimaryKey();
        _hostAppLifetime.ApplicationStopping.Register(() =>
        {
            _logger.LogInformation($"### Search {_key} graceful shutdown");

            _search.WriteStateAsync().ConfigureAwait(false).GetAwaiter();

            // https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-11-avoiding-downtime-in-rolling-deployments-by-blocking-sigterm/
            _logger.LogInformation("SIGTERM received, waiting for 30 seconds");
            Thread.Sleep(30000);
            _logger.LogInformation("Termination delay complete, continuing stopping process");
        });
        _logger.LogInformation($"### Search {_key} activated");
        return Task.CompletedTask;
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"### Search {_key} deactivated");
        return Task.CompletedTask;
    }

    public async Task ReceiveReminder(string reminderName, TickStatus status)
    {
        if (DateTimeOffset.Now > State.Parameters.EndAt)
        {
            State.IsStarted = false;
            await _search.WriteStateAsync();
        }

        await RunAsync();
        _logger.LogInformation($"### Search 'ReceiveReminder' {status.CurrentTickTime} handled");
    }

    public async Task StartAsync(IOrderGrain order, SearchParameters parameters)
    {
        if (State.IsStarted)
            return;

        State.Order = order;
        State.Parameters = parameters;
        State.IsStarted = true;

        await this.RegisterOrUpdateReminder(ReminderName, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        await _search.WriteStateAsync();

        await Console.Out.WriteLineAsync($"Value = {State.Value}");
        await RunAsync();

        _logger.LogInformation($"### Search 'RegisterOrUpdateReminder' {DateTime.Now} handled");
    }

    public async Task StopAsync()
    {
        if (!State.IsStarted)
            return;

        State.IsStarted = false;
        await _search.WriteStateAsync();
    }

    public Task<long> GetValueAsync()
    {
        return Task.FromResult(State.Value);
    }

    protected async Task RunAsync()
    {
        if (!State.IsStarted)
        {
            var reminder = await this.GetReminder(ReminderName);
            await this.UnregisterReminder(reminder);
            return;
        }

        Loop(TimeSpan.Zero);
    }

    protected void Loop(TimeSpan dueTime)
    {
        _logger.LogInformation($"### Search 'Timer' start waiting {DateTime.Now} handled");

        this.RegisterTimer(_ =>
        {
            _logger.LogInformation($"### Search 'Timer' end waiting {DateTime.Now} handled");
        
            return LoopSearchAsync(Loop);
        }, null, dueTime, TimeSpan.FromMilliseconds(-1));
    }

    private async Task LoopSearchAsync(Action<TimeSpan> loop)
    {
        await Task.Delay(500);
        State.Value += 1;
        await Console.Out.WriteLineAsync($"Value = {State.Value}");
        loop(TimeSpan.FromSeconds(10));
    }
    
    [Serializable]
    public class SearchState
    {
        public IOrderGrain Order { get; set; }
        public SearchParameters Parameters { get; set; }
        public long Value { get; set; }
        public bool IsStarted { get; set; }
    }
}