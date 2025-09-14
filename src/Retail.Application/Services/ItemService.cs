using Retail.Application.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Retail.Application.Pagination;

namespace Retail.Application.Services
{
    public class ItemService : IItemService
    {
        private readonly IMapper _mapper;

        public ItemService(IMapper mapper)
        {
            _mapper = mapper;
        }

        Task<StoreItemDto> IItemService.CreateAsync(StoreItemCreateDto dto)
        {
            throw new NotImplementedException();
        }

        Task<bool> IItemService.DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<StoreItemDto?> IItemService.GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<PagedList<StoreItemDto>> IItemService.GetPagedAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        Task<bool> IItemService.UpdateAsync(Guid id, StoreItemUpdateDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
