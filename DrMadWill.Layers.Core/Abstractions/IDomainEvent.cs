using MediatR;

namespace DrMadWill.Layers.Core.Abstractions;

public interface IDomainEvent
{
    ICollection<INotification>? Events { get; }

    void AddDomainEvent(INotification @event);
}