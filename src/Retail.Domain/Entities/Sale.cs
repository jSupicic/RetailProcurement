using Retail.Domain;
using Retail.Domain.Entities;

public class Sale
{
    public int Id { get; set; }
    public int StoreItemId { get; set; }
    public int SupplierId { get; set; }
    public int Quantity { get; set; }
    public DateTime SaleDate { get; set; }

    // Navigation
    public virtual StoreItem StoreItem { get; set; } = null!;
    public virtual Supplier Supplier { get; set; } = null!;
}
