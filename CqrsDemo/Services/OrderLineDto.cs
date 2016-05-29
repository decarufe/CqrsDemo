namespace CqrsDemo.Services
{
    public class OrderLineDto
    {
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public override string ToString()
        {
            return string.Format("{0,5:D}, {1}", Quantity, ProductName);
        }
    }
}