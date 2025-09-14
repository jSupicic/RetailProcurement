using Retail.Domain;
using Retail.Domain.Entities;

public class SupplierStoreItem
{
    public int SupplierId { get; set; }
    public int StoreItemId { get; set; }
    public decimal SupplierPrice { get; set; }

    // Navigation
    public virtual Supplier Supplier { get; set; } = null!;
    public virtual StoreItem StoreItem { get; set; } = null!;
}
