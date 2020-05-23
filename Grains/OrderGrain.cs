using System;
using System.Threading.Tasks;
using Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Grains
{
    public class OrderGrain : Grain<OrderState>, IOrder
    {
        private readonly IGrainFactory grainFactory;
        private readonly ILogger<OrderGrain> logger;
        private Guid key;

        public OrderGrain(IGrainFactory grainFactory, ILogger<OrderGrain> logger)
        {
            this.grainFactory = grainFactory;
            this.logger = logger;
        }

        public async Task<Guid> CreateAsync()
        {
            State.Created = true;
            await WriteStateAsync();
            logger.LogInformation($"Order {key} created");
            return key;
        }

        public async Task StartSearchAsync()
        {
            ThrowIfNotExists();
            var search = grainFactory.GetGrain<ISearchGrain>(key);
            await search.StartAsync(this, BuildSearchParameters());
            logger.LogInformation($"Order {key} searching started");
        }

        private static SearchParameters BuildSearchParameters()
        {
            return new SearchParameters(endAt: DateTimeOffset.Now.AddMinutes(4));
        }

        public async Task StopSearchAsync()
        {
            ThrowIfNotExists();
            var search = grainFactory.GetGrain<ISearchGrain>(key);
            await search.StopAsync();
            logger.LogInformation($"Order {key} searching stopped");
        }

        public Task AcceptAsync()
        {
            ThrowIfNotExists();
            logger.LogInformation($"Order {key} accepted");
            return Task.CompletedTask;
        }

        private void ThrowIfNotExists()
        {
            if (!State.Created)
                throw new InvalidOperationException($"Order {key} does not exist");
        }

        public override Task OnActivateAsync()
        {
            key = this.GetPrimaryKey();

            logger.LogInformation($"Order {key} activated");
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            logger.LogInformation($"Order {key} deactivated");
            return base.OnDeactivateAsync();
        }
    }
}