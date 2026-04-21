namespace OCP
{
    public interface IPaymentProcessor
    {
        PaymentMethod Method { get; }

        Task Process(PaymentRequest request);
    }
}
