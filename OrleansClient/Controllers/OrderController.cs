using System;
using System.Threading.Tasks;
using Grains.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace OrleansClient.Controllers
{
    using System.Diagnostics;

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
            var order = clusterClient.GetGrain<IOrderGrain>(Guid.NewGuid());
            return order.CreateAsync();
        }

        // (customer) confirm the order to start running automatic search
        [HttpPost("{id}/confirm")]
        public async Task ConfirmAsync(Guid id)
        {
            var order = clusterClient.GetGrain<IOrderGrain>(id);
            await order.StartSearchAsync();
        }

        // (customer) cancel the order to stop automatic search
        [HttpPost("{id}/cancel")]
        public async Task CancelAsync(Guid id)
        {
            var order = clusterClient.GetGrain<IOrderGrain>(id);
            await order.StopSearchAsync();
        }

        // (executor) accept the order
        [HttpPost("{id}/accept")]
        public async Task AcceptAsync(Guid id)
        {
            var order = clusterClient.GetGrain<IOrderGrain>(id);
            await order.StopSearchAsync();
        }

        [HttpPost("{id}/check")]
        public async Task CheckExceptionSerializationAsync(Guid id)
        {
            try
            {
                var order = clusterClient.GetGrain<IOrderGrain>(id);
                await order.CheckExceptionSerializationAsync();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                throw;
            }
        }
    }
}