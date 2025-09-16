using Microsoft.AspNetCore.Mvc;
using Retail.Application.DTOs;
using Retail.Application.Services;

namespace Retail.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierStoreItemsController : ControllerBase
    {
        private readonly ISupplierStoreItemService _supplierStoreItemService;

        public SupplierStoreItemsController(ISupplierStoreItemService supplierStoreItemService)
        {
            _supplierStoreItemService = supplierStoreItemService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierStoreItemDto>>> GetAll()
        {
            var result = await _supplierStoreItemService.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<SupplierStoreItemDto>> Create(SupplierStoreItemCreateDto dto)
        {
            var created = await _supplierStoreItemService.CreateAsync(dto);

            if (created == null) return BadRequest();
            return Ok(created);
        }

        [HttpDelete("{supplierId}/{storeItemId}")]
        public async Task<IActionResult> Delete(int supplierId, int storeItemId)
        {
            var success = await _supplierStoreItemService.DeleteAsync(supplierId, storeItemId);

            if (!success) return NotFound();
            return NoContent();
        }
    }
}
