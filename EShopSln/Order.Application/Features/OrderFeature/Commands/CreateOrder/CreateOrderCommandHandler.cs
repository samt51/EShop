using EShop.Shared.Dtos.BasesResponses;
using MediatR;
using Order.Application.Bases;
using Order.Application.Dtos;
using Order.Application.Interfaces.Mapping;
using Order.Application.Interfaces.UnitOfWorks;
using Order.Domain.OrderAggregate;

namespace Order.Application.Features.OrderFeature.Commands.CreateOrder;

public class CreateOrderCommandHandler : BaseHandler,IRequestHandler<CreateOrderCommandRequest, ResponseDto<CreateOrderCommandResponse>>
{
    public CreateOrderCommandHandler(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
    }

    public async Task<ResponseDto<CreateOrderCommandResponse>> Handle(CreateOrderCommandRequest request, CancellationToken cancellationToken)
    {
        var mapToAddress = mapper.Map<Address, AddressDto>(request.Address);

        Domain.OrderAggregate.Order newOrder = new Domain.OrderAggregate.Order(request.BuyerId, mapToAddress);

        request.OrderItems.ForEach(x =>
        {
            newOrder.AddOrderItem(x.ProductId, x.ProductName, x.Price, x.PictureUrl);
        });
        
        var save = await unitOfWork.GetWriteRepository<Domain.OrderAggregate.Order>().AddAsync(newOrder, cancellationToken);

         unitOfWork.OpenTransaction();

         await unitOfWork.SaveAsync();
         
         await unitOfWork.CommitAsync();

         return new ResponseDto<CreateOrderCommandResponse>().Success();
    }
}