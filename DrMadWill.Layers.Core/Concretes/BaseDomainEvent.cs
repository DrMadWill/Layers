using DrMadWill.Layers.Core.Abstractions;
using MediatR;

namespace DrMadWill.Layers.Core.Concretes;

public abstract class BaseDomainEvent : IDomainEvent
{
    public ICollection<INotification>? Events { get; private set; }

    public void AddDomainEvent(INotification @event) => (Events ??= new List<INotification>()).Add(@event);
    public void RemoveEvent(INotification @event) => Events?.Remove(@event);
    public void ClearEvent() => Events?.Clear();

}