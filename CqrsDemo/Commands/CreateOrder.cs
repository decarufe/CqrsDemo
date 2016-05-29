using System;

namespace CqrsDemo.Commands
{
    public class CreateOrder : DomainCommand
    {
        public CreateOrder(Guid id) : base(id)
        {
        }
    }
}