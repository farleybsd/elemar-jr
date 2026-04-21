using OCP;
using OCP.MeiosPagamento;
using OCP.Servicos;

var processors = new List<IPaymentProcessor>
{
    new CreditCardProcessor(),
    new PixProcessor()
};

var paymentService = new PaymentService(processors);

await paymentService.Pay(new PaymentRequest
{
    Method = PaymentMethod.Pix,
    Amount = 150.00m,
    CustomerId = "123",
    PixKey = "email@teste.com",
    Description = "Pagamento do pedido"
});