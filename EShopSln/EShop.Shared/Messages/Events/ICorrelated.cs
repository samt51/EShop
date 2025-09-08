namespace EShop.Shared.Messages.Events;


public interface ICorrelated
{
    Guid CorrelationId { get; }
}
