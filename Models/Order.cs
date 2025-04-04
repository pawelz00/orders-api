using System.ComponentModel.DataAnnotations;

namespace orders_api.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        // For demonstration purposes, we are using simple string properties
        [Required]
        public string CustomerName { get; set; }
        
        [Required]
        public string ShippingAddress { get; set; }
        
        [Required]
        public DateTime OrderDate { get; set; }
        
        public string Status { get; set; } // "Pending", "Shipped", "Delivered"

        // Navigation property for related OrderItems
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
            OrderDate = DateTime.UtcNow;
            Status = "Pending"; // Default
        }
    }
}
