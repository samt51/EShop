using AutoMapper;
using Catalog.Application.Dtos.CategoryDto;
using Catalog.Application.Features.CategoryFeature.Queries.GetAllCategory;
using Catalog.Application.Features.ProductFeature.Queries.GetAllProduct;
using Catalog.Domain.Entities;

namespace Catalog.Persistence.Concrete.Repositories;

public class CatalogProfile :Profile
{
    public CatalogProfile()
    {
        CreateMap<Category, CategoryResponseDto>();

        CreateMap<Product, GetAllProductQueryResponse>()
            .ForMember(d => d.CategoryResponse, o => o.MapFrom(s => s.Category)); // varsa
    
        CreateMap<Category, GetAllCategoryResponse>();
    }
}