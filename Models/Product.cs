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
        
        [Required]
        public decimal Price { get; set; }

        // Simple category in string format for demonstration purposes
        public string Category { get; set; }

        // Navigation property for related OrderItems

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public Product()
        {
            OrderItems = new HashSet<OrderItem>();
            Category = "General";
        }
    }
}
