using Grains.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Grains.Api.Controllers;

[Route("customers")]
[ApiController]
public class CustomersController(IClusterClient clusterClient) : ControllerBase
{
    [HttpPost("{customerId}")]
    public async Task<string> CreateAsync(string customerId)
    {
        var customer = clusterClient.GetGrain<ICustomerGrain>(customerId);
        return await customer.GetNameAsync();
    }
}