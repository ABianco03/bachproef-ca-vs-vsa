using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Products
{
    public record CreateProductResult(int Id, string Name, decimal Price, int StockQuantity);

}
