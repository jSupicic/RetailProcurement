using Retail.Domain.Entities;

public class QuarterlyPlan
{
    public int Id { get; set; }
    public int Year { get; set; }
    public int Quarter { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual ICollection<QuarterlyPlanSupplier> Suppliers { get; set; } = new List<QuarterlyPlanSupplier>();

}
