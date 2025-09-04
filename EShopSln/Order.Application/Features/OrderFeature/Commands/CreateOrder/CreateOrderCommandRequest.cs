using EShop.Shared.Dtos.BasesResponses;
using MediatR;
using Order.Application.Dtos;

namespace Order.Application.Features.OrderFeature.Commands.CreateOrder;

public class CreateOrderCommandRequest:  IRequest<ResponseDto<CreateOrderCommandResponse>>
{
    public string BuyerId { get; set; }

    public List<OrderItemDto> OrderItems { get; set; }

    public AddressDto Address { get; set; }
}