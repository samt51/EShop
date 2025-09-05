using EShop.Shared.Dtos.BasesResponses;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Payment.Application.Bases;
using Payment.Application.Interfaces.Mapping;

namespace Payment.Application.Features.PaymentFeature.Commands.ReceivePayment;

public class ReceivePaymentCommandHandler :  BaseHandler, IRequestHandler<ReceivePaymentCommandRequest, ResponseDto<ReceivePaymentCommandResponse>>
{
    private readonly IPublishEndpoint _publish;       
  

    public ReceivePaymentCommandHandler(
        IMapper mapper,
        IPublishEndpoint publish,                   
        IConfiguration configuration) 
        : base(mapper)
    {
        _publish = publish;
    }

    public async Task<ResponseDto<ReceivePaymentCommandResponse>> Handle(ReceivePaymentCommandRequest request, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid();

        await _publish.Publish<EShop.Shared.Messages.Events.PaymentAuthorizedEvent>(new
        {
            CorrelationId = correlationId,
            BuyerId = request.Order.BuyerId,
            Province = request.Order.Address.Province,
            District = request.Order.Address.District,
            Street   = request.Order.Address.Street,
            Line     = request.Order.Address.Line,
            ZipCode  = request.Order.Address.ZipCode,
            PaymentId = Guid.NewGuid().ToString(),
            OrderItems = request.Order.OrderItems.Select(x => new {
                x.ProductId,
                x.ProductName,
                x.Price,
                x.PictureUrl
            }).ToList()
        }, cancellationToken);
        
        return new ResponseDto<ReceivePaymentCommandResponse>().Success();
    }
}