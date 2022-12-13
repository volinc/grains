using System.Threading.Tasks;
using Orleans;

namespace Grains.Interfaces;

public interface ISearchGrain : IGrainWithStringKey
{
    Task StartAsync(IOrderGrain order, SearchParameters parameters);
    Task StopAsync();
    Task<long> GetValueAsync();
}