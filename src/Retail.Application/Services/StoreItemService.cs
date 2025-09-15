using AutoMapper;
using Retail.Application.DTOs;
using Retail.Domain.Entities;
using Retail.Infrastructure.Repositories;

namespace Retail.Application.Services
{
    public class StoreItemService : IStoreItemService
    {
        private readonly IRepository<StoreItem> _storeItemRepository;
        private readonly IMapper _mapper;

        public StoreItemService(
            IRepository<StoreItem> storeItemRepository,
            IMapper mapper)
        {
            _storeItemRepository = storeItemRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StoreItemDto>> GetAllAsync()
        {
            var items = await _storeItemRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<StoreItemDto>>(items);
        }

        public async Task<StoreItemDto?> GetByIdAsync(int id)
        {
            var item = await _storeItemRepository.GetByIdAsync(id);

            return item == null ? null : _mapper.Map<StoreItemDto>(item);
        }

        public async Task<StoreItemDto> CreateAsync(StoreItemCreateDto dto)
        {
            var entity = _mapper.Map<StoreItem>(dto);

            await _storeItemRepository.AddAsync(entity);
            await _storeItemRepository.SaveChangesAsync();

            return _mapper.Map<StoreItemDto>(entity);
        }

        public async Task<bool> UpdateAsync(int id, StoreItemUpdateDto dto)
        {
            var entity = await _storeItemRepository.GetByIdAsync(id);
            if (entity == null) return false;

            _mapper.Map(dto, entity);
            
            await _storeItemRepository.UpdateAsync(entity);
            await _storeItemRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _storeItemRepository.GetByIdAsync(id);
            if (entity == null) return false;

            await _storeItemRepository.DeleteAsync(entity);
            await _storeItemRepository.SaveChangesAsync();

            return true;
        }
    }
}
