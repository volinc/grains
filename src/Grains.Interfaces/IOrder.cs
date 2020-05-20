namespace Grains.Interfaces
{
    using Orleans;
    using System.Threading.Tasks;

    public interface IOrder : IGrainWithGuidKey
    {
        Task StartSearchAsync();

        Task StopSearchAsync();
    }
}
