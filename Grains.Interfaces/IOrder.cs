using System.Threading.Tasks;
using Orleans;

namespace Grains.Interfaces
{
    public interface IOrder : IGrainWithGuidKey
    {
        Task StartSearchAsync();

        Task StopSearchAsync();
    }
}