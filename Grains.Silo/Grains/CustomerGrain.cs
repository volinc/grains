namespace Grains.Silo.Grains;

public class CustomerGrain : Grain, ICustomerGrain
{
    private readonly List<string> _activeOrders = [];
    
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
        _activeOrders.Add(orderId);
    }

    public Task<List<string>> GetActiveOrders()
    {
        return Task.FromResult(_activeOrders);
    }

    public async Task StartSearchAsync(string orderId)
    {
        if (!_activeOrders.Contains(orderId))
            throw new EntityNotFoundException();

        var orderGrain = GrainFactory.GetGrain<IOrderGrain>(orderId);
        await orderGrain.StartSearchAsync();
    }

    public async Task StopSearchAsync(string orderId)
    {
        if (!_activeOrders.Contains(orderId))
            throw new EntityNotFoundException();

        var orderGrain = GrainFactory.GetGrain<IOrderGrain>(orderId);
        await orderGrain.StopSearchAsync();
    }

    public async Task<long> GetSearchValueAsync(string orderId)
    {
        if (!_activeOrders.Contains(orderId))
            throw new EntityNotFoundException();

        var orderGrain = GrainFactory.GetGrain<IOrderGrain>(orderId);
        return await orderGrain.GetSearchValueAsync();
    }
}

