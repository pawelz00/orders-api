namespace OrdersApi.Application.DTO.Order
{
    public class OrderUpdate
    {
        public string? CustomerName { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Status { get; set; } = "Pending";
        public List<OrderItemCreate>? Products { get; set; } = [];
    }
}