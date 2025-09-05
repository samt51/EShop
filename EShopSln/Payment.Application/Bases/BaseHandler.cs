using Payment.Application.Interfaces.Mapping;

namespace Payment.Application.Bases;

public class BaseHandler
{
    public readonly IMapper mapper;

    public BaseHandler(IMapper mapper)
    {
        this.mapper = mapper;
    }
}