using System;
using System.Threading.Tasks;
using Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Grains
{
    using Microsoft.Extensions.Hosting;
    using Orleans.Runtime;

    public class OrderGrain : Grain<OrderState>, IOrderGrain
    {
        private readonly IGrainFactory grainFactory;
        private readonly ILogger<OrderGrain> logger;
        private readonly IHostApplicationLifetime appLifetime;
        private Guid key;

        public OrderGrain(IGrainFactory grainFactory, 
                          ILogger<OrderGrain> logger, 
                          IHostApplicationLifetime appLifetime)
        {
            this.grainFactory = grainFactory;
            this.logger = logger;
            this.appLifetime = appLifetime;
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

        public Task CheckExceptionSerializationAsync()
        {
            throw new EntityNotFoundException(typeof(IOrderGrain), "A");
        }

        private void ThrowIfNotExists()
        {
            if (!State.Created)
                throw new InvalidOperationException($"Order {key} does not exist");
        }

        public override Task OnActivateAsync()
        {
            appLifetime.ApplicationStopping.Register(() =>
            {
                WriteStateAsync().ConfigureAwait(false).GetAwaiter();
            });

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