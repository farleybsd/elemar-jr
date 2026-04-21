namespace OCP;

public sealed class PaymentRequest
{
    public PaymentMethod Method { get; init; }

    public decimal Amount { get; init; }

    public string Currency { get; init; } = "BRL";

    public string CustomerId { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? CreditCardNumber { get; init; }

    public string? PixKey { get; init; }

    public DateTime RequestedAt { get; init; } = DateTime.UtcNow;
}
