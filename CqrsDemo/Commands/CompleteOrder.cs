using System;

namespace CqrsDemo.Commands
{
    public class CompleteOrder : DomainCommand
    {
        public CompleteOrder(Guid id) : base(id)
        {
        }
    }
}