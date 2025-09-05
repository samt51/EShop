using EShop.Shared.Dtos.BasesResponses;
using Newtonsoft.Json;

namespace Payment.Api.Middleware.Exceptions;

public class ExceptionModel
{
    public ResponseDto<ExceptionModel> Response { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}