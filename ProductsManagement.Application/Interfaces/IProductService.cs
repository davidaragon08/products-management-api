using ProductsManagement.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace ProductsManagement.Application.Interfaces;

/// <summary>
/// Contrato de casos de uso para la gestión de productos (Application Layer).
/// </summary>
/// <remarks>
/// Define las operaciones requeridas por la capa API sin exponer detalles de persistencia.
/// 
/// Concurrencia optimista:
/// - Las operaciones de actualización/eliminación utilizan un token de versión (Version).
/// - Si la versión esperada no coincide con la versión actual, se lanza ConcurrencyException
///   desde la capa de persistencia y la API lo traduce a HTTP 409 (Conflict).
/// </remarks>
public interface IProductService
{
    /// <summary>
    /// Obtiene un listado paginado de productos con filtro y ordenación.
    /// </summary>
    Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryDto query);

    /// <summary>
    /// Obtiene un producto por Id, o null si no existe.
    /// </summary>
    Task<ProductDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Crea un producto y devuelve el recurso creado.
    /// </summary>
    Task<ProductDto> CreateAsync(CreateProductDto dto);

    /// <summary>
    /// Reemplaza completamente un producto existente (PUT).
    /// </summary>
    /// <remarks>
    /// El DTO debe incluir la Version esperada para control de concurrencia.
    /// </remarks>
    Task<ProductDto?> ReplaceAsync(Guid id, UpdateProductDto dto);

    /// <summary>
    /// Actualiza parcialmente un producto existente (PATCH).
    /// </summary>
    /// <remarks>
    /// El DTO debe incluir la Version esperada para control de concurrencia.
    /// </remarks>
    Task<ProductDto?> PatchAsync(Guid id, PatchProductDto dto);

    /// <summary>
    /// Elimina un producto existente aplicando concurrencia optimista.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <param name="expectedVersion">Versión esperada del recurso para evitar conflictos concurrentes.</param>
    /// <returns>True si se eliminó; false si el producto no existe.</returns>
    Task<bool> DeleteAsync(Guid id, int expectedVersion);
}
