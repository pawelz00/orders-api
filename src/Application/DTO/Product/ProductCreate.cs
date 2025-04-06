using System.ComponentModel.DataAnnotations;

namespace OrdersApi.Application.DTO.Product
{
    public class ProductCreate
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [Range(0.01, (double)decimal.MaxValue)]
        public decimal Price { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [MaxLength(100)]
        public string Category { get; set; }    
    }
}
