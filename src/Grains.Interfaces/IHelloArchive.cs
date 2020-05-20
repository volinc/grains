namespace Grains.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Orleans;

    public interface IHelloArchive : IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);

        Task<IEnumerable<string>> GetGreetings();
    }
}