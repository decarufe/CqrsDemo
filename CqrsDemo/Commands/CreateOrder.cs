using System;

namespace CqrsDemo.Commands
{
    public class CreateOrder : Command
    {
        public CreateOrder(Guid id) : base(id)
        {
        }
    }
}