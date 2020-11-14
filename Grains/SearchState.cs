using Grains.Interfaces;

namespace Grains
{
    public class SearchState
    {
        public IOrderGrain Order { get; set; }

        public SearchParameters Parameters { get; set; }

        public bool IsStarted { get; set; }
    }
}