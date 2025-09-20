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
    public class SupplierStoreItemsController : ControllerBase
    {
        private readonly ISupplierStoreItemService _supplierStoreItemService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public SupplierStoreItemsController(ISupplierStoreItemService supplierStoreItemService, IHubContext<NotificationHub> hubContext)
        {
            _supplierStoreItemService = supplierStoreItemService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Get all supplier-store item associations.
        /// </summary>
        /// <returns>Returns a list of supplier-store item DTOs.</returns>
        /// <response code="200">Returns the list of associations.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SupplierStoreItemDto>), 200)]
        public async Task<ActionResult<IEnumerable<SupplierStoreItemDto>>> GetAll()
        {
            var result = await _supplierStoreItemService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Create a new supplier-store item association.
        /// </summary>
        /// <param name="dto">The supplier-store item creation data.</param>
        /// <returns>Returns the newly created association.</returns>
        /// <response code="200">Association created successfully.</response>
        /// <response code="400">If creation failed.</response>
        [HttpPost]
        [ProducesResponseType(typeof(SupplierStoreItemDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<SupplierStoreItemDto>> Create(SupplierStoreItemCreateDto dto)
        {
            var created = await _supplierStoreItemService.CreateAsync(dto);

            if (created == null) return BadRequest();

            await _hubContext.Clients.All.SendAsync("SupplierStoreItemCreated", created);

            return Ok(created);
        }

        /// <summary>
        /// Delete a supplier-store item association by supplier ID and store item ID.
        /// </summary>
        /// <param name="supplierId">The supplier ID.</param>
        /// <param name="storeItemId">The store item ID.</param>
        /// <returns>No content if deletion was successful.</returns>
        /// <response code="204">Association deleted successfully.</response>
        /// <response code="404">If the association was not found.</response>
        [HttpDelete("{supplierId}/{storeItemId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int supplierId, int storeItemId)
        {
            var success = await _supplierStoreItemService.DeleteAsync(supplierId, storeItemId);

            if (!success) return NotFound();

            await _hubContext.Clients.All.SendAsync("SupplierStoreItemDeleted", new { SupplierId = supplierId, StoreItemId = storeItemId });

            return NoContent();
        }
    }
}
