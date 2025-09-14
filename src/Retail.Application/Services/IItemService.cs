using Retail.Application.DTOs;
using Retail.Application.Pagination;

namespace Retail.Application.Services;

public interface IItemService
{
    Task<PagedList<StoreItemDto>> GetPagedAsync(int page, int pageSize);
    Task<StoreItemDto?> GetByIdAsync(Guid id);
    Task<StoreItemDto> CreateAsync(StoreItemCreateDto dto);
    Task<bool> UpdateAsync(Guid id, StoreItemUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
}
