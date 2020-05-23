using System;
using System.Threading.Tasks;
using Orleans;

namespace Grains.Interfaces
{
    public interface IOrder : IGrainWithGuidKey
    {
        Task<Guid> CreateAsync();

        Task StartSearchAsync();

        Task StopSearchAsync();

        Task AcceptAsync();
    }
}