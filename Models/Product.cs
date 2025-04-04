using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace orders_api.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Precision(18, 2)]
        [Required]
        public decimal Price { get; set; }

        // Simple category in string format for demonstration purposes
        public string Category { get; set; } = "General";

        // Navigation property for related OrderItems

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public Product()
        {
            OrderItems = new List<OrderItem>();
        }
    }
}
