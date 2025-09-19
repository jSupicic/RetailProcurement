using AutoMapper;
using Retail.Application.DTOs;
using Retail.Domain.Entities;
using Retail.Infrastructure.Repositories;

namespace Retail.Application.Services
{
    public class SupplierStoreItemService : ISupplierStoreItemService
    {
        private readonly IRepository<SupplierStoreItem> _supplierStoreItemRepository;
        private readonly IRepository<StoreItem> _storeItemRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IMapper _mapper;

        public SupplierStoreItemService(
            IRepository<SupplierStoreItem> supplierStoreItemRepository,
            IRepository<StoreItem> storeItemRepository,
            IRepository<Supplier> supplierRepository,
            IMapper mapper)
        {
            _supplierStoreItemRepository = supplierStoreItemRepository;
            _storeItemRepository = storeItemRepository;
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SupplierStoreItemDto>> GetAllAsync()
        {
            var entities = await _supplierStoreItemRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SupplierStoreItemDto>>(entities);
        }

        public async Task<SupplierStoreItemDto?> CreateAsync(SupplierStoreItemCreateDto dto)
        {
            var existingRelation = _supplierStoreItemRepository
                .FindAsync(ssi => ssi.SupplierId == dto.SupplierId && ssi.StoreItemId == dto.StoreItemId)
                .Result
                .FirstOrDefault();

            if (existingRelation is not null) throw new Exception("The supplier already offers this item.");

            var existingSupplier = await _supplierRepository.GetByIdAsync(dto.SupplierId);
            if (existingSupplier is null) throw new Exception("This supplier does not exist");

            var existingStoreItem = await _storeItemRepository .GetByIdAsync(dto.StoreItemId);
            if (existingStoreItem is null) throw new Exception("This store item does not exist");

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
