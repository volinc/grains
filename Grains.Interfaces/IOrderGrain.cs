namespace Grains.Interfaces;

public interface IOrderGrain : IGrainWithStringKey
{
    Task SetCreatedAsync();
    Task StartSearchAsync();
    Task StopSearchAsync();
    Task<long> GetSearchValueAsync();

    Task AcceptAsync();

    Task StopProcessAsync();
}