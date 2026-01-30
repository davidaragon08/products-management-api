using Microsoft.AspNetCore.Mvc;
using ProductsManagement.Application.DTOs;
using ProductsManagement.Application.Interfaces;

namespace ProductsManagement.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET /api/v1/products?page=1&pageSize=20&search=abc&sortBy=name&sortDirection=asc
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts([FromQuery] ProductQueryDto query)
    {
        var result = await _productService.GetProductsAsync(query);
        return Ok(result);
    }

    // GET /api/v1/products/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
            return NotFound();

        return Ok(product);
    }

    // POST /api/v1/products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        var created = await _productService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created
        );
    }

    // PUT /api/v1/products/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Replace(Guid id, [FromBody] UpdateProductDto dto)
    {
        var updated = await _productService.ReplaceAsync(id, dto);
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    // PATCH /api/v1/products/{id}
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Patch(Guid id, [FromBody] PatchProductDto dto)
    {
        var updated = await _productService.PatchAsync(id, dto);
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    // DELETE /api/v1/products/{id}?version=3
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] int version)
    {
        var deleted = await _productService.DeleteAsync(id, version);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

