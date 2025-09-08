using EShop.Shared.Dtos.BasesResponses;
using Newtonsoft.Json;

namespace Payment.Application.Middleware.Exceptions;

public class ExceptionModel
{
    public ResponseDto<ExceptionModel> Response { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}