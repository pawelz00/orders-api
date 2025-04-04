using orders_api.DTO.Product;

namespace orders_api.DTO.Order
{
    public class ProductResponseWithQuantity
    {
        public ProductResponse Product { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderResponse
    {
        public string CustomerName { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public List<ProductResponseWithQuantity> products { get; set; } = new List<ProductResponseWithQuantity>();
    }
}
