using Domain;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Products;


public class CreateProductService(IProductRepository repository, IValidator<CreateProductRequest> validator) : ICreateProductService
{

    public async Task<CreateProductResult> ExecuteAsync(CreateProductRequest request)
    {
        await validator.ValidateAndThrowAsync(request);

        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };

        var saved = await repository.AddAsync(product);

        return new CreateProductResult(saved.Id, saved.Name, saved.Price, saved.StockQuantity);
    }
}
