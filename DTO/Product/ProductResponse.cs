namespace orders_api.DTO.Product
{
    public class ProductResponse
    {
        public ProductResponse(Models.Product product)
        {
            Name = product.Name;
            Price = product.Price;
            Description = product.Description;
            Category = product.Category;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }
}