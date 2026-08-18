using FastEndpoints;
using VSA.FastEndpoints.OrderMgmt.Features.Products;

namespace VSA.FastEndpoints.OrderMgmt.Features.Products.Delete;

public record DeleteProductRequest(int Id);

public class DeleteProductEndpoint(AppDbContext context) : Endpoint<DeleteProductRequest>
{

    public override void Configure()
    {
        Delete("/api/products/{id}");
    }

    public override async Task HandleAsync(DeleteProductRequest req, CancellationToken ct)
    {
        var product = await context.Products.FindAsync(new object[] { req.Id }, ct);

        if (product is null)
            ThrowError("Product not found.", statusCode: 404);

        context.Products.Remove(product);
        await context.SaveChangesAsync(ct);

        await Send.NoContentAsync(ct);
    }
}
