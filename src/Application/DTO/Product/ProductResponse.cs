namespace OrdersApi.Application.DTO.Product
{
    public class ProductResponse
    {
        public ProductResponse(OrdersApi.Domain.Models.Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Price = product.Price;
            Description = product.Description;
            Category = product.Category;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }
}