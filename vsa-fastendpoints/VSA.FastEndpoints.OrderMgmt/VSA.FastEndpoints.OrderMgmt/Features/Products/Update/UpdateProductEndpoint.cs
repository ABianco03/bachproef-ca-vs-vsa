using FastEndpoints;
using FluentValidation;
using VSA.FastEndpoints.OrderMgmt.Features.Products;

namespace VSA.FastEndpoints.OrderMgmt.Features.Products.Update;

public record UpdateProductRequest(int Id, string Name, decimal Price, int StockQuantity);
public record UpdateProductResponse(int Id, string Name, decimal Price, int StockQuantity);

public class UpdateProductValidator : Validator<UpdateProductRequest>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
    }
}

public class UpdateProductEndpoint(AppDbContext context) : Endpoint<UpdateProductRequest, UpdateProductResponse>
{

    public override void Configure()
    {
        Put("/api/products/{id}");
        DontThrowIfValidationFails();
    }

    public override async Task HandleAsync(UpdateProductRequest req, CancellationToken ct)
    {
        var product = await context.Products.FindAsync(new object[] { req.Id }, ct);
        if (product is null)
            ThrowError("Product not found.", statusCode: 404);

        if (ValidationFailures.Count > 0)
        {
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        product.Name = req.Name;
        product.Price = req.Price;
        product.StockQuantity = req.StockQuantity;

        await context.SaveChangesAsync(ct);

        await Send.OkAsync(
            new UpdateProductResponse(product.Id, product.Name, product.Price, product.StockQuantity),
            ct);
    }
}
