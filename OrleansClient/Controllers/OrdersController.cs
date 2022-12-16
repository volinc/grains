namespace OrleansClient.Controllers;

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
        var customer = _clusterClient.GetGrain<ICustomerGrain>(customerId);
        await customer.CreateOrderAsync();
    }

    [HttpGet("{customerId}/orders")]
    public async Task<List<string>> GetActiveOrdersAsync(string customerId)
    {
        var customer = _clusterClient.GetGrain<ICustomerGrain>(customerId);
        return await customer.GetActiveOrders();
    }

    //[HttpPost("{id}/confirm")]
    //public async Task ConfirmAsync(string id)
    //{
    //    var order = _clusterClient.GetGrain<IOrderGrain>(id);
    //    await order.StartSearchAsync();
    //}

    //[HttpPost("{id}/cancel")]
    //public async Task CancelAsync(string id)
    //{
    //    var order = _clusterClient.GetGrain<IOrderGrain>(id);
    //    await order.StopSearchAsync();
    //}

    //[HttpPost("{id}/accept")]
    //public async Task AcceptAsync(string id)
    //{
    //    var order = _clusterClient.GetGrain<IOrderGrain>(id);
    //    await order.StopSearchAsync();
    //}

    //[HttpGet("{id}/search-value")]
    //public async Task<long> GetSearchValueAsync(string id)
    //{
    //    var order = _clusterClient.GetGrain<IOrderGrain>(id);
    //    return await order.GetSearchValueAsync();
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