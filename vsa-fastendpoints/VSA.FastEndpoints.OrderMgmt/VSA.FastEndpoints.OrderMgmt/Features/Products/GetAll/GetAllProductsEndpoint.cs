using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using VSA.FastEndpoints.OrderMgmt.Features.Products;

namespace VSA.FastEndpoints.OrderMgmt.Features.Products.GetAll;

public record GetAllProductsResponse(int Id, string Name, decimal Price, int StockQuantity);

public class GetAllProductsEndpoint(AppDbContext context) : EndpointWithoutRequest<List<GetAllProductsResponse>>
{

    public override void Configure()
    {
        Get("/api/products");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var products = await context.Products
            .Select(p => new GetAllProductsResponse(p.Id, p.Name, p.Price, p.StockQuantity))
            .ToListAsync(ct);

        await Send.OkAsync(products, ct);
    }
}
