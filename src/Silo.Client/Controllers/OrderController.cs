namespace TestWebAPI.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Grains.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Orleans;
    
    [Route("orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IClusterClient clusterClient;

        public OrderController(IClusterClient clusterClient)
        {
            this.clusterClient = clusterClient;
        }

        [HttpPost]
        public Task<Guid> CreateAsync()
        {
            var order = clusterClient.GetGrain<IOrder>(Guid.NewGuid());
            return Task.FromResult(order.GetPrimaryKey());
        }

        [HttpPost("{id}/confirm")]
        public async Task ConfirmAsync(Guid id)
        {
            var order = clusterClient.GetGrain<IOrder>(id);
            await order.StartSearchAsync();            
        }

        [HttpPost("{id}/cancel")]
        public async Task CancelAsync(Guid id)
        {
            var order = clusterClient.GetGrain<IOrder>(id);
            await order.StopSearchAsync();
        }
    }
}