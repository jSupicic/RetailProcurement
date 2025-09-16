using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Retail.Application.DTOs;
using Retail.Domain.Entities;
using Retail.Infrastructure.Repositories;

namespace Retail.Application.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<SupplierStoreItem> _supplierStoreItemRepository;
        private readonly IRepository<QuarterlyPlan> _quarterlyPlanRepository;
        private readonly IMapper _mapper;

        public StatisticsService(
            IRepository<Supplier> supplierRepository,
            IRepository<SupplierStoreItem> supplierStoreItemRepository,
            IRepository<QuarterlyPlan> quarterlyPlanRepository,
            IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _supplierStoreItemRepository = supplierStoreItemRepository;
            _quarterlyPlanRepository = quarterlyPlanRepository;
            _mapper = mapper;
        }

        public async Task<SupplierStatisticDto?> GetSupplierStatisticsAsync(int supplierId)
        {
            var supplier = await _supplierRepository.GetByIdAsync(supplierId);
            
            if (supplier == null) return null;
            return _mapper.Map<SupplierStatisticDto>(supplier);
        }

        public async Task<SupplierBestOfferDto?> GetBestOfferAsync(int storeItemId)
        {
            var allItems = await _supplierStoreItemRepository.GetAllAsync();
            var supplierStoreItem = allItems
                .Where(x => x.StoreItemId == storeItemId)
                .OrderBy(x => x.SupplierPrice)
                .FirstOrDefault();

            if (supplierStoreItem == null) return null;
            return _mapper.Map<SupplierBestOfferDto>(supplierStoreItem);
        }

        public async Task<bool> CreateQuarterlyPlanAsync(QuarterlyPlanCreateDto dto)
        {
            var allSupplierIds = _supplierRepository
                .GetAllAsync().Result
                .Select(x => x.Id);

            //filter non-existent and double suppliers
            dto.SupplierIds = dto.SupplierIds.Where(id => allSupplierIds.Contains(id))
                .Distinct()
                .Order()
                .ToArray();

            var existing = _quarterlyPlanRepository
                .FindAsync(x => x.Quarter == dto.Quarter && x.Year == dto.Year).Result
                .FirstOrDefault();

            if (existing == null)
            {
                var plan = _mapper.Map<QuarterlyPlan>(dto);
                await _quarterlyPlanRepository.AddAsync(plan);
            }
            else
            {
                var entity = await _quarterlyPlanRepository.GetByIdAsync(existing.Id);
                _mapper.Map(dto, entity);
            }
            
            await _quarterlyPlanRepository.SaveChangesAsync();

            return true;
        }
        public async Task<List<SupplierDto>?> GetCurrentQuarterPlanAsync()
        {
            var now = DateTime.UtcNow;
            int quarter = (now.Month - 1) / 3 + 1;

            var plans = await _quarterlyPlanRepository.GetAllAsync();
            var currentPlan = plans.FirstOrDefault(p => p.Year == now.Year && p.Quarter == quarter);

            if (currentPlan == null) return null;

            var supliers = await _supplierRepository
                .FindAsync(s => currentPlan.SupplierIds.Contains(s.Id));


            return _mapper.Map<List<SupplierDto>>(supliers);
        }
    }
}
