namespace orders_api.DTO.Order
{
    public class OrderItemCreate
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public OrderItemCreate(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
