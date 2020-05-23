using System;
using System.Threading.Tasks;
using Grains.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace OrleansClient.Controllers
{
    [Route("orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IClusterClient clusterClient;

        public OrderController(IClusterClient clusterClient)
        {
            this.clusterClient = clusterClient;
        }

        // (customer) create an order
        [HttpPost]
        public Task<Guid> CreateAsync()
        {
            var order = clusterClient.GetGrain<IOrder>(Guid.NewGuid());
            return order.CreateAsync();
        }

        // (customer) confirm the order to start running automatic search
        [HttpPost("{id}/confirm")]
        public async Task ConfirmAsync(Guid id)
        {
            var order = clusterClient.GetGrain<IOrder>(id);
            await order.StartSearchAsync();
        }

        // (customer) cancel the order to stop automatic search
        [HttpPost("{id}/cancel")]
        public async Task CancelAsync(Guid id)
        {
            var order = clusterClient.GetGrain<IOrder>(id);
            await order.StopSearchAsync();
        }

        // (executor) accept the order
        [HttpPost("{id}/accept")]
        public async Task AcceptAsync(Guid id)
        {
            var order = clusterClient.GetGrain<IOrder>(id);
            await order.StopSearchAsync();
        }
    }
}