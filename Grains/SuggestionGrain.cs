using System;
using System.Threading.Tasks;
using Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Grains
{
    public class SuggestionGrain : Grain, ISuggestion
    {
        private readonly ILogger logger;

        public SuggestionGrain(ILogger<SuggestionGrain> logger)
        {
            this.logger = logger;
        }

        public Task AcceptAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeclineAsync()
        {
            throw new NotImplementedException();
        }
    }
}