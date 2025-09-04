using EShop.Shared.Dtos.BasesResponses;
using MediatR;

namespace Order.Application.Features.OrderFeature.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommandRequest, ResponseDto<CreateOrderCommandResponse>>
{
    public Task<ResponseDto<CreateOrderCommandResponse>> Handle(CreateOrderCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}