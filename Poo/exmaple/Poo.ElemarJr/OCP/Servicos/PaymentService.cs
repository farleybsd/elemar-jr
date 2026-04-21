namespace OCP.Servicos;

public sealed class PaymentService
{
    private readonly IReadOnlyDictionary<PaymentMethod, IPaymentProcessor> _processors;

    public PaymentService(IEnumerable<IPaymentProcessor> processors)
    {
        _processors = processors.ToDictionary(p => p.Method);
    }

    public Task Pay(PaymentRequest request)
    {
        if (!_processors.TryGetValue(request.Method, out var processor))
            throw new NotSupportedException($"Payment method {request.Method} not supported.");

        return processor.Process(request);
    }
}
