using Application.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly ICreateProductService _createProductService;
    private readonly IGetProductService _getProductService;
    private readonly IGetAllProductsService _getAllProductsService;
    private readonly IUpdateProductService _updateProductService;
    private readonly IDeleteProductService _deleteProductService;

    public ProductsController(
        ICreateProductService createProductService,
        IGetProductService getProductService,
        IGetAllProductsService getAllProductsService,
        IUpdateProductService updateProductService,
        IDeleteProductService deleteProductService)
    {
        _createProductService = createProductService;
        _getProductService = getProductService;
        _getAllProductsService = getAllProductsService;
        _updateProductService = updateProductService;
        _deleteProductService = deleteProductService;
    }

    [HttpPost]
    public async Task<ActionResult<CreateProductResult>> Create(CreateProductRequest request)
    {

        var result = await _createProductService.ExecuteAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
 
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetProductResult>> GetById(int id)
    {
        var result = await _getProductService.ExecuteAsync(id);

        if (result is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Resource not found",
                detail: $"Product with id {id} was not found.");
        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetProductResult>>> GetAll()
    {
        var result = await _getAllProductsService.ExecuteAsync();
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateProductResult>> Update(int id, UpdateProductRequest request)
    {
        var result = await _updateProductService.ExecuteAsync(id, request);

        if (result is null)
        {
            return Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Resource not found",
            detail: $"Product with id {id} was not found.");
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _deleteProductService.ExecuteAsync(id);

        if (!deleted)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Resource not found",
                detail: $"Product with id {id} was not found.");
        }

        return NoContent();
    }
}