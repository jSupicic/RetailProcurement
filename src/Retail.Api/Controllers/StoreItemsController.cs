using Microsoft.AspNetCore.Mvc;
using Retail.Application.DTOs;
using Retail.Application.Services;

namespace Retail.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreItemsController : ControllerBase
    {
        private readonly IStoreItemService _storeItemService;

        public StoreItemsController(IStoreItemService storeItemService)
        {
            _storeItemService = storeItemService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoreItemDto>>> GetStoreItems()
        {
            var items = await _storeItemService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StoreItemDto>> GetStoreItem(int id)
        {
            var dto = await _storeItemService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<StoreItemDto>> CreateStoreItem(StoreItemCreateDto dto)
        {
            var created = await _storeItemService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetStoreItem), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStoreItem(int id, StoreItemUpdateDto dto)
        {
            var updated = await _storeItemService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStoreItem(int id)
        {
            var deleted = await _storeItemService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
