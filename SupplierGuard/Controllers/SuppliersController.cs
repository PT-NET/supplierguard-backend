using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierGuard.Application.Suppliers.Commands.CreateSupplier;
using SupplierGuard.Application.Suppliers.Commands.DeleteSupplier;
using SupplierGuard.Application.Suppliers.Commands.UpdateSupplier;
using SupplierGuard.Application.Suppliers.DTOs;
using SupplierGuard.Application.Suppliers.Queries.GetAllSuppliers;
using SupplierGuard.Application.Suppliers.Queries.GetSupplierById;
using SupplierGuard.Application.Suppliers.Queries.GetSuppliersWithFilters;
using SupplierGuard.Models;

namespace SupplierGuard.Controllers
{
    /// <summary>
    /// Controller for supplier management.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class SuppliersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SuppliersController> _logger;

        public SuppliersController(IMediator mediator, ILogger<SuppliersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets all suppliers with optional filters and pagination.
        /// </summary>
        /// <param name="searchTerm">Search term (optional).</param>
        /// <param name="country">Country filter (optional).</param>
        /// <param name="minRevenue">Minimum revenue (optional).</param>
        /// <param name="maxRevenue">Maximum revenue (optional).</param>
        /// <param name="orderBy">Sort field (default: LastModifiedAt).</param>
        /// <param name="ascending">Ascending order (default: false).</param>
        /// <param name="pageNumber">Page number (default: 1).</param>
        /// <param name="pageSize">Page size (default: 10, max: 100).</param>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<Application.Common.Models.PaginatedList<SupplierListDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSuppliers(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? country = null,
            [FromQuery] decimal? minRevenue = null,
            [FromQuery] decimal? maxRevenue = null,
            [FromQuery] string orderBy = "LastModifiedAt",
            [FromQuery] bool ascending = false,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetSuppliersWithFiltersQuery
            {
                SearchTerm = searchTerm,
                Country = country,
                MinRevenue = minRevenue,
                MaxRevenue = maxRevenue,
                OrderBy = orderBy,
                Ascending = ascending,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            return Ok(ApiResponse<Application.Common.Models.PaginatedList<SupplierListDto>>.Ok(
                result,
                $"Retrieved {result.Items.Count} suppliers (page {result.PageNumber} of {result.TotalPages})"
            ));
        }

        /// <summary>
        /// Gets a supplier by its ID.
        /// </summary>
        /// <param name="id">Supplier ID.</param>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<SupplierDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSupplier(Guid id)
        {
            var query = new GetSupplierByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<SupplierDto>.Ok(result, "Supplier retrieved successfully"));
        }

        /// <summary>
        /// Creates a new supplier.
        /// </summary>
        /// <param name="dto">Supplier data to create.</param>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SupplierDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto dto)
        {
            var command = new CreateSupplierCommand
            {
                LegalName = dto.LegalName,
                CommercialName = dto.CommercialName,
                TaxId = dto.TaxId,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Website = dto.Website,
                PhysicalAddress = dto.PhysicalAddress,
                Country = dto.Country,
                AnnualRevenue = dto.AnnualRevenue
            };

            var result = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetSupplier),
                new { id = result.Id },
                ApiResponse<SupplierDto>.Ok(result, "Supplier created successfully")
            );
        }

        /// <summary>
        /// Updates an existing supplier.
        /// </summary>
        /// <param name="id">ID of the supplier to update.</param>
        /// <param name="dto">Updated supplier data.</param>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<SupplierDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateSupplier(Guid id, [FromBody] UpdateSupplierDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(ApiResponse<object>.Fail("ID in URL does not match ID in body"));
            }

            var command = new UpdateSupplierCommand
            {
                Id = dto.Id,
                LegalName = dto.LegalName,
                CommercialName = dto.CommercialName,
                TaxId = dto.TaxId,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Website = dto.Website,
                PhysicalAddress = dto.PhysicalAddress,
                Country = dto.Country,
                AnnualRevenue = dto.AnnualRevenue
            };

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<SupplierDto>.Ok(result, "Supplier updated successfully"));
        }

        /// <summary>
        /// Deletes a supplier.
        /// </summary>
        /// <param name="id">ID of the supplier to delete.</param>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            var command = new DeleteSupplierCommand { Id = id };
            await _mediator.Send(command);

            return Ok(ApiResponse.Ok("Supplier deleted successfully"));
        }

        /// <summary>
        /// Gets all suppliers without filters (simplified version).
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(typeof(ApiResponse<List<SupplierListDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSuppliers()
        {
            var query = new GetAllSuppliersQuery();
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<List<SupplierListDto>>.Ok(
                result,
                $"Retrieved {result.Count} suppliers"
            ));
        }
    }

}
