using ProductsManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsManagement.Application.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryDto query);
        Task<ProductDto?> GetByIdAsync(Guid id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto?> ReplaceAsync(Guid id, UpdateProductDto dto);
        Task<ProductDto?> PatchAsync(Guid id, PatchProductDto dto);
        Task<bool> DeleteAsync(Guid id, int expectedVersion);
    }

}
