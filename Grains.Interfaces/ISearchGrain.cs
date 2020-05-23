using System.Threading.Tasks;
using Orleans;

namespace Grains.Interfaces
{
    public interface ISearchGrain : IGrainWithGuidKey
    {
        Task StartAsync(IOrder order, SearchParameters parameters);

        Task StopAsync();
    }
}