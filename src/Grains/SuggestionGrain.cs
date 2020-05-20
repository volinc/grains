namespace Grains
{
    using System.Threading.Tasks;
    using Grains.Interfaces;
    using Microsoft.Extensions.Logging;
    using Orleans;

    public class SuggestionGrain : Grain, ISuggestion
    {
        private readonly ILogger logger;

        public SuggestionGrain(ILogger<SuggestionGrain> logger)
        {
            this.logger = logger;
        }

        public Task AcceptAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task DeclineAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}