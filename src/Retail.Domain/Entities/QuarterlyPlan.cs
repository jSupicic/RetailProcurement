public class QuarterlyPlan
{
    public int Id { get; set; }
    public int Year { get; set; }
    public int Quarter { get; set; }
    public required int[] SupplierIds { get; set; }
    public DateTime CreatedAt { get; set; }

}
