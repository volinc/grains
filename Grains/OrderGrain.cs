using System;
using System.Threading.Tasks;
using Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Microsoft.Extensions.Hosting;
using Orleans.Runtime;

namespace Grains
{
    public class OrderGrain : Grain, IOrderGrain
    {
        private readonly IPersistentState<OrderState> _order;
        private readonly IGrainFactory _grainFactory;
        private readonly ILogger<OrderGrain> _logger;
        private readonly IHostApplicationLifetime _hostAppLifetime;
        private Guid _key;

        private OrderState State => _order.State;

        public OrderGrain(
            [PersistentState(nameof(OrderGrain))] IPersistentState<OrderState> order,
            IGrainFactory grainFactory, 
            ILogger<OrderGrain> logger,
            IHostApplicationLifetime hostAppLifetime)
        {
            _order = order;
            _grainFactory = grainFactory;
            _logger = logger;
            _hostAppLifetime = hostAppLifetime;                        
        }

        public async Task<Guid> CreateAsync()
        {
            State.Created = true;
            await _order.WriteStateAsync();
            _logger.LogInformation($"Order {_key} created");
            return _key;
        }

        public async Task StartSearchAsync()
        {
            ThrowIfNotExists();
            var search = _grainFactory.GetGrain<ISearchGrain>(_key);
            await search.StartAsync(this, BuildSearchParameters());
            _logger.LogInformation($"Order {_key} searching started");
        }

        private static SearchParameters BuildSearchParameters()
        {
            return new SearchParameters(endAt: DateTimeOffset.Now.AddMinutes(4));
        }

        public async Task StopSearchAsync()
        {
            ThrowIfNotExists();
            var search = _grainFactory.GetGrain<ISearchGrain>(_key);
            await search.StopAsync();
            _logger.LogInformation($"Order {_key} searching stopped");
        }

        public Task AcceptAsync()
        {
            ThrowIfNotExists();
            _logger.LogInformation($"Order {_key} accepted");
            return Task.CompletedTask;
        }

        private void ThrowIfNotExists()
        {
            if (!State.Created)
                throw new InvalidOperationException($"Order {_key} does not exist");
        }

        public override async Task OnActivateAsync()
        {            
            _key = this.GetPrimaryKey();            
            await base.OnActivateAsync();
            _hostAppLifetime.ApplicationStopping.Register(() =>
            {
                _order.WriteStateAsync().ConfigureAwait(false).GetAwaiter();
            });
            _logger.LogInformation($"Order {_key} activated");            
        }

        public override async Task OnDeactivateAsync()
        {
            await base.OnDeactivateAsync();
            _logger.LogInformation($"Order {_key} deactivated");
        }
        
        public Task StopProcessAsync()
        {            
            _hostAppLifetime.StopApplication();
            return Task.CompletedTask;
        }

        public async Task<long> GetSearchValueAsync()
        {
            ThrowIfNotExists();
            var search = _grainFactory.GetGrain<ISearchGrain>(_key);
            return await search.GetValueAsync();
        }

        [Serializable]
        public class OrderState
        {
            public bool Created { get; set; }
        }
    }
}