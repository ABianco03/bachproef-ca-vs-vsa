using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Products
{
    public interface IProductRepository
    {
        Task<Product> AddAsync(Product product);
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}
