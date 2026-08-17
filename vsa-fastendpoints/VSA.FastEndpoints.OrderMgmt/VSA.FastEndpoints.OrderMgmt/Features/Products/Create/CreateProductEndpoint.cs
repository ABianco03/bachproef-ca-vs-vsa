using FastEndpoints;
using FluentValidation;


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

public class CreateProductEndpoint : Endpoint<CreateProductRequest, CreateProductResponse>
{
    private readonly AppDbContext _context;

    public CreateProductEndpoint(AppDbContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("/api/products");
        AllowAnonymous(); // later vervangen door je JWT-vereiste uit de NFR's
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var product = new Product
        {
            Name = req.Name,
            Price = req.Price,
            StockQuantity = req.StockQuantity
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(ct);

        await Send.CreatedAtAsync<CreateProductEndpoint>(
            new { id = product.Id },
            new CreateProductResponse(product.Id, product.Name, product.Price, product.StockQuantity),
            cancellation: ct);
    }
}
