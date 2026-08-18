using Domain;

namespace Application.Products;

public interface IGetAllProductsService
{
    Task<IEnumerable<GetProductResult>> ExecuteAsync();
}

public class GetAllProductsService(IProductRepository repository) : IGetAllProductsService
{
    public async Task<IEnumerable<GetProductResult>> ExecuteAsync()
    {
        var products = await repository.GetAllAsync();

        return products.Select(p => new GetProductResult(p.Id, p.Name, p.Price, p.StockQuantity));
    }
}
