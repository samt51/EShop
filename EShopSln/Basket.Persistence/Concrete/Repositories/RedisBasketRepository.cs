using System.Text.Json;
using Basket.Application.Dtos.BasketDtos;
using Basket.Application.Interfaces.Repositories;
using EShop.Shared.Dtos.BasesResponses;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Basket.Persistence.Concrete.Repositories;

public class RedisBasketRepository : IBasketRepository
{
    private readonly IConnectionMultiplexer _mux;
    private readonly IDatabase _db;
    private readonly string _ns;
    private readonly TimeSpan _defaultTtl;
    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true,                
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };
    public RedisBasketRepository(IConnectionMultiplexer mux, IConfiguration cfg)
    {
        _mux = mux;
        _db = mux.GetDatabase();
        _ns = cfg.GetSection("Redis")["InstanceName"] ?? "ecom:basket:";
        var ttlMin = int.TryParse(cfg.GetSection("Redis")["DefaultTtlMinutes"], out var m) ? m : 120;
        _defaultTtl = TimeSpan.FromMinutes(ttlMin);
    }

    private string Key(string userId) => $"{_ns}{userId}";
  public async Task<ResponseDto<BasketResponseDto>> GetAsync(string userId, CancellationToken ct = default)
    {
        var val = await _db.StringGetAsync(Key(userId));
        if (val.IsNullOrEmpty) return new ResponseDto<BasketResponseDto>();
        
        var basket = JsonSerializer.Deserialize<BasketResponseDto>(val!, _json);

        return new ResponseDto<BasketResponseDto>().Success(basket);
    }

    public async Task<bool> UpsertAsync(Domain.Entities.Basket basket, TimeSpan? ttl = null, CancellationToken ct = default)
    {
        basket.ModifyDate = DateTime.UtcNow;
        var payload = JsonSerializer.Serialize(basket, _json);
        return await _db.StringSetAsync(Key(basket.UserId.ToString()), payload, ttl ?? _defaultTtl);
    }

    public async Task<bool> DeleteAsync(string userId, CancellationToken ct = default)
        => await _db.KeyDeleteAsync(Key(userId));
}