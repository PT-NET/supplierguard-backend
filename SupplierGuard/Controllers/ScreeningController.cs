using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierGuard.Application.Screening.Commands.PerformScreening;
using SupplierGuard.Application.Screening.DTOs;
using SupplierGuard.Models;

namespace SupplierGuard.Controllers
{
    /// <summary>
    /// Controller for supplier screening against high-risk lists.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class ScreeningController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ScreeningController> _logger;

        public ScreeningController(IMediator mediator, ILogger<ScreeningController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Performs a screening of a supplier against high-risk lists.
        /// </summary>
        /// <param name="supplierId">ID of the supplier to evaluate.</param>
        /// <param name="request">Screening configuration (sources to query).</param>
        /// <remarks>
        /// Available sources:
        /// - 1: Offshore Leaks Database
        /// - 2: World Bank Debarred Firms
        /// - 3: OFAC (Office of Foreign Assets Control)
        /// 
        /// Example request:
        /// 
        ///     POST /api/screening/{supplierId}
        ///     {
        ///         "sources": [1, 2, 3]
        ///     }
        /// 
        /// </remarks>
        [HttpPost("{supplierId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ScreeningResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PerformScreening(
            Guid supplierId,
            [FromBody] ScreeningRequestDto request)
        {
            var command = new PerformScreeningCommand
            {
                SupplierId = supplierId,
                Sources = request.Sources
            };

            var result = await _mediator.Send(command);

            var message = result.IsHighRisk
                ? $"⚠️ HIGH RISK: {result.TotalHits} matches found in screening lists"
                : "✓ No matches found - Supplier is clear";

            return Ok(ApiResponse<ScreeningResultDto>.Ok(result, message));
        }

        /// <summary>
        /// Performs a quick screening across all available sources.
        /// </summary>
        /// <param name="supplierId">ID of the supplier to evaluate.</param>
        [HttpPost("{supplierId:guid}/quick")]
        [ProducesResponseType(typeof(ApiResponse<ScreeningResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> QuickScreening(Guid supplierId)
        {
            var command = new PerformScreeningCommand
            {
                SupplierId = supplierId,
                Sources = new List<int> { 1, 2, 3 }
            };

            var result = await _mediator.Send(command);

            var message = result.IsHighRisk
                ? $"⚠️ HIGH RISK: {result.TotalHits} matches found"
                : "✓ Supplier is clear";

            return Ok(ApiResponse<ScreeningResultDto>.Ok(result, message));
        }
    }
}
