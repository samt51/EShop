using AutoMapper;
using Basket.Application.Dtos.BasketDtos;
using Basket.Application.Dtos.BasketItemsDtos;
using Basket.Application.Features.BasketFeature.Queries;

namespace Basket.Persistence.Concrete.Mapping;

public class BasketProfile : Profile
{
    public BasketProfile()
    {
        CreateMap<Domain.Entities.Basket, BasketResponseDto>().ReverseMap();
        CreateMap<Domain.Entities.BasketItem, BasketItemResponseDto>().ReverseMap();
        
        CreateMap<Domain.Entities.Basket, BasketResponseDto>()
            .ForMember(d => d.basketItems, o => o.MapFrom(s => s.basketItems)); 
        
        CreateMap<BasketResponseDto,GetAllBasketQueryResponse>().ReverseMap();
        
        CreateMap<BasketResponseDto,GetAllBasketQueryResponse>().ForMember(
            d=>d.basketItems, o => o.MapFrom(s => s.basketItems));
    }
}