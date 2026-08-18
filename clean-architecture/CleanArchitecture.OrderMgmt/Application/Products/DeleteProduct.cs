using Domain;

namespace Application.Products;

public interface IDeleteProductService
{
    Task<bool> ExecuteAsync(int id);
}

public class DeleteProductService(IProductRepository repository) : IDeleteProductService
{
    public async Task<bool> ExecuteAsync(int id)
    {
        var product = await repository.GetByIdAsync(id);

        if (product is null)
        {
            return false;
        }

        await repository.DeleteAsync(product);

        return true;
    }
}
