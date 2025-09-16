using Microsoft.AspNetCore.Mvc;
using Retail.Application.DTOs;
using Retail.Application.Services;
using Retail.Domain.Entities;

namespace Retail.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("supplier/{id}")]
        public async Task<ActionResult<SupplierStatisticDto>> GetSupplierStatistics(int id)
        {
            var stats = await _statisticsService.GetSupplierStatisticsAsync(id);
            if (stats == null) return NotFound();
            return Ok(stats);
        }

        [HttpGet("best-offer/{storeItemId}")]
        public async Task<ActionResult<SupplierBestOfferDto>> GetBestOffer(int storeItemId)
        {
            var offer = await _statisticsService.GetBestOfferAsync(storeItemId);
            if (offer == null) return NotFound();
            return Ok(offer);
        }

        [HttpPost("quarterly-plan")]
        public async Task<IActionResult> CreateQuarterlyPlan(QuarterlyPlanCreateDto dto)
        {
            var success = await _statisticsService.CreateQuarterlyPlanAsync(dto);
            if (!success) return BadRequest();
            return Ok();
        }

        [HttpGet("quarterly-plan")]
        public async Task<ActionResult<List<SupplierDto>>> GetCurrentQuarterPlan()
        {
            var plan = await _statisticsService.GetCurrentQuarterPlanAsync();
            if (plan == null) return NotFound();
            return Ok(plan);
        }
    }
}
