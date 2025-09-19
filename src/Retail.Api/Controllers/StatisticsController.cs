using Microsoft.AspNetCore.Mvc;
using Retail.Application.DTOs;
using Retail.Application.Services;
using Microsoft.AspNetCore.SignalR;
using Retail.Api.Hubs;

namespace Retail.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public StatisticsController(IStatisticsService statisticsService, IHubContext<NotificationHub> hubContext)
        {
            _statisticsService = statisticsService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Get supplier statistics by supplier ID.
        /// </summary>
        /// <param name="id">The supplier ID.</param>
        /// <returns>Returns statistics for the given supplier.</returns>
        /// <response code="200">Returns the supplier statistics.</response>
        /// <response code="404">If no statistics were found for the given supplier.</response>
        [HttpGet("supplier/{id}")]
        [ProducesResponseType(typeof(SupplierStatisticDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SupplierStatisticDto>> GetSupplierStatistics(int id)
        {
            var stats = await _statisticsService.GetSupplierStatisticsAsync(id);
            if (stats == null) return NotFound();
            return Ok(stats);
        }

        /// <summary>
        /// Get the best supplier offer for a given store item.
        /// </summary>
        /// <param name="storeItemId">The store item ID.</param>
        /// <returns>Returns the best supplier offer for the item.</returns>
        /// <response code="200">Returns the best offer.</response>
        /// <response code="404">If no offer was found for the item.</response>
        [HttpGet("best-offer/{storeItemId}")]
        [ProducesResponseType(typeof(SupplierBestOfferDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SupplierBestOfferDto>> GetBestOffer(int storeItemId)
        {
            var offer = await _statisticsService.GetBestOfferAsync(storeItemId);
            if (offer == null) return NotFound();
            return Ok(offer);
        }

        /// <summary>
        /// Create a new quarterly plan.
        /// </summary>
        /// <param name="dto">Quarterly plan data.</param>
        /// <returns>Returns status of the creation.</returns>
        /// <response code="200">Quarterly plan created successfully.</response>
        /// <response code="400">If the plan creation failed.</response>
        [HttpPost("quarterly-plan")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateQuarterlyPlan(QuarterlyPlanCreateDto dto)
        {
            var success = await _statisticsService.CreateQuarterlyPlanAsync(dto);
            if (!success) return BadRequest();

            // notify clients about new/updated quarterly plan
            await _hubContext.Clients.All.SendAsync("QuarterlyPlanCreatedOrUpdated", dto);

            return Ok();
        }

        /// <summary>
        /// Get the current year and quarter’s supplier plan.
        /// </summary>
        /// <returns>List of suppliers in the current quarter plan.</returns>
        /// <response code="200">Returns the current quarter plan.</response>
        /// <response code="404">If no quarterly plan was found.</response>
        [HttpGet("quarterly-plan")]
        [ProducesResponseType(typeof(List<SupplierDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<SupplierDto>>> GetCurrentQuarterPlan()
        {
            var plan = await _statisticsService.GetCurrentQuarterPlanAsync();
            if (plan == null) return NotFound();
            return Ok(plan);
        }
    }
}
