using AutoMapper;
using Retail.Application.DTOs;
using Retail.Domain.Entities;
using Retail.Infrastructure.Repositories;

namespace Retail.Application.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IMapper _mapper;

        public SupplierService(IRepository<Supplier> supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllAsync()
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
        }

        public async Task<SupplierDto?> GetByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            return supplier == null ? null : _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<SupplierDto> CreateAsync(SupplierCreateDto dto)
        {
            var entity = _mapper.Map<Supplier>(dto);
            await _supplierRepository.AddAsync(entity);
            await _supplierRepository.SaveChangesAsync();

            return _mapper.Map<SupplierDto>(entity);
        }

        public async Task<bool> UpdateAsync(int id, SupplierUpdateDto dto)
        {
            var entity = await _supplierRepository.GetByIdAsync(id);
            if (entity == null) return false;

            _mapper.Map(dto, entity);

            await _supplierRepository.UpdateAsync(entity);
            await _supplierRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _supplierRepository.GetByIdAsync(id);
            if (entity == null) return false;

            await _supplierRepository.DeleteAsync(entity);
            await _supplierRepository.SaveChangesAsync();

            return true;
        }
    }
}
