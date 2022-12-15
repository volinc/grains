using System;
using System.Threading.Tasks;
using Orleans;

namespace Grains.Interfaces;

public interface ICustomerGrain : IGrainWithStringKey
{
    Task<string> GetNameAsync();
}

