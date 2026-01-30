using ProductsManagement.Domain.Entities;
using ProductsManagement.Domain.Exceptions;
using ProductsManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsManagement.Infrastructure.Repositories
{
    public class InMemoryProductRepository : IProductRepository
    {
        private static readonly List<Product> _products = new();
        private static readonly object _lock = new();

        public Task<Product?> GetByIdAsync(Guid id)
        {
            lock (_lock)
            {
                var product = _products.FirstOrDefault(p => p.Id == id);
                return Task.FromResult(product);
            }
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            lock (_lock)
            {
                return Task.FromResult<IEnumerable<Product>>(_products.ToList());
            }
        }

        public Task AddAsync(Product product)
        {
            lock (_lock)
            {
                // opcional: evitar duplicados
                if (_products.Any(p => p.Id == product.Id))
                    throw new InvalidOperationException("Product already exists.");

                product.Version = 1;
                _products.Add(product);
                return Task.CompletedTask;
            }
        }

        public Task UpdateAsync(Product product, int expectedVersion)
        {
            lock (_lock)
            {
                var existing = _products.FirstOrDefault(p => p.Id == product.Id);
                if (existing == null) return Task.CompletedTask; // o NotFoundException (según tu diseño)

                if (existing.Version != expectedVersion)
                    throw new ConcurrencyException($"Version mismatch. Current version is {existing.Version}.");

                existing.Name = product.Name;
                existing.Price = product.Price;
                existing.Quantity = product.Quantity;
                existing.Version++; // incrementa al actualizar

                return Task.CompletedTask;
            }
        }

        public Task DeleteAsync(Guid id, int expectedVersion)
        {
            lock (_lock)
            {
                var existing = _products.FirstOrDefault(p => p.Id == id);
                if (existing == null) return Task.CompletedTask;

                if (existing.Version != expectedVersion)
                    throw new ConcurrencyException($"Version mismatch. Current version is {existing.Version}.");

                _products.Remove(existing);
                return Task.CompletedTask;
            }
        }
    }

}
