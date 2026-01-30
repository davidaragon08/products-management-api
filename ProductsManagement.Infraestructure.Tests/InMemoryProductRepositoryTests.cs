using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ProductsManagement.Domain.Entities;
using ProductsManagement.Domain.Exceptions;
using ProductsManagement.Infrastructure.Repositories;
using Xunit;

namespace ProductsManagement.Infrastructure.Tests
{
    public class InMemoryProductRepositoryTests
    {
        public InMemoryProductRepositoryTests()
        {
            ResetStaticProducts();
        }

        [Fact]
        public async Task AddAsync_ShouldAddProduct_WithVersionOne()
        {
            var repository = new InMemoryProductRepository();
            var product = new Product { Id = Guid.NewGuid(), Name = "Test", Price = 10, Quantity = 5 };

            await repository.AddAsync(product);
            var result = await repository.GetByIdAsync(product.Id);

            Assert.NotNull(result);
            Assert.Equal(product.Id, result!.Id);
            Assert.Equal(1, result.Version);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            var repository = new InMemoryProductRepository();

            var result = await repository.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            var repository = new InMemoryProductRepository();

            await repository.AddAsync(new Product { Id = Guid.NewGuid(), Name = "P1", Price = 10, Quantity = 1 });
            await repository.AddAsync(new Product { Id = Guid.NewGuid(), Name = "P2", Price = 20, Quantity = 2 });

            var result = await repository.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_WithMatchingVersion_ShouldUpdateAndIncrementVersion()
        {
            var repository = new InMemoryProductRepository();
            var product = new Product { Id = Guid.NewGuid(), Name = "Test", Price = 10, Quantity = 5 };

            await repository.AddAsync(product);

            var existing = await repository.GetByIdAsync(product.Id);
            Assert.NotNull(existing);
            Assert.Equal(1, existing!.Version);

            existing.Name = "Updated";
            existing.Price = 99;

            await repository.UpdateAsync(existing, expectedVersion: 1);

            var updated = await repository.GetByIdAsync(product.Id);
            Assert.NotNull(updated);
            Assert.Equal("Updated", updated!.Name);
            Assert.Equal(99, updated.Price);
            Assert.Equal(2, updated.Version); // incrementa
        }

        [Fact]
        public async Task UpdateAsync_WithWrongVersion_ShouldThrowConcurrencyException()
        {
            var repository = new InMemoryProductRepository();
            var product = new Product { Id = Guid.NewGuid(), Name = "Test", Price = 10, Quantity = 5 };

            await repository.AddAsync(product);

            var existing = await repository.GetByIdAsync(product.Id);
            Assert.NotNull(existing);

            existing!.Name = "Updated";

            await Assert.ThrowsAsync<ConcurrencyException>(() =>
                repository.UpdateAsync(existing, expectedVersion: 999));
        }

        [Fact]
        public async Task DeleteAsync_WithMatchingVersion_ShouldDelete()
        {
            var repository = new InMemoryProductRepository();
            var product = new Product { Id = Guid.NewGuid(), Name = "Test", Price = 10, Quantity = 5 };

            await repository.AddAsync(product);

            await repository.DeleteAsync(product.Id, expectedVersion: 1);

            var result = await repository.GetByIdAsync(product.Id);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_WithWrongVersion_ShouldThrowConcurrencyException()
        {
            var repository = new InMemoryProductRepository();
            var product = new Product { Id = Guid.NewGuid(), Name = "Test", Price = 10, Quantity = 5 };

            await repository.AddAsync(product);

            await Assert.ThrowsAsync<ConcurrencyException>(() =>
                repository.DeleteAsync(product.Id, expectedVersion: 999));
        }

        private static void ResetStaticProducts()
        {
            // Resetea el private static readonly List<Product> _products usando reflection.
            // Funciona para challenge; alternativa más limpia: método internal ClearForTests() + InternalsVisibleTo.
            var repoType = typeof(InMemoryProductRepository);
            var field = repoType.GetField("_products", BindingFlags.NonPublic | BindingFlags.Static);
            if (field?.GetValue(null) is System.Collections.IList list)
            {
                list.Clear();
            }
        }
    }
}
