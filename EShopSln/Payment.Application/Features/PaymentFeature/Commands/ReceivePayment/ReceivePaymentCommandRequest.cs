using EShop.Shared.Dtos.BasesResponses;
using MediatR;
using Payment.Application.Dtos;

namespace Payment.Application.Features.PaymentFeature.Commands.ReceivePayment;

public class ReceivePaymentCommandRequest :  IRequest<ResponseDto<ReceivePaymentCommandResponse>>
{
    public string CardName { get; set; }
    public string CardNumber { get; set; }
    public string Expiration { get; set; }
    public string CVV { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderDto Order { get; set; }
}