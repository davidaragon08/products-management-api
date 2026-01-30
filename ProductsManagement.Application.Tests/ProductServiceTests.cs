using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using ProductsManagement.Application.DTOs;
using ProductsManagement.Application.Services;
using ProductsManagement.Domain.Entities;
using ProductsManagement.Domain.Exceptions;
using ProductsManagement.Domain.Interfaces;

namespace ProductsManagement.Application.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repoMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repoMock = new Mock<IProductRepository>();
        var logger = new Mock<ILogger<ProductService>>();
        _service = new ProductService(_repoMock.Object, logger.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct_WithVersionOne()
    {
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Product>()))
                 .Returns(Task.CompletedTask);

        var dto = new CreateProductDto
        {
            Name = "Keyboard",
            Price = 100,
            Quantity = 10
        };

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(1, result.Version);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                 .ReturnsAsync((Product?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetProductsAsync_ShouldApplyPagination()
    {
        var products = Enumerable.Range(1, 30)
            .Select(i => new Product
            {
                Id = Guid.NewGuid(),
                Name = $"Product {i}",
                Price = i,
                Quantity = i,
                Version = 1
            }).ToList();

        _repoMock.Setup(r => r.GetAllAsync())
                 .ReturnsAsync(products);

        var query = new ProductQueryDto
        {
            Page = 2,
            PageSize = 10
        };

        var result = await _service.GetProductsAsync(query);

        Assert.Equal(10, result.Items.Count);
        Assert.Equal(30, result.TotalItems);
        Assert.Equal(2, result.Page);
    }

    [Fact]
    public async Task ReplaceAsync_ShouldUpdate_WhenVersionMatches()
    {
        var id = Guid.NewGuid();
        var product = new Product
        {
            Id = id,
            Name = "Old",
            Price = 10,
            Quantity = 1,
            Version = 1
        };

        _repoMock.Setup(r => r.GetByIdAsync(id))
                 .ReturnsAsync(product);

        _repoMock.Setup(r => r.UpdateAsync(product, 1))
                 .Returns(Task.CompletedTask);

        var dto = new UpdateProductDto
        {
            Name = "New",
            Price = 20,
            Quantity = 2,
            Version = 1
        };

        var result = await _service.ReplaceAsync(id, dto);

        Assert.NotNull(result);
        _repoMock.Verify(r => r.UpdateAsync(product, 1), Times.Once);
    }

    [Fact]
    public async Task ReplaceAsync_ShouldThrowConcurrencyException_WhenVersionMismatch()
    {
        var id = Guid.NewGuid();
        var product = new Product
        {
            Id = id,
            Name = "Any",
            Price = 1m,
            Quantity = 1,
            Version = 2
        };

        _repoMock.Setup(r => r.GetByIdAsync(id))
                 .ReturnsAsync(product);

        _repoMock.Setup(r => r.UpdateAsync(product, 1))
                 .ThrowsAsync(new ConcurrencyException("conflict"));

        var dto = new UpdateProductDto
        {
            Name = "Test",
            Price = 1,
            Quantity = 1,
            Version = 1
        };

        await Assert.ThrowsAsync<ConcurrencyException>(() =>
            _service.ReplaceAsync(id, dto));
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                 .ReturnsAsync((Product?)null);

        var result = await _service.DeleteAsync(Guid.NewGuid(), 1);

        Assert.False(result);
    }
}
