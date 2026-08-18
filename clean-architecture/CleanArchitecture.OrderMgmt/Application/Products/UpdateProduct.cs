using Domain;
using FluentValidation;

namespace Application.Products;

public record UpdateProductRequest(string Name, decimal Price, int StockQuantity);

public record UpdateProductResult(int Id, string Name, decimal Price, int StockQuantity);

public interface IUpdateProductService
{
    Task<UpdateProductResult?> ExecuteAsync(int id, UpdateProductRequest request);
}

public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
    }
}

public class UpdateProductService(IProductRepository repository, IValidator<UpdateProductRequest> validator) : IUpdateProductService
{
    public async Task<UpdateProductResult?> ExecuteAsync(int id, UpdateProductRequest request)
    {        
        var product = await repository.GetByIdAsync(id);
        if (product is null)
        {
            return null;
        }

        await validator.ValidateAndThrowAsync(request);

        product.Name = request.Name;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;

        var saved = await repository.UpdateAsync(product);

        return new UpdateProductResult(saved.Id, saved.Name, saved.Price, saved.StockQuantity);
    }
}
