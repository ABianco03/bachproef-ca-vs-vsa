using FastEndpoints;
using VSA.FastEndpoints.OrderMgmt.Features.Products;

namespace VSA.FastEndpoints.OrderMgmt.Features.Products.Get;

public record GetProductRequest(int Id);
public record GetProductResponse(int Id, string Name, decimal Price, int StockQuantity);

public class GetProductEndpoint(AppDbContext context) : Endpoint<GetProductRequest, GetProductResponse>
{

    public override void Configure()
    {
        Get("/api/products/{id}");
    }

    public override async Task HandleAsync(GetProductRequest req, CancellationToken ct)
    {
        var product = await context.Products.FindAsync(new object[] { req.Id }, ct);

        if (product is null)
            ThrowError("Product not found.", statusCode: 404);

        await Send.OkAsync(
            new GetProductResponse(product.Id, product.Name, product.Price, product.StockQuantity),
            ct);
    }
}
