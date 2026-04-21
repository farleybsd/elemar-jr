namespace OCP.MeiosPagamento;

public sealed class CreditCardProcessor : IPaymentProcessor
{
    public PaymentMethod Method => PaymentMethod.CreditCard;

    public Task Process(PaymentRequest request)
    {
        // INTEGRA COM ADQUIRENTE, ANTIFRAUDE ETC.
        return Task.CompletedTask;
    }
}
