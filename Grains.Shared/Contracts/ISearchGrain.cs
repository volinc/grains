namespace Grains.Shared.Contracts;

public interface ISearchGrain : IGrainWithStringKey
{
    Task StartAsync(IOrderGrain order, SearchParameters parameters);
    Task StopAsync();
    Task<long> GetValueAsync();
}