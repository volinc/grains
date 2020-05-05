﻿namespace HelloWorld.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Orleans;

    /// <summary>
    ///     Orleans grain communication interface that will save all greetings
    /// </summary>
    public interface IHelloArchive : IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);

        Task<IEnumerable<string>> GetGreetings();
    }
}