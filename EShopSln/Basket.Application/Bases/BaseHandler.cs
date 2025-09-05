using Basket.Application.Interfaces.Mapping;
using Basket.Application.Interfaces.Repositories;
using MassTransit;

namespace Basket.Application.Bases;

public class BaseHandler
{
    private readonly IBasketRepository _repo;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publish;
    public BaseHandler(IBasketRepository repo,IMapper mapper,IPublishEndpoint publish)
    {
        this._repo = repo;
        this._mapper = mapper;
        this._publish = publish;
    }
}