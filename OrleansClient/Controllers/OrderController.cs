namespace OrleansClient.Controllers;

[Route("orders")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IClusterClient _clusterClient;

    public OrderController(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    [HttpPost]
    public Task<Guid> CreateAsync()
    {
        var order = _clusterClient.GetGrain<IOrderGrain>(Guid.NewGuid());
        return order.CreateAsync();
    }

    [HttpPost("{id}/confirm")]
    public async Task ConfirmAsync(Guid id)
    {
        var order = _clusterClient.GetGrain<IOrderGrain>(id);
        await order.StartSearchAsync();
    }

    [HttpPost("{id}/cancel")]
    public async Task CancelAsync(Guid id)
    {
        var order = _clusterClient.GetGrain<IOrderGrain>(id);
        await order.StopSearchAsync();
    }

    [HttpPost("{id}/accept")]
    public async Task AcceptAsync(Guid id)
    {
        var order = _clusterClient.GetGrain<IOrderGrain>(id);
        await order.StopSearchAsync();
    }

    [HttpGet("{id}/search-value")]
    public async Task<long> GetSearchValueAsync(Guid id)
    {
        var order = _clusterClient.GetGrain<IOrderGrain>(id);
        return await order.GetSearchValueAsync();
    }

    [HttpPost("{id}/stop-process")]
    public Task StopProcessAsync(Guid id)
    {
        var order = _clusterClient.GetGrain<IOrderGrain>(id);
        order.StopProcessAsync().Ignore();
        return Task.CompletedTask;
    }

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