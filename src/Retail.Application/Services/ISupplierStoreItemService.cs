using Retail.Application.DTOs;

namespace Retail.Application.Services
{
    public interface ISupplierStoreItemService
    {
        Task<SupplierStoreItemDto?> CreateAsync(SupplierStoreItemCreateDto dto);
        Task<bool> DeleteAsync(int supplierId, int storeItemId);
        Task<IEnumerable<SupplierStoreItemDto>> GetAllAsync();
    }
}