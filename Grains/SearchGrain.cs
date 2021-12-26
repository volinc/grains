namespace Grains;

public class SearchGrain : Grain, ISearchGrain, IRemindable
{
    private const string ReminderName = "search";
    private readonly IHostApplicationLifetime _hostAppLifetime;
    private readonly ILogger _logger;

    private readonly IPersistentState<SearchState> _search;
    private Guid _key;

    public SearchGrain(
        [PersistentState(nameof(SearchGrain))] IPersistentState<SearchState> search,
        ILogger<SearchGrain> logger,
        IHostApplicationLifetime hostAppLifetime)
    {
        _search = search;
        _logger = logger;
        _hostAppLifetime = hostAppLifetime;
    }

    private SearchState State => _search.State;

    public override async Task OnActivateAsync()
    {
        _key = this.GetPrimaryKey();
        await base.OnActivateAsync();
        _hostAppLifetime.ApplicationStopping.Register(() =>
        {
            _logger.LogInformation("Handle shutdown");
            _search.WriteStateAsync().ConfigureAwait(false).GetAwaiter();
        });
        _logger.LogInformation($"### Search {_key} activated");
    }

    public override async Task OnDeactivateAsync()
    {
        await base.OnDeactivateAsync();
        _logger.LogInformation($"### Search {_key} deactivated");
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

        await RegisterOrUpdateReminder(ReminderName, TimeSpan.Zero, TimeSpan.FromMinutes(1));
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
            var reminder = await GetReminder(ReminderName);
            await UnregisterReminder(reminder);
            return;
        }

        Loop(TimeSpan.Zero);
    }

    protected void Loop(TimeSpan dueTime)
    {
        _logger.LogInformation($"### Search 'Timer' start waiting {DateTime.Now} handled");

        RegisterTimer(_ =>
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