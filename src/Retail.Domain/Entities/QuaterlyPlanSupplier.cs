using Retail.Domain;
using Retail.Domain.Entities;

public class QuarterlyPlanSupplier
{
    public int PlanId { get; set; }
    public int SupplierId { get; set; }

    // Navigation
    public virtual QuarterlyPlan QuarterlyPlan { get; set; } = null!;
    public virtual Supplier Supplier { get; set; } = null!;
}
