using Catalog.Application.Interfaces.Mapping;
using Catalog.Application.Interfaces.UnitOfWorks;

namespace Catalog.Application.Bases;

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