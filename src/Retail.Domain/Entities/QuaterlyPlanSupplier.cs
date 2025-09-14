using Retail.Domain;
using Retail.Domain.Entities;

public class QuarterlyPlanSupplier
{
    public int PlanId { get; set; }
    public int SupplierId { get; set; }

    // Navigation
    public QuarterlyPlan QuarterlyPlan { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;
}
