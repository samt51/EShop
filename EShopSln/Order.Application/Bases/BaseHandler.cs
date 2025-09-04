using Order.Application.Interfaces.Mapping;
using Order.Application.Interfaces.UnitOfWorks;

namespace Order.Application.Bases;

public class BaseHandler
{
    public readonly IMapper mapper;
    public readonly IUnitOfWork unitOfWork;
    public BaseHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
    }
}