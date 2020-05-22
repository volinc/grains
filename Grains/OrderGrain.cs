namespace Grains
{
    using Grains.Interfaces;    
    using System.Threading.Tasks;
    using Orleans;
    using System;
    using Microsoft.Extensions.Logging;
    
    public class OrderGrain : Grain<OrderData>, IOrder
    {        
        private readonly ILogger<OrderGrain> logger;
        private Guid id;

        public OrderGrain(ILogger<OrderGrain> logger)
        {            
            this.logger = logger;            
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
    }

    public class OrderData
    {
        
    }
}