
using Microsoft.CodeAnalysis;

namespace University_Cafeteria_Task8.Models
{
    public class RequestItem
    {
        public int Id { get; set; }

        public int SupplyRequestId { get; set; }

        public SupplyRequest SupplyRequest { get; set; } = null!;

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
