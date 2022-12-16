namespace Grains.Interfaces;

public interface ICustomerGrain : IGrainWithStringKey
{
    Task<string> GetNameAsync();

    Task CreateOrderAsync();
    Task<List<string>> GetActiveOrders();
    Task StartSearchAsync(string orderId);
    Task StopSearchAsync(string orderId);
    Task<long> GetSearchValueAsync(string orderId);
}

