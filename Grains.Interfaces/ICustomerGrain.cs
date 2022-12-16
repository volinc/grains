namespace Grains.Interfaces;

public interface ICustomerGrain : IGrainWithStringKey
{
    Task<string> GetNameAsync();

    Task CreateOrderAsync();

    Task<List<string>> GetActiveOrders();
}

