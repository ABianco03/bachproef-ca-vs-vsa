using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Products;

public record CreateProductRequest(string Name, decimal Price, int StockQuantity);
public record CreateProductResult(int Id, string Name, decimal Price, int StockQuantity);

public class CreateProductService : ICreateProductService
{
    private readonly IProductRepository _repository;

    public CreateProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateProductResult> ExecuteAsync(CreateProductRequest request)
    {
        if (request.Price <= 0)
            throw new ArgumentException("Price must be positive.");
        if (request.StockQuantity < 0)
            throw new ArgumentException("StockQuantity cannot be negative.");

        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };

        var saved = await _repository.AddAsync(product);
        return new CreateProductResult(saved.Id, saved.Name, saved.Price, saved.StockQuantity);
    }
}
