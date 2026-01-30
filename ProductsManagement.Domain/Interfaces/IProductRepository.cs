using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductsManagement.Domain.Entities;

namespace ProductsManagement.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task AddAsync(Product product);
        Task UpdateAsync(Product product, int expectedVersion);
        Task DeleteAsync(Guid id, int expectedVersion);
    }
}
