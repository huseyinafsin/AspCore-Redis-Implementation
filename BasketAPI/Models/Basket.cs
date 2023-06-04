using System.Runtime.InteropServices;

namespace BasketAPI.Models
{
    public class Basket
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CustomerId { get; set; }
        public double TotalAmount { get; set; }

        public List<BasketItem> Items { get; set; }

    }
}
