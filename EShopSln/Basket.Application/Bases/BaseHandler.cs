using Basket.Application.Interfaces.Mapping;
using Basket.Application.Interfaces.Repositories;

namespace Basket.Application.Bases;

public class BaseHandler
{
    private readonly IBasketRepository _repo;
    private readonly IMapper _mapper;
    public BaseHandler(IBasketRepository repo,IMapper mapper)
    {
        this._repo = repo;
        this._mapper = mapper;
    }
}