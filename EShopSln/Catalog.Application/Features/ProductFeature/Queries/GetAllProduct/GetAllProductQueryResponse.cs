
using Catalog.Application.Dtos.CategoryDto;

namespace Catalog.Application.Features.ProductFeature.Queries.GetAllProduct;

public class GetAllProductQueryResponse
{
    public int Id {
        get;
        set;
    }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public CategoryResponseDto CategoryResponse { get; set; }
}