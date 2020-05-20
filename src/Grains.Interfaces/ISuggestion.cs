namespace Grains.Interfaces
{
    using Orleans;
    using System.Threading.Tasks;

    public interface ISuggestion : IGrainWithGuidKey
    {
        Task AcceptAsync();

        Task DeclineAsync();
    }
}
