namespace OrleansClient.Controllers;

[Route("customers")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly IClusterClient _clusterClient;

    public CustomersController(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    [HttpPost("{customerId}")]
    public async Task<string> CreateAsync(string customerId)
    {
        var customer = _clusterClient.GetGrain<ICustomerGrain>(customerId);
        return await customer.GetNameAsync();
    }
}