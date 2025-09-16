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

    public class SupplierStatisticDto
    {
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public int TotalItemsSold { get; set; }
        public decimal TotalEarnings { get; set; }
    }

    public class SupplierBestOfferDto
    {
        public required string SupplierName { get; set; }
        public required string StoreItemName { get; set; }
        public required decimal StoreItemPrice { get; set; }
    }
}
