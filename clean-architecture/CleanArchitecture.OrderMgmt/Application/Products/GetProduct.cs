using Domain;

namespace Application.Products;

public record GetProductResult(int Id, string Name, decimal Price, int StockQuantity);

public interface IGetProductService
{
    Task<GetProductResult?> ExecuteAsync(int id);
}

public class GetProductService(IProductRepository repository) : IGetProductService
{
    public async Task<GetProductResult?> ExecuteAsync(int id)
    {
        var product = await repository.GetByIdAsync(id);

        if (product is null)
        {
            return null;
        }

        return new GetProductResult(product.Id, product.Name, product.Price, product.StockQuantity);
    }
}
