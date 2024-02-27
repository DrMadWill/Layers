using DrMadWill.Layers.Core.Abstractions;
using MediatR;

namespace DrMadWill.Layers.Core.Concretes;

public abstract class BaseDomainEvent : IDomainEvent
{
    public ICollection<INotification>? Events { get; private set; }

    public void AddDomainEvent(INotification @event)
    {
        Events ??= new List<INotification>();
        Events.Add(@event);
    }
}