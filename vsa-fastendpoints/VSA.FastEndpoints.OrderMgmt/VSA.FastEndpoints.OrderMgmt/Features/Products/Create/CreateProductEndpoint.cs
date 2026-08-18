using FastEndpoints;
using FluentValidation;
using VSA.FastEndpoints.OrderMgmt.Features.Products;

namespace VSA.FastEndpoints.OrderMgmt.Features.Products.Create;
public record CreateProductRequest(string Name, decimal Price, int StockQuantity);
public record CreateProductResponse(int Id, string Name, decimal Price, int StockQuantity);

public class CreateProductValidator : Validator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
    }
}

public class CreateProductEndpoint(AppDbContext context) : Endpoint<CreateProductRequest, CreateProductResponse>
{

    public override void Configure()
    {
        Post("/api/products");
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var product = new Product
        {
            Name = req.Name,
            Price = req.Price,
            StockQuantity = req.StockQuantity
        };

        context.Products.Add(product);
        await context.SaveChangesAsync(ct);

        await Send.CreatedAtAsync<CreateProductEndpoint>(
            new { id = product.Id },
            new CreateProductResponse(product.Id, product.Name, product.Price, product.StockQuantity),
            cancellation: ct);
    }
}
