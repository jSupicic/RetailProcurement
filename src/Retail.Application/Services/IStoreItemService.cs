using Retail.Application.DTOs;

namespace Retail.Application.Services
{
    public interface IStoreItemService
    {
        Task<StoreItemDto> CreateAsync(StoreItemCreateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<StoreItemDto>> GetAllAsync();
        Task<StoreItemDto?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, StoreItemUpdateDto dto);
    }
}