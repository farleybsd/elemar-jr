using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutBox;
using OutBox.Application;
using OutBox.Backgrounds;
using OutBox.Data;
using OutBox.Domain.Entities;
using OutBox.Domain.Services;
using OutBox.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddOptions<RabbitMqOptions>()
    .BindConfiguration(RabbitMqOptions.SectionName)
    .Validate(
        options => Uri.TryCreate(
            options.Uri,
            UriKind.Absolute,
            out _),
        "A URI do RabbitMQ é inválida.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.QueueName),
        "O nome da fila é obrigatório.")
    .ValidateOnStart();

builder.Services.AddScoped<OrderService>();

builder.Services.AddDbContext<OutboxDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Database"));
});

// 1. Aplica as migrations.
builder.Services.AddHostedService<DatabaseMigrationHostedService>();

// 2. Cria a instância única do produtor.
builder.Services.AddSingleton<RabbitMqMessageProducer>();

builder.Services.AddSingleton<IBrokerPublisherService>(provider =>
    provider.GetRequiredService<RabbitMqMessageProducer>());

// 3. Inicializa a conexão com o RabbitMQ.
builder.Services.AddHostedService(provider =>
    provider.GetRequiredService<RabbitMqMessageProducer>());

// 4. Inicia o processamento do Outbox.
builder.Services.AddHostedService<OutboxPublisherBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/order", async ([FromBody] OrderRequest request, [FromServices] OrderService orderService, [FromServices] ILogger<Program> logger) =>
{
    var order = new Order();

    foreach (var item in request.Items)
    {
        order.AddItem(new OrderItem(item.ProductName, item.UnitPrice, item.Discount, item.Units));
    }

    await orderService.AddOrderAsync(order);

    logger.LogInformation("Order with Id {OrderId} created", order.Id);

    return order;
})
.WithName("CreateOrder");

app.MapPut("/order/{orderId}/pay", async (Guid orderId, [FromServices] OrderService orderService) =>
{
    var order = await orderService.SetOrderAsPaid(orderId);

    return order;
})
.WithName("SetOrderAsPaid");

app.Run();


