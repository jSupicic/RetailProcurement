namespace Retail.Domain.Entities;

public class QuarterlyPlanSupplier
{
    public int SupplierId { get; set; }
    public int QuarterlyPlanId { get; set; }

    // Navigation
    public virtual QuarterlyPlan QuarterlyPlan { get; set; } = null!;
    public virtual Supplier Supplier { get; set; } = null!;
}
