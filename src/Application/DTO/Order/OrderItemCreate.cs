using System.ComponentModel.DataAnnotations;

namespace OrdersApi.Application.DTO.Order
{
    public class OrderItemCreate
    {
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
        public OrderItemCreate(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
