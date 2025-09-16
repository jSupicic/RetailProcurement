using Retail.Application.DTOs;

namespace Retail.Application.Services
{
    public interface IStatisticsService
    {
        Task<bool> CreateQuarterlyPlanAsync(QuarterlyPlanCreateDto dto);
        Task<SupplierBestOfferDto?> GetBestOfferAsync(int storeItemId);
        Task<List<SupplierDto>?> GetCurrentQuarterPlanAsync();
        Task<SupplierStatisticDto?> GetSupplierStatisticsAsync(int supplierId);
    }
}