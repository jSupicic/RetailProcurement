namespace Retail.Application.DTOs
{
    public class QuarterlyPlanDto
    {
        public required int Year { get; set; }
        public required int Quarter { get; set; }
        public required int[] SupplierIds { get; set; }
    }
    public class QuarterlyPlanCreateDto : QuarterlyPlanDto { }

}
