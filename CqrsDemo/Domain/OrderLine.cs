using System;

namespace CqrsDemo.Domain
{
    [Serializable]
    public class OrderLine
    {
        public int Quantity { get; set; }
        public string ProductName { get; set; }

        public OrderLine(int quantity, string productName)
        {
            Quantity = quantity;
            ProductName = productName;
        }
    }
}