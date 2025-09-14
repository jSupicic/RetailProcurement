namespace Retail.Domain.Entities
{
    public class StoreItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        // Relationships
        public virtual ICollection<SupplierStoreItem> SupplierStoreItems { get; set; } = new List<SupplierStoreItem>();
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }

}