using System;

namespace CqrsDemo.Commands
{
    public class CompleteOrder : Command
    {
        public CompleteOrder(Guid id) : base(id)
        {
        }
    }
}