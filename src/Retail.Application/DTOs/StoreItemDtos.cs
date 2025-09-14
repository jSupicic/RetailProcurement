namespace Retail.Application.DTOs
{
    public class StoreItemDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public List<SupplierDto> Suppliers { get; set; } = new();
    }

    public class StoreItemCreateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class StoreItemUpdateDto : StoreItemCreateDto { }
}
