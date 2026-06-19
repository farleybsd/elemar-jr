using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OutBox.Domain.Entities;

public class Order
{
    private List<OrderItem> _items = [];
    public Guid Id { get; private set; }
    public OrderStatus Status { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public OrderItem GetItemWithHighestDiscount()
    {
        var item = Items
                        .OrderByDescending(item => item.Discount)
                        .FirstOrDefault();
        return item;
    }

    internal void AddItem(OrderItem item)
    {
        _items.Add(item);
    }

    internal void SetAsPaid()
    {
        Status = OrderStatus.Paid;
    }

}
