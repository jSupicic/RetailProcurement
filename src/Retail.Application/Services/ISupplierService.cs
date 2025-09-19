using Retail.Application.DTOs;

namespace Retail.Application.Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierDto>> GetAllAsync();
        Task<SupplierDto?> GetByIdAsync(int id);
        Task<SupplierDto> CreateAsync(SupplierCreateDto dto);
        Task<SupplierDto?> UpdateAsync(int id, SupplierUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
