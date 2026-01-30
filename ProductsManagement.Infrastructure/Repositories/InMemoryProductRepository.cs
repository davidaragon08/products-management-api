using ProductsManagement.Domain.Entities;
using ProductsManagement.Domain.Exceptions;
using ProductsManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsManagement.Infrastructure.Repositories;

/// <summary>
/// Implementación en memoria del repositorio de productos.
/// </summary>
/// <remarks>
/// - Persistencia en memoria basada en una lista estática, para simular un almacenamiento compartido
///   durante la vida de la aplicación (propósito: challenge técnico).
/// - Thread-safe mediante lock.
/// - Implementa concurrencia optimista usando la propiedad Version:
///   si la versión esperada no coincide con la actual, lanza ConcurrencyException.
/// 
/// Preparado para ampliaciones futuras:
/// Para migrar a EF Core u otra tecnología, basta con crear una nueva implementación de IProductRepository
/// (por ejemplo EfProductRepository) y cambiar el registro de DI.
/// </remarks>
public class InMemoryProductRepository : IProductRepository
{
    private static readonly List<Product> _products = new();
    private static readonly object _lock = new();

    /// <summary>
    /// Obtiene un producto por Id.
    /// </summary>
    public Task<Product?> GetByIdAsync(Guid id)
    {
        lock (_lock)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }
    }

    /// <summary>
    /// Obtiene todos los productos.
    /// </summary>
    public Task<IEnumerable<Product>> GetAllAsync()
    {
        lock (_lock)
        {
            // Se devuelve copia para evitar modificaciones externas sobre la colección interna.
            return Task.FromResult<IEnumerable<Product>>(_products.ToList());
        }
    }

    /// <summary>
    /// Agrega un nuevo producto.
    /// </summary>
    /// <exception cref="InvalidOperationException">Si ya existe un producto con el mismo Id.</exception>
    public Task AddAsync(Product product)
    {
        lock (_lock)
        {
            // Evita duplicados por Id (consistencia del almacenamiento en memoria).
            if (_products.Any(p => p.Id == product.Id))
                throw new InvalidOperationException("Ya existe un producto con el mismo Id.");

            product.Version = 1;
            _products.Add(product);
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Actualiza un producto existente aplicando concurrencia optimista.
    /// </summary>
    /// <remarks>
    /// Si el producto no existe, no realiza ninguna acción.
    /// El servicio de aplicación es quien decide si esto se traduce a 404.
    /// </remarks>
    /// <exception cref="ConcurrencyException">Si la versión esperada no coincide con la versión actual.</exception>
    public Task UpdateAsync(Product product, int expectedVersion)
    {
        lock (_lock)
        {
            var existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing is null)
                return Task.CompletedTask;

            if (existing.Version != expectedVersion)
                throw new ConcurrencyException(
                    $"Conflicto de concurrencia: la versión actual es {existing.Version}."
                );

            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.Quantity = product.Quantity;
            existing.Version++; // incrementa al actualizar

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Elimina un producto aplicando concurrencia optimista.
    /// </summary>
    /// <remarks>
    /// Si el producto no existe, no realiza ninguna acción.
    /// El servicio de aplicación es quien decide si esto se traduce a 404.
    /// </remarks>
    /// <exception cref="ConcurrencyException">Si la versión esperada no coincide con la versión actual.</exception>
    public Task DeleteAsync(Guid id, int expectedVersion)
    {
        lock (_lock)
        {
            var existing = _products.FirstOrDefault(p => p.Id == id);
            if (existing is null)
                return Task.CompletedTask;

            if (existing.Version != expectedVersion)
                throw new ConcurrencyException(
                    $"Conflicto de concurrencia: la versión actual es {existing.Version}."
                );

            _products.Remove(existing);
            return Task.CompletedTask;
        }
    }
}
