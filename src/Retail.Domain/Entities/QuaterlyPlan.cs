using Retail.Domain;

public class QuarterlyPlan
{
    public int Id { get; set; }
    public int Year { get; set; }
    public int Quarter { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public virtual ICollection<QuarterlyPlanSupplier> QuarterlyPlanSuppliers { get; set; } = new List<QuarterlyPlanSupplier>();
}
