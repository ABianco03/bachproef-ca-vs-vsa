using Application.Products;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ICreateProductService _createProductService;

    public ProductsController(ICreateProductService createProductService)
    {
        _createProductService = createProductService;
    }

    [HttpPost]
    public async Task<ActionResult<CreateProductResult>> Create(CreateProductRequest request)
    {
        try
        {
            var result = await _createProductService.ExecuteAsync(request);
            return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}