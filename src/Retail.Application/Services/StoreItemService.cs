using Retail.Application.DTOs;
using Retail.Domain.Entities;
using Retail.Infrastructure.Repositories;

namespace Retail.Application.Services
{
    public class StoreItemService : IStoreItemService
    {
        private readonly IRepository<StoreItem> _storeItemRepository;

        public StoreItemService(IRepository<StoreItem> storeItemRepository)
        {
            _storeItemRepository = storeItemRepository;
        }

        public async Task<IEnumerable<StoreItemDto>> GetAllAsync()
        {
            var items = await _storeItemRepository.GetAllAsync();
            return items.Select(item => new StoreItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                StockQuantity = item.StockQuantity,
                Suppliers = item.SupplierStoreItems.Select(ssi => new SupplierDto
                {
                    Id = ssi.SupplierId,
                    Name = ssi.Supplier.Name,
                    SupplierPrice = ssi.SupplierPrice
                }).ToList()
            });
        }

        public async Task<StoreItemDto?> GetByIdAsync(int id)
        {
            var item = await _storeItemRepository.GetByIdAsync(id);
            if (item == null) return null;

            return new StoreItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                StockQuantity = item.StockQuantity,
                Suppliers = item.SupplierStoreItems.Select(ssi => new SupplierDto
                {
                    Id = ssi.SupplierId,
                    Name = ssi.Supplier.Name,
                    SupplierPrice = ssi.SupplierPrice
                }).ToList()
            };
        }

        public async Task<StoreItemDto> CreateAsync(StoreItemCreateDto dto)
        {
            var entity = new StoreItem
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price ?? 0,
                StockQuantity = dto.StockQuantity ?? 0
            };

            await _storeItemRepository.AddAsync(entity);
            await _storeItemRepository.SaveChangesAsync();

            return new StoreItemDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                StockQuantity = entity.StockQuantity
            };
        }

        public async Task<bool> UpdateAsync(int id, StoreItemUpdateDto dto)
        {
            var entity = await _storeItemRepository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.Name = dto.Name ?? entity.Name;
            entity.Description = dto.Description ?? entity.Description;
            entity.Price = dto.Price ?? entity.Price;
            entity.StockQuantity = dto.StockQuantity ?? entity.StockQuantity;

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
