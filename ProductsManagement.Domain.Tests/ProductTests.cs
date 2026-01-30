using System;
using Xunit;
using ProductsManagement.Domain.Entities;

namespace ProductsManagement.Domain.Tests
{
    public class ProductTests
    {
        [Fact]
        public void Product_DefaultVersion_IsOne()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                Price = 10.0m,
                Quantity = 5
            };

            Assert.Equal(1, product.Version);
        }

        [Fact]
        public void Product_Properties_AreAssignedCorrectly()
        {
            var id = Guid.NewGuid();

            var product = new Product
            {
                Id = id,
                Name = "Test Product",
                Price = 10.0m,
                Quantity = 5,
                Version = 7
            };

            Assert.Equal(id, product.Id);
            Assert.Equal("Test Product", product.Name);
            Assert.Equal(10.0m, product.Price);
            Assert.Equal(5, product.Quantity);
            Assert.Equal(7, product.Version);
        }
    }
}
