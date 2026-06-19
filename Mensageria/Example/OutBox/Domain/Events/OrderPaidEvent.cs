using OutBox.Domain.Entities;

namespace OutBox.Domain.Events;

public class OrderPaidEvent
{
    public OrderPaidEvent(Order order)
    {
        Id = Guid.NewGuid();
        Order = order;
    }

    public Guid Id { get; }
    public string Name => "OrderPaidEvent";
    public Order Order { get; }
}
