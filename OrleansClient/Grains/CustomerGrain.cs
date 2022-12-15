namespace Grains;

public class CustomerGrain : IGrainBase, ICustomerGrain
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
}

