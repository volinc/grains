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
    public Task<string> CreateAsync()
    {
        var order = _clusterClient.GetGrain<IOrderGrain>(Guid.NewGuid().ToString("N"));
        return order.CreateAsync();
    }

    [HttpPost("{id}/confirm")]
    public async Task ConfirmAsync(string id)
    {
        var order = _clusterClient.GetGrain<IOrderGrain>(id);
        await order.StartSearchAsync();
    }

    [HttpPost("{id}/cancel")]
    public async Task CancelAsync(string id)
    {
        var order = _clusterClient.GetGrain<IOrderGrain>(id);
        await order.StopSearchAsync();
    }

    [HttpPost("{id}/accept")]
    public async Task AcceptAsync(string id)
    {
        var order = _clusterClient.GetGrain<IOrderGrain>(id);
        await order.StopSearchAsync();
    }

    [HttpGet("{id}/search-value")]
    public async Task<long> GetSearchValueAsync(string id)
    {
        var order = _clusterClient.GetGrain<IOrderGrain>(id);
        return await order.GetSearchValueAsync();
    }

    [HttpPost("{id}/stop-process")]
    public Task StopProcessAsync(string id)
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