namespace CoHosted.Controllers;

[Route("customers")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IClusterClient _clusterClient;

    public OrdersController(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    [HttpPost("{customerId}/orders")]
    public async Task CreateOrderAsync(string customerId)
    {
        try
        {
            var customer = _clusterClient.GetGrain<ICustomerGrain>(customerId);
            await customer.CreateOrderAsync();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    [HttpGet("{customerId}/orders")]
    public async Task<List<string>> GetActiveOrdersAsync(string customerId)
    {
        var customer = _clusterClient.GetGrain<ICustomerGrain>(customerId);
        return await customer.GetActiveOrders();
    }

    [HttpPost("{customerId}/orders/{orderId}/start-search")]
    public async Task ConfirmAsync(string customerId, string orderId)
    {
        var order = _clusterClient.GetGrain<ICustomerGrain>(customerId);
        await order.StartSearchAsync(orderId);
    }

    [HttpPost("{customerId}/orders/{orderId}/stop-search")]
    public async Task AcceptAsync(string customerId, string orderId)
    {
        var order = _clusterClient.GetGrain<ICustomerGrain>(customerId);
        await order.StopSearchAsync(orderId);
    }

    [HttpGet("{customerId}/orders/{orderId}/search-value")]
    public async Task<long> GetSearchValueAsync(string customerId, string orderId)
    {
        var order = _clusterClient.GetGrain<ICustomerGrain>(customerId);
        return await order.GetSearchValueAsync(orderId);
    }

    //[HttpPost("{id}/cancel")]
    //public async Task CancelAsync(string id)
    //{
    //    var order = _clusterClient.GetGrain<IOrderGrain>(id);
    //    await order.StopSearchAsync();
    //}

    //[HttpPost("{id}/stop-process")]
    //public Task StopProcessAsync(string id)
    //{
    //    var order = _clusterClient.GetGrain<IOrderGrain>(id);
    //    order.StopProcessAsync().Ignore();
    //    return Task.CompletedTask;
    //}

    //[HttpPost("{id}/check")]
    //public async Task CheckExceptionSerializationAsync(Guid id)
    //{
    //    try
    //    {
    //        var order = clusterClient.GetGrain<IOrderGrain>(id);
    //        await order.CheckExceptionSerializationAsync();
    //    }
    //    catch (Exception exception)
    //    {
    //        Debug.WriteLine(exception);
    //        throw;
    //    }
    //}
}