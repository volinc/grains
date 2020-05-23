using Grains.Interfaces;

namespace Grains
{
    public class SearchState
    {
        public IOrder Order { get; set; }

        public SearchParameters Parameters { get; set; }

        public bool IsStarted { get; set; }
    }
}