using Basket.Application.Dtos.BasketDtos;
using EShop.Shared.Dtos.BasesResponses;

namespace Basket.Application.Interfaces.Repositories;

public interface IBasketRepository
{
    Task<ResponseDto<BasketResponseDto>> GetAsync(string userId, CancellationToken ct = default);
    Task<bool> UpsertAsync(Domain.Entities.Basket basket, TimeSpan? ttl = null, CancellationToken ct = default);
    Task<bool> DeleteAsync(string userId, CancellationToken ct = default);
}