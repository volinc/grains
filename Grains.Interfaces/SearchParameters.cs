using System;

namespace Grains.Interfaces
{
    public class SearchParameters
    {
        public SearchParameters(DateTimeOffset endAt)
        {
            EndAt = endAt;
        }

        public DateTimeOffset EndAt { get; }
    }
}
