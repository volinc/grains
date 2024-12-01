using Grains.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Grains.Api.Controllers;

[Route("customers")]
[ApiController]
public class OrdersController(IClusterClient clusterClient) : ControllerBase
{
    [HttpPost("{customerId}/orders")]
    public async Task CreateOrderAsync(string customerId)
    {
        var customer = clusterClient.GetGrain<ICustomerGrain>(customerId);
        await customer.CreateOrderAsync();
    }

    [HttpGet("{customerId}/orders")]
    public async Task<List<string>> GetActiveOrdersAsync(string customerId)
    {
        var customer = clusterClient.GetGrain<ICustomerGrain>(customerId);
        return await customer.GetActiveOrders();
    }

    [HttpPost("{customerId}/orders/{orderId}/start-search")]
    public async Task ConfirmAsync(string customerId, string orderId)
    {
        var order = clusterClient.GetGrain<ICustomerGrain>(customerId);
        await order.StartSearchAsync(orderId);
    }

    [HttpPost("{customerId}/orders/{orderId}/stop-search")]
    public async Task AcceptAsync(string customerId, string orderId)
    {
        var order = clusterClient.GetGrain<ICustomerGrain>(customerId);
        await order.StopSearchAsync(orderId);
    }

    [HttpGet("{customerId}/orders/{orderId}/search-value")]
    public async Task<long> GetSearchValueAsync(string customerId, string orderId)
    {
        var order = clusterClient.GetGrain<ICustomerGrain>(customerId);
        return await order.GetSearchValueAsync(orderId);
    }
}