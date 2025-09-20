using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Retail.Api.Hubs;
using Retail.Application.DTOs;
using Retail.Application.Services;

namespace Retail.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public SuppliersController(ISupplierService supplierService, IHubContext<NotificationHub> hubContext)
        {
            _supplierService = supplierService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Get all suppliers.
        /// </summary>
        /// <returns>Returns a list of all suppliers.</returns>
        /// <response code="200">Returns the list of suppliers.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SupplierDto>), 200)]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAllSuppliers()
        {
            var suppliers = await _supplierService.GetAllAsync();
            return Ok(suppliers);
        }

        /// <summary>
        /// Get a supplier by ID.
        /// </summary>
        /// <param name="id">The supplier ID.</param>
        /// <returns>Returns the supplier with the specified ID.</returns>
        /// <response code="200">Returns the supplier.</response>
        /// <response code="404">If the supplier was not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SupplierDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SupplierDto>> GetSupplierById(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null) return NotFound();
            return Ok(supplier);
        }

        /// <summary>
        /// Create a new supplier.
        /// </summary>
        /// <param name="dto">The supplier creation data.</param>
        /// <returns>Returns the newly created supplier.</returns>
        /// <response code="200">Supplier created successfully.</response>
        [HttpPost]
        [ProducesResponseType(typeof(SupplierDto), 200)]
        public async Task<ActionResult<SupplierDto>> CreateSupplier(SupplierCreateDto dto)
        {
            var created = await _supplierService.CreateAsync(dto);
            await _hubContext.Clients.All.SendAsync("SupplierCreated", created);
            return Ok(created);
        }

        /// <summary>
        /// Update an existing supplier.
        /// </summary>
        /// <param name="id">The supplier ID.</param>
        /// <param name="dto">The updated supplier data.</param>
        /// <returns>Returns the updated supplier.</returns>
        /// <response code="200">Supplier updated successfully.</response>
        /// <response code="404">If the supplier was not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SupplierDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateSupplier(int id, SupplierUpdateDto dto)
        {
            var updated = await _supplierService.UpdateAsync(id, dto);
            if (updated is null) return NotFound();

            await _hubContext.Clients.All.SendAsync("SupplierUpdated", updated);
            return Ok(updated);
        }

        /// <summary>
        /// Delete a supplier by ID.
        /// </summary>
        /// <param name="id">The supplier ID.</param>
        /// <returns>No content if the deletion was successful.</returns>
        /// <response code="204">Supplier deleted successfully.</response>
        /// <response code="404">If the supplier was not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var success = await _supplierService.DeleteAsync(id);
            if (!success) return NotFound();
            await _hubContext.Clients.All.SendAsync("SupplierDeleted", id);
            return NoContent();
        }
    }
}
