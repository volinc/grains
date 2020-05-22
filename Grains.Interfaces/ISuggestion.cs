using System.Threading.Tasks;
using Orleans;

namespace Grains.Interfaces
{
    public interface ISuggestion : IGrainWithGuidKey
    {
        Task AcceptAsync();

        Task DeclineAsync();
    }
}