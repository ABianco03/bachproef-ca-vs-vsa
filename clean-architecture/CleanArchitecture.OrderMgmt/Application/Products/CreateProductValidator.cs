using FluentValidation;

namespace Application.Products;

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