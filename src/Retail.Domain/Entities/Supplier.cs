using Retail.Domain;
using Retail.Domain.Entities;

namespace Retail.Domain.Entities
{
    public class Supplier
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        // Relationships
        public virtual ICollection<SupplierStoreItem>? SupplierStoreItems { get; set; }
        public virtual ICollection<Sale>? Sales { get; set; }
        public virtual ICollection<QuarterlyPlanSupplier>? QuarterlyPlanSuppliers { get; set; }
    }

}