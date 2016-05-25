using System;
using Pyxis.Cqrs.Domain.Exception;

namespace Pyxis.Cqrs.Domain.Factories
{
    internal static class AggregateFactory
    {
        public static T CreateAggregate<T>()
        {
            try
            {
                return (T) Activator.CreateInstance(typeof (T), true);
            }
            catch (MissingMethodException)
            {
                throw new MissingParameterLessConstructorException(typeof (T));
            }
        }
    }
}