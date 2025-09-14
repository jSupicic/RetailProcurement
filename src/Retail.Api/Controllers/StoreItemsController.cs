using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retail.Application.DTOs;
using Retail.Domain.Entities;
using Retail.Infrastructure.Context;

namespace Retail.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreItemsController : ControllerBase
    {
        private readonly RetailDbContext _context;

        public StoreItemsController(RetailDbContext context)
        {
            _context = context;
        }

        // GET: /api/store-items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoreItemDto>>> GetStoreItems()
        {
            var items = await _context.StoreItems
                .Include(s => s.SupplierStoreItems)
                    .ThenInclude(ssi => ssi.Supplier)
                .ToListAsync();

            var result = items.Select(item => new StoreItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Suppliers = item.SupplierStoreItems.Select(ssi => new SupplierDto
                {
                    Id = ssi.SupplierId,
                    Name = ssi.Supplier.Name,
                    SupplierPrice = ssi.SupplierPrice
                }).ToList()
            });

            return Ok(result);
        }

        // GET: /api/store-items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<StoreItemDto>> GetStoreItem(int id)
        {
            var item = await _context.StoreItems
                .Include(s => s.SupplierStoreItems)
                    .ThenInclude(ssi => ssi.Supplier)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (item == null)
                return NotFound();

            var dto = new StoreItemDto
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

            return Ok(dto);
        }

        // POST: /api/store-items
        [HttpPost]
        public async Task<ActionResult<StoreItemDto>> CreateStoreItem(StoreItemCreateDto dto)
        {
            var entity = new StoreItem
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
            };

            _context.StoreItems.Add(entity);
            await _context.SaveChangesAsync();

            var result = new StoreItemDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                StockQuantity = entity.StockQuantity
            };

            return CreatedAtAction(nameof(GetStoreItem), new { id = entity.Id }, result);
        }

        // PUT: /api/store-items/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStoreItem(int id, StoreItemUpdateDto dto)
        {
            var entity = await _context.StoreItems.FindAsync(id);
            if (entity == null)
                return NotFound();

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Price = dto.Price;
            entity.StockQuantity = dto.StockQuantity;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/store-items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStoreItem(int id)
        {
            var entity = await _context.StoreItems.FindAsync(id);
            if (entity == null)
                return NotFound();

            _context.StoreItems.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
