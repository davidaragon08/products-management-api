using Microsoft.Extensions.Logging;
using ProductsManagement.Application.DTOs;
using ProductsManagement.Application.Interfaces;
using ProductsManagement.Domain.Entities;
using ProductsManagement.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsManagement.Application.Services;


public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    // GET list (paginación + filtro + ordenación)
    public virtual async Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryDto query)
    {
        _logger.LogInformation("Listando productos. Page={Page}, PageSize={PageSize}, Search={Search}, SortBy={SortBy}, SortDirection={SortDirection}",
            query.Page, query.PageSize, query.Search, query.SortBy, query.SortDirection);

        var products = await _productRepository.GetAllAsync();
        var q = products.AsQueryable();

        // Filter
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            q = q.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        // Sort (allow-list)
        var sortBy = (query.SortBy ?? "name").Trim().ToLowerInvariant();
        var sortDir = (query.SortDirection ?? "asc").Trim().ToLowerInvariant();
        var desc = sortDir == "desc";

        q = sortBy switch
        {
            "name" => desc ? q.OrderByDescending(p => p.Name) : q.OrderBy(p => p.Name),
            "price" => desc ? q.OrderByDescending(p => p.Price) : q.OrderBy(p => p.Price),
            "quantity" => desc ? q.OrderByDescending(p => p.Quantity) : q.OrderBy(p => p.Quantity),
            _ => q.OrderBy(p => p.Name) // default seguro
        };

        var total = q.Count();

        var items = q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(ToDto)
            .ToList();

        return new PagedResult<ProductDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = total
        };
    }

    public virtual async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Obteniendo producto. Id={Id}", id);

        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            _logger.LogWarning("Producto no encontrado. Id={Id}", id);
            return null;
        }

        return ToDto(product);
    }

    public virtual async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        _logger.LogInformation("Creando producto. Name={Name}", dto.Name);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Price = dto.Price,
            Quantity = dto.Quantity,
            Version = 1
        };

        await _productRepository.AddAsync(product);

        _logger.LogInformation("Producto creado correctamente. Id={Id}", product.Id);
        return ToDto(product);
    }



    // PUT (reemplazo completo)
    public virtual async Task<ProductDto?> ReplaceAsync(Guid id, UpdateProductDto dto)
    {
        _logger.LogInformation("Reemplazando producto. Id={Id}, VersionEsperada={Version}", id, dto.Version);

        var existing = await _productRepository.GetByIdAsync(id);
        if (existing is null)
        {
            _logger.LogWarning("Producto no encontrado para reemplazo. Id={Id}", id);
            return null;
        }

        existing.Name = dto.Name;
        existing.Price = dto.Price;
        existing.Quantity = dto.Quantity;

        await _productRepository.UpdateAsync(existing, dto.Version);

        _logger.LogInformation("Producto reemplazado. Id={Id}, NuevaVersion={NewVersion}", id, existing.Version);
        return ToDto(existing);
    }

    // PATCH (actualización parcial)
    public virtual async Task<ProductDto?> PatchAsync(Guid id, PatchProductDto dto)
    {
        _logger.LogInformation("Actualizando parcialmente producto. Id={Id}, VersionEsperada={Version}", id, dto.Version);

        var existing = await _productRepository.GetByIdAsync(id);
        if (existing is null)
        {
            _logger.LogWarning("Producto no encontrado para patch. Id={Id}", id);
            return null;
        }

        ApplyPatch(existing, dto);

        await _productRepository.UpdateAsync(existing, dto.Version);

        _logger.LogInformation("Producto actualizado parcialmente. Id={Id}, NuevaVersion={NewVersion}", id, existing.Version);
        return ToDto(existing);
    }

    public virtual async Task<bool> DeleteAsync(Guid id, int expectedVersion)
    {
        _logger.LogInformation("Eliminando producto. Id={Id}, VersionEsperada={Version}", id, expectedVersion);

        var existing = await _productRepository.GetByIdAsync(id);
        if (existing is null)
        {
            _logger.LogWarning("Producto no encontrado para eliminación. Id={Id}", id);
            return false;
        }

        await _productRepository.DeleteAsync(id, expectedVersion);

        _logger.LogInformation("Producto eliminado correctamente. Id={Id}", id);
        return true;
    }

    // -------------------------
    // Mapping helpers
    // -------------------------

    private static ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Quantity = product.Quantity,
            Version = product.Version
        };
    }

    private static void ApplyPatch(Product product, PatchProductDto dto)
    {
        if (dto.Name is not null)
            product.Name = dto.Name;

        if (dto.Price.HasValue)
            product.Price = dto.Price.Value;

        if (dto.Quantity.HasValue)
            product.Quantity = dto.Quantity.Value;
    }
}
