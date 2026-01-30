using Microsoft.AspNetCore.Mvc;
using ProductsManagement.Application.DTOs;
using ProductsManagement.Application.Interfaces;

namespace ProductsManagement.Api.Controllers;
/// <summary>
/// Products CRUD endpoints.
/// </summary>
/// <remarks>
/// Implementa operaciones básicas de gestión de productos siguiendo las convenciones REST
/// Incluye paginación, filtrado y clasificación para puntos finales de lista y concurrencia optimista a través de la versión.
/// </remarks>

[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }


    /// <summary>
    /// Devuelve una lista paginada de productos.
    /// </summary>
    /// <remarks>
    /// Query parameters:
    /// - page: índice de página basado en 1 (el valor predeterminado depende de la implementación) 
    /// - pageSize: Tamaño de página (el valor predeterminado depende de la implementación)
    /// - search: término de búsqueda opcional (normalmente coincide con el nombre)
    /// - sortBy: campo de clasificación opcional (por ejemplo, nombre, precio, cantidad)
    /// - sortDirection: dirección de clasificación opcional (asc|desc)
    /// </remarks>


    // GET /api/v1/products?page=1&pageSize=20&search=abc&sortBy=name&sortDirection=asc
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts([FromQuery] ProductQueryDto query)
    {
        var result = await _productService.GetProductsAsync(query);
        return Ok(result);
    }



    /// <summary>
    /// Devuelve detalles del producto por identificación
    /// </summary>


    // GET /api/v1/products/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
            return NotFound();

        return Ok(product);
    }



    /// <summary>
    /// Creacion de nuevo producto.
    /// </summary>
    /// <remarks>
    /// Devuelve 201 Creado con el encabezado de ubicación que apunta a GET /api/v1/products/{id}.
    /// </remarks>


    // POST /api/v1/products
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        var created = await _productService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created
        );
    }



    /// <summary>
    /// Reemplaza un producto por completo (actualización completa)
    /// </summary>
    /// <remarks>
    /// Concurrencia optimista:
    /// - El cliente debe enviar la versión esperada en la carga útil
    /// - Si la versión actual es diferente, la API devuelve el error 409 Conflict (manejado por el middleware global).
    /// </remarks>


    // PUT /api/v1/products/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDto>> Replace(Guid id, [FromBody] UpdateProductDto dto)
    {
        var updated = await _productService.ReplaceAsync(id, dto);
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }


    /// <summary>
    /// Actualiza parcialmente un producto
    /// </summary>
    /// <remarks>
    /// Concurrencia optimista:
    /// - El cliente debe enviar la versión esperada en la carga útil
    /// - Si la versión actual es diferente, la API devuelve el error 409 Conflict (manejado por el middleware global).
    /// </remarks> 


    // PATCH /api/v1/products/{id}
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDto>> Patch(Guid id, [FromBody] PatchProductDto dto)
    {
        var updated = await _productService.PatchAsync(id, dto);
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }



    /// <summary>
    /// Elimina el producto
    /// </summary>
    /// <remarks>
    /// Concurrencia optimista:
    /// - El cliente debe proporcionar la versión esperada utilizando el parámetro de consulta "versión"
    /// - Si la versión actual es diferente, la API devuelve el error 409 Conflict (manejado por el middleware global).
    /// </remarks> 


    // DELETE /api/v1/products/{id}?version=3
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] int version)
    {
        var deleted = await _productService.DeleteAsync(id, version);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

