namespace Channels.Products.Products;

public class ProductCart
{
    public Guid Id { get;  }
    public string UserId { get; } 

    private readonly List<CartItem> _cartItems = new();
    public IReadOnlyCollection<CartItem> CartItems => _cartItems;

    public ProductCart(Guid id, string userId)
    {
        IsValid(id, userId);
        Id = id;
        UserId = userId;
    }

    public void AddItem(Guid productId, string name, int quantity, decimal price)
    {
        CartItem.IsValid(productId, name, quantity, price);

        var existingItem = _cartItems.FirstOrDefault(x => x.Id == productId);

        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
            return;
        }

        _cartItems.Add(new CartItem(productId, name, quantity, price));
    }

    public static void IsValid(Guid id, string userId)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Carrinho inválido.", nameof(id));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Usuário inválido.", nameof(userId));
    }
}

public class CartItem
{
    public CartItem(Guid id, string name, int quantity, decimal price)
    {
        IsValid(id, name, quantity, price);
        Id = id;
        Name = name;
        Quantity = quantity;
        Price = price;
    }

    public Guid Id { get; }
    public string Name { get; }
    public int Quantity { get; private set; }
    public decimal Price { get; }

    public void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        Quantity += quantity;
    }

    public static void IsValid(Guid id, string name, int quantity, decimal price)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Produto inválido.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome do produto é obrigatório.");

        if (quantity <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        if (price < 0)
            throw new ArgumentException("O preço não pode ser negativo.");
    }

}