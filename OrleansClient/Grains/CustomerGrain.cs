namespace Grains;

public class CustomerGrain : Grain, ICustomerGrain
{
    public CustomerGrain(IGrainContext grainContext)
	{
        GrainContext = grainContext;
    }

    public IGrainContext GrainContext { get; }

    public Task<string> GetNameAsync()
    {
        var key = this.GetPrimaryKeyString();
        return Task.FromResult(key);
    }

    public async Task CreateOrderAsync()
    {
        var orderId = Guid.NewGuid().ToString("N").ToLowerInvariant();
        var orderGrain = GrainFactory.GetGrain<IOrderGrain>(orderId);
        await orderGrain.SetCreatedAsync();
        activeOrders.Add(orderId);
    }

    public Task<List<string>> GetActiveOrders()
    {
        return Task.FromResult(activeOrders);
    }

    public async Task StartSearchAsync(string orderId)
    {
        if (!activeOrders.Contains(orderId))
            throw new EntityNotFoundException();

        var orderGrain = GrainFactory.GetGrain<IOrderGrain>(orderId);
        await orderGrain.StartSearchAsync();
    }

    public async Task StopSearchAsync(string orderId)
    {
        if (!activeOrders.Contains(orderId))
            throw new EntityNotFoundException();

        var orderGrain = GrainFactory.GetGrain<IOrderGrain>(orderId);
        await orderGrain.StopSearchAsync();
    }

    public async Task<long> GetSearchValueAsync(string orderId)
    {
        if (!activeOrders.Contains(orderId))
            throw new EntityNotFoundException();

        var orderGrain = GrainFactory.GetGrain<IOrderGrain>(orderId);
        return await orderGrain.GetSearchValueAsync();
    }

    private List<string> activeOrders = new List<string>();
}

