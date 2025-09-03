using EShop.Shared.Dtos.BasesResponses;
using MediatR;

namespace Catalog.Application.Features.CategoryFeature.Commands;

public class CreateCategoryCommandRequest : IRequest<ResponseDto<CreateCategoryCommandResponse>>
{
    public string Name { get; set; }

    public CreateCategoryCommandRequest(string name)
    {
        this.Name = name;
    }
}