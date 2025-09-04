using AutoMapper;
using Order.Application.Dtos;
using Order.Application.Features.OrderFeature.Queries.GetOrderByUserId;
using Order.Domain.OrderAggregate;

namespace Order.Infrastructure.Concrete.Mapping;

public class MappingProfile:Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.OrderAggregate.Order, GetOrderByUserIdQueryResponse>().ReverseMap();
        CreateMap<Address, AddressDto>().ReverseMap();
        CreateMap<Domain.OrderAggregate.OrderItem, OrderItemDto>().ReverseMap();
        CreateMap<Domain.OrderAggregate.Order, GetOrderByUserIdQueryResponse>()
            .ForMember(x => x.Address, o => o.MapFrom(s => s.Address));

        CreateMap<Domain.OrderAggregate.Order, GetOrderByUserIdQueryResponse>()
            .ForMember(x => x.OrderItems, o => o.MapFrom(s => s.OrderItems));
    }
}