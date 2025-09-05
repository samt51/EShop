using EShop.Shared.Dtos.BasesResponses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Features.PaymentFeature.Commands.ReceivePayment;

namespace Payment.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IMediator mediator;
    public PaymentController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost]
    public async Task<ResponseDto<ReceivePaymentCommandResponse>> ReceivePayment(ReceivePaymentCommandRequest request,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(request, cancellationToken);
    }
}
