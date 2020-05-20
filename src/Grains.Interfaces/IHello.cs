namespace Grains.Interfaces
{
    using System.Threading.Tasks;
    using Orleans;

    public interface IHello : IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}