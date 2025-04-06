
namespace OrdersApi.Application.DTO.Order
{
    public class OrderCreate
    {
        public string CustomerName { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; 
        public List<OrderItemCreate> Products { get; set; } = new List<OrderItemCreate>();
    }
}