namespace Retail.Application.DTOs
{
    public class StoreItemDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int StockQuantity { get; set; }

        public List<FromSupplierDto>? Suppliers { get; set; }
    }

    public class StoreItemCreateDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required int? StockQuantity { get; set; }
    }

    public class StoreItemUpdateDto : StoreItemCreateDto { }

    public class FromSupplierDto
    {
        public required string Name { get; set; }
        public required decimal Price { get; set; }
    }
}
