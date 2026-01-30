using System;

namespace ProductsManagement.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa un producto.
/// </summary>
/// <remarks>
/// Esta entidad forma parte del dominio y es independiente de cualquier
/// tecnología de persistencia o framework.
///
/// La concurrencia optimista se maneja mediante la propiedad Version,
/// la cual debe ser proporcionada por el cliente en operaciones de
/// actualización o eliminación.
/// </remarks>
public class Product
{
    /// <summary>
    /// Identificador único del producto.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Nombre del producto.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Precio del producto.
    /// </summary>
    public required decimal Price { get; set; }

    /// <summary>
    /// Cantidad disponible del producto.
    /// </summary>
    public required int Quantity { get; set; }

    /// <summary>
    /// Token de concurrencia optimista.
    /// </summary>
    /// <remarks>
    /// Se incrementa automáticamente en cada actualización exitosa.
    /// El cliente debe enviar la versión esperada para evitar sobrescrituras
    /// concurrentes.
    /// </remarks>
    public int Version { get; set; } = 1;
}
