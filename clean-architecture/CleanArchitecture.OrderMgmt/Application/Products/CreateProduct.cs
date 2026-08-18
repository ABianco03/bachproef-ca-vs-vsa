using Domain;
using FluentValidation;

namespace Application.Products;

public record CreateProductRequest(string Name, decimal Price, int StockQuantity);

public record CreateProductResult(int Id, string Name, decimal Price, int StockQuantity);

public interface ICreateProductService
{
    Task<CreateProductResult> ExecuteAsync(CreateProductRequest request);
}

public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be positive.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("StockQuantity cannot be negative.");
    }
}

public class CreateProductService(IProductRepository repository, IValidator<CreateProductRequest> validator) : ICreateProductService
{
    public async Task<CreateProductResult> ExecuteAsync(CreateProductRequest request)
    {
        await validator.ValidateAndThrowAsync(request);

        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };

        var saved = await repository.AddAsync(product);

        return new CreateProductResult(saved.Id, saved.Name, saved.Price, saved.StockQuantity);
    }
}
