using System;

namespace CoreLibrary.Utilities.CustomPopDamage
{
    public interface IPopDamageHandlerProvider
    {

        IPopDamageHandler Current { get; }
    }

    public class StaticPopDamageHandlerProvider : IPopDamageHandlerProvider
    {
        public IPopDamageHandler Current => EntityPopDamage.handler;
    }
}