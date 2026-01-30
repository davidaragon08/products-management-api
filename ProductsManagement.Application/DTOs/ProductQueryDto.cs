using System.ComponentModel.DataAnnotations;

namespace ProductsManagement.Application.DTOs;

/// <summary>
/// DTO de consulta para listado de productos.
/// </summary>
/// <remarks>
/// Define los parámetros de paginación, filtrado y ordenación permitidos
/// para el endpoint GET /api/v1/products.
///
/// La validación se realiza mediante DataAnnotations y es aplicada automáticamente
/// por el framework al usar [ApiController].
/// </remarks>
public class ProductQueryDto
{
    /// <summary>
    /// Número de página (base 1).
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page debe ser mayor o igual a 1.")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Cantidad de elementos por página.
    /// </summary>
    [Range(1, 100, ErrorMessage = "PageSize debe estar entre 1 y 100.")]
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Filtro opcional por nombre de producto (búsqueda parcial, case-insensitive).
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Campo de ordenación.
    /// Campos permitidos: name, price, quantity.
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Dirección de ordenación.
    /// Valores permitidos: asc | desc.
    /// </summary>
    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection debe ser 'asc' o 'desc'.")]
    public string? SortDirection { get; set; }
}
