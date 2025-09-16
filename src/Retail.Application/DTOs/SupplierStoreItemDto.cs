namespace Retail.Application.DTOs
{
    public class SupplierStoreItemDto
    {
        public int SupplierId { get; set; }
        public int StoreItemId { get; set; }
        public decimal SupplierPrice { get; set; }

        public string SupplierName { get; set; } = string.Empty;
        public string StoreItemName { get; set; } = string.Empty;
    }

    public class SupplierStoreItemCreateDto
    {
        public int SupplierId { get; set; }
        public int StoreItemId { get; set; }
        public decimal SupplierPrice { get; set; }
    }
}
