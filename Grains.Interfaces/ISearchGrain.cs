using System.Threading.Tasks;
using Orleans;

namespace Grains.Interfaces
{
    public interface ISearchGrain : IGrainWithGuidKey
    {
        Task StartAsync(IOrderGrain order, SearchParameters parameters);
        Task StopAsync();
        Task<long> GetValueAsync();
    }
}