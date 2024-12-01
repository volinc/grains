namespace Grains.Silo.Grains;

public class SearchGrain(
    [PersistentState(nameof(SearchGrain), "grains")] IPersistentState<SearchGrainState> persistentState,
    ILogger<SearchGrain> logger,
    IHostApplicationLifetime hostAppLifetime)
    : Grain, ISearchGrain, IRemindable
{
    private const string ReminderName = "search";
    private readonly ILogger _logger = logger;

    private string? _key;

    private SearchGrainState State => persistentState.State;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _key = this.GetPrimaryKeyString();
        hostAppLifetime.ApplicationStopping.Register(() =>
        {
            _logger.LogInformation("### Search {Key} graceful shutdown", _key);

            persistentState.WriteStateAsync().ConfigureAwait(false).GetAwaiter();

            // https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-11-avoiding-downtime-in-rolling-deployments-by-blocking-sigterm/
            _logger.LogInformation("SIGTERM received, waiting for 29 seconds");
        });
        _logger.LogInformation("### Search {Key} activated", _key);
        return Task.CompletedTask;
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        _logger.LogInformation("### Search {Key} deactivated", _key);
        return Task.CompletedTask;
    }

    public async Task ReceiveReminder(string reminderName, TickStatus status)
    {
        if (State.IsExpired(DateTimeOffset.Now))
        {
            persistentState.State = SearchGrainState.CreateStopped(State);
            await persistentState.WriteStateAsync();
        }

        await RunAsync();
        _logger.LogInformation("### Search \'ReceiveReminder\' {StatusCurrentTickTime} handled", status.CurrentTickTime);
    }

    public async Task StartAsync(IOrderGrain order, SearchParameters parameters)
    {
        try
        {
            if (State.IsStarted)
                return;

            persistentState.State = SearchGrainState.CreateStarted(order, parameters);
            await this.RegisterOrUpdateReminder(ReminderName, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            await persistentState.WriteStateAsync();
            await Console.Out.WriteLineAsync($"Value = {State.Value}");
            await RunAsync();

            _logger.LogInformation("### Search \'RegisterOrUpdateReminder\' {Now} handled", DateTime.Now);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    public async Task StopAsync()
    {
        if (!State.IsStarted)
            return;

        persistentState.State = SearchGrainState.CreateStopped(State);
        await persistentState.WriteStateAsync();
    }

    public Task<long> GetValueAsync()
    {
        return Task.FromResult(State.Value);
    }

    private async Task RunAsync()
    {
        if (State.IsStarted)
        {
            Loop(TimeSpan.Zero);
            return;
        }

        var reminder = await this.GetReminder(ReminderName);
        if (reminder != null)
            await this.UnregisterReminder(reminder);
    }

    private void Loop(TimeSpan dueTime)
    {
        _logger.LogInformation("### Search \'Timer\' start waiting {Now} handled", DateTime.Now);

        this.RegisterGrainTimer<object>(_ =>
        {
            _logger.LogInformation("### Search \'Timer\' end waiting {Now} handled", DateTime.Now);

            return LoopSearchAsync(Loop);
        }, null!, dueTime, TimeSpan.FromMilliseconds(-1));
    }

    private async Task LoopSearchAsync(Action<TimeSpan> loop)
    {
        await Task.Delay(500);

        if (!State.IsStarted)
            return;

        persistentState.State = SearchGrainState.CreateNextIncremental(State);
        await persistentState.WriteStateAsync();
        await Console.Out.WriteLineAsync($"Value = {State.Value}");

        loop(TimeSpan.FromSeconds(10));
    }
}