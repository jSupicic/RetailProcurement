using Retail.Domain;
using Retail.Domain.Entities;

namespace Retail.Domain.Entities
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        // Relationships
        public ICollection<SupplierStoreItem> SupplierStoreItems { get; set; } = new List<SupplierStoreItem>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<QuarterlyPlanSupplier> QuarterlyPlanSuppliers { get; set; } = new List<QuarterlyPlanSupplier>();
    }

}