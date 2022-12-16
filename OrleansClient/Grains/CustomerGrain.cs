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

    public Task CreateOrderAsync()
    {
        var orderId = Guid.NewGuid().ToString("N").ToLowerInvariant();
        GrainFactory.GetGrain<IOrderGrain>(orderId);
        activeOrders.Add(orderId);
        return Task.CompletedTask;
    }

    public Task<List<string>> GetActiveOrders()
    {
        return Task.FromResult(activeOrders);
    }

    private List<string> activeOrders = new List<string>();
}

