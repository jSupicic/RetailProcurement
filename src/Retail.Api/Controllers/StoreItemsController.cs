using Microsoft.AspNetCore.Mvc;
using Retail.Application.DTOs;
using Retail.Application.Services;
using Microsoft.AspNetCore.SignalR;
using Retail.Api.Hubs;

namespace Retail.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreItemsController : ControllerBase
    {
        private readonly IStoreItemService _storeItemService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public StoreItemsController(IStoreItemService storeItemService, IHubContext<NotificationHub> hubContext)
        {
            _storeItemService = storeItemService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Get all store items.
        /// </summary>
        /// <returns>Returns a list of store items.</returns>
        /// <response code="200">Returns the list of store items.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StoreItemDto>), 200)]
        public async Task<ActionResult<IEnumerable<StoreItemDto>>> GetAllStoreItems()
        {
            var items = await _storeItemService.GetAllAsync();
            return Ok(items);
        }

        /// <summary>
        /// Get a specific store item by ID.
        /// </summary>
        /// <param name="id">The store item ID.</param>
        /// <returns>Returns the store item with the specified ID.</returns>
        /// <response code="200">Returns the store item.</response>
        /// <response code="404">If the store item was not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StoreItemDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<StoreItemDto>> GetStoreItem(int id)
        {
            var storeItem = await _storeItemService.GetByIdAsync(id);
            if (storeItem == null) return NotFound();
            return Ok(storeItem);
        }

        /// <summary>
        /// Create a new store item.
        /// </summary>
        /// <param name="dto">The store item creation data.</param>
        /// <returns>Returns the newly created store item.</returns>
        /// <response code="200">Store item created successfully.</response>
        [HttpPost]
        [ProducesResponseType(typeof(StoreItemDto), 200)]
        public async Task<ActionResult<StoreItemDto>> CreateStoreItem(StoreItemCreateDto dto)
        {
            var created = await _storeItemService.CreateAsync(dto);
            await _hubContext.Clients.All.SendAsync("StoreItemCreated", created);
            return Ok(created);
        }

        /// <summary>
        /// Update an existing store item.
        /// </summary>
        /// <param name="id">The store item ID.</param>
        /// <param name="dto">The updated store item data.</param>
        /// <returns>No content if the update was successful.</returns>
        /// <response code="204">Store item updated successfully.</response>
        /// <response code="404">If the store item was not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateStoreItem(int id, StoreItemUpdateDto dto)
        {
            var updated = await _storeItemService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            var after = await _storeItemService.GetByIdAsync(id);
            await _hubContext.Clients.All.SendAsync("StoreItemUpdated", after);
            return NoContent();
        }

        /// <summary>
        /// Delete a store item by ID.
        /// </summary>
        /// <param name="id">The store item ID.</param>
        /// <returns>No content if the deletion was successful.</returns>
        /// <response code="204">Store item deleted successfully.</response>
        /// <response code="404">If the store item was not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteStoreItem(int id)
        {
            var deleted = await _storeItemService.DeleteAsync(id);
            if (!deleted) return NotFound();
            await _hubContext.Clients.All.SendAsync("StoreItemDeleted", id);
            return NoContent();
        }
    }
}
