using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Products
{
    public interface IProductRepository
    {
        Task<Product> AddAsync(Product product);
    }
}
