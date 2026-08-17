using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Products
{
    public record CreateProductRequest(string Name, decimal Price, int StockQuantity);
}
