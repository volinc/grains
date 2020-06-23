using System;
using System.Threading.Tasks;
using Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace Grains
{
    public class SearchGrain : Grain<SearchState>, ISearchGrain, IRemindable
    {
        private const string ReminderName = "search";

        private readonly ILogger logger;

        public SearchGrain(ILogger<SearchGrain> logger)
        {
            this.logger = logger;
        }
        
        public async Task StartAsync(IOrder order, SearchParameters parameters)
        {
            if (State.IsStarted)
                return;
            
            State.Order = order;
            State.Parameters = parameters;
            State.IsStarted = true;
            
            await RegisterOrUpdateReminder(ReminderName, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            await WriteStateAsync();

            await RunAsync();

            logger.LogInformation($"### Search 'RegisterOrUpdateReminder' {DateTime.Now} handled");
        }
        
        public async Task StopAsync()
        {
            if (!State.IsStarted)
                return;

            State.IsStarted = false;
            await WriteStateAsync();
        }

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            if (!State.IsStarted || State.Parameters.EndAt < DateTimeOffset.Now)
            {
                var reminder = await GetReminder(reminderName);
                await UnregisterReminder(reminder);
                return;
            }

            await RunAsync();
            logger.LogInformation($"### Search 'ReceiveReminder' {status.CurrentTickTime} handled");
        }

        protected async Task RunAsync()
        {
            // UpdateSuggestionStates

            if (!State.IsStarted)
                return;

            if (State.Parameters.EndAt < DateTimeOffset.Now)
            {
                State.IsStarted = false;
                await WriteStateAsync();
                return;
            }

            // CreateOrReadSuggestions
            // SendOffers

            logger.LogInformation($"### Search 'Timer' start waiting {DateTime.Now} handled");

            RegisterTimer(stateObj =>
            {
                logger.LogInformation($"### Search 'Timer' end waiting {DateTime.Now} handled");
                return RunAsync();

            }, null, TimeSpan.FromSeconds(15), TimeSpan.FromMilliseconds(-1));
        }

        public override async Task OnActivateAsync()
        {
            logger.LogInformation($"### Search {this.GetPrimaryKey()} activated");
            await base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            logger.LogInformation($"### Search {this.GetPrimaryKey()} deactivated");
            return base.OnDeactivateAsync();
        }
    }
}