using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace orders_api.Models
{
    public class OrderItem
    {
        [Required]
        public int OrderId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Required]
        public int ProductId { get; set; }
        
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
