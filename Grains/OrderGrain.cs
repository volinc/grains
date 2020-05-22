using System;
using System.Threading.Tasks;
using Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Grains
{
    public class OrderGrain : Grain<OrderData>, IOrder
    {
        private readonly ILogger<OrderGrain> logger;
        private Guid id;

        public OrderGrain(ILogger<OrderGrain> logger)
        {
            this.logger = logger;
        }

        public Task StartSearchAsync()
        {
            logger.LogDebug($"Order {id} searching started");
            return Task.CompletedTask;
        }

        public Task StopSearchAsync()
        {
            logger.LogDebug($"Order {id} searching stopped");
            return Task.CompletedTask;
        }

        public override Task OnActivateAsync()
        {
            id = this.GetPrimaryKey();

            logger.LogDebug($"Order {id} activated");
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            logger.LogDebug($"Order {id} deactivated");
            return base.OnDeactivateAsync();
        }
    }
}