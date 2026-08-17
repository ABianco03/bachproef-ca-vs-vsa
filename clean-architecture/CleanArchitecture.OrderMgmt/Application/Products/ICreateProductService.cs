using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Products
{
    public interface ICreateProductService
    {
        Task<CreateProductResult> ExecuteAsync(CreateProductRequest request);
    }
}
