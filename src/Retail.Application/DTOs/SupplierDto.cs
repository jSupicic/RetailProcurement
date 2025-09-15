namespace Retail.Application.DTOs
{

    public class SupplierDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public List<StoreItemDto>? StoreItems { get; set; }
    }

    public class SupplierCreateDto
    {
        public required string Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public class SupplierUpdateDto : SupplierCreateDto { }

}
