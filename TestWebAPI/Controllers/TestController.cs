namespace TestWebAPI.Controllers
{
    using System.Threading.Tasks;
    using HelloWorld.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Orleans;
    using Orleans.Runtime;

    [Route("grains")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IClusterClient clusterClient;

        public TestController(IClusterClient clusterClient)
        {
            this.clusterClient = clusterClient;
        }

        [HttpGet("{id}")]
        public async Task<string> GetById(long id)
        {
            var friend = this.clusterClient.GetGrain<IHello>(id);
            var response = await friend.SayHello("Test By ID");
            return response;
        }

        [HttpGet("{id}/{great}")]
        public async Task<string> GetPersistentById(long id, string great)
        {
            var g = this.clusterClient.GetGrain<IHelloArchive>(id);
            await g.SayHello(great);

            var greetings = await g.GetGreetings();
            
            return Utils.EnumerableToString(greetings);
        }
    }
}