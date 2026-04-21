namespace OCP.MeiosPagamento;

public sealed class PixProcessor : IPaymentProcessor
{
    public PaymentMethod Method => PaymentMethod.Pix;

    public Task Process(PaymentRequest request)
    {
        // GERA QR CODE, REGISTRA COBRANÇA, WEBHOOK ETC.
        return Task.CompletedTask;
    }
}
