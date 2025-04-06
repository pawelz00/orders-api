using OrdersApi.Application.DTO.Product;

namespace OrdersApi.Application.DTO.Order

{
    public class ProductResponseWithQuantity
    {
        public ProductResponse Product { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderResponse
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public List<ProductResponseWithQuantity> Products { get; set; } = new List<ProductResponseWithQuantity>();
    }
}
