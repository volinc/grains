namespace HelloWorld.Interfaces
{
    using System.Threading.Tasks;
    using Orleans;

    /// <summary>
    ///     Orleans grain communication interface IHello
    /// </summary>
    public interface IHello : IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}