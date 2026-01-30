using System;
using System.Collections.Generic;

namespace ProductsManagement.Application.DTOs;

/// <summary>
/// Resultado paginado genérico.
/// </summary>
/// <remarks>
/// Representa una página de resultados junto con la información necesaria
/// para navegación (paginación).
///
/// Este DTO es inmutable (init-only) y desacoplado de la capa de persistencia.
/// </remarks>
public class PagedResult<T>
{
    /// <summary>
    /// Elementos de la página actual.
    /// </summary>
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    /// <summary>
    /// Número de página actual (base 1).
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Cantidad de elementos por página.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Cantidad total de elementos disponibles.
    /// </summary>
    public int TotalItems { get; init; }

    /// <summary>
    /// Cantidad total de páginas calculadas a partir de TotalItems y PageSize.
    /// </summary>
    public int TotalPages =>
        PageSize <= 0
            ? 0
            : (int)Math.Ceiling(TotalItems / (double)PageSize);
}
