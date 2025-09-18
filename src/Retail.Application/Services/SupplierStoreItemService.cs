using AutoMapper;
using Retail.Application.DTOs;
using Retail.Domain.Entities;
using Retail.Infrastructure.Repositories;

namespace Retail.Application.Services
{
    public class SupplierStoreItemService : ISupplierStoreItemService
    {
        private readonly IRepository<SupplierStoreItem> _supplierStoreItemRepository;
        private readonly IMapper _mapper;

        public SupplierStoreItemService(
            IRepository<SupplierStoreItem> supplierStoreItemRepository,
            IMapper mapper)
        {
            _supplierStoreItemRepository = supplierStoreItemRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SupplierStoreItemDto>> GetAllAsync()
        {
            var entities = await _supplierStoreItemRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SupplierStoreItemDto>>(entities);
        }

        public async Task<SupplierStoreItemDto?> CreateAsync(SupplierStoreItemCreateDto dto)
        {
            var existing = _supplierStoreItemRepository
                .FindAsync(ssi => ssi.SupplierId == dto.SupplierId && ssi.StoreItemId == dto.StoreItemId)
                .Result
                .FirstOrDefault();

            if (existing is not null) throw new Exception("The supplier already offers this item.");

            var entity = _mapper.Map<SupplierStoreItem>(dto);

            await _supplierStoreItemRepository.AddAsync(entity);
            await _supplierStoreItemRepository.SaveChangesAsync();

            return _mapper.Map<SupplierStoreItemDto>(entity);
        }

        public async Task<bool> DeleteAsync(int supplierId, int storeItemId)
        {
            var entities = await _supplierStoreItemRepository.FindAsync(x => x.SupplierId == supplierId && x.StoreItemId == storeItemId);
            var entity = entities.FirstOrDefault();
            if (entity == null) return false;

            await _supplierStoreItemRepository.DeleteAsync(entity);
            await _supplierStoreItemRepository.SaveChangesAsync();

            return true;
        }
    }
}
