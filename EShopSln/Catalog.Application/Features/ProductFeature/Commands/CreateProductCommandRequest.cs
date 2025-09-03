using EShop.Shared.Dtos.BasesResponses;
using MediatR;

namespace Catalog.Application.Features.ProductFeature.Commands;

public class CreateProductCommandRequest : IRequest<ResponseDto<CreateProductCommandResponse>>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public int CategoryId { get; set; }

    public CreateProductCommandRequest(string name, decimal price, int stock, int categoryId)
    {
        this.Name = name;
        this.Price = price;
        this.Stock = stock;
        this.CategoryId = categoryId;
    }
}