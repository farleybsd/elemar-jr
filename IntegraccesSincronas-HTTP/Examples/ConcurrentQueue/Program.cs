using ConcurrentQueue;
using ConcurrentQueue.BackgroundServices;
using ConcurrentQueue.Emails;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<
    IBackgroundTaskQueue,
    BackgroundTaskQueue>();

builder.Services.AddHostedService<
    QueuedProcessorBackgroundService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddControllers();
builder.Services.AddLogging();
var app = builder.Build();

// Mapeia uma rota HTTP POST para o endereço "/enqueue-email".
app.MapPost("enqueue-email", ([FromBody] EmailRequest emailRequest,
                             IBackgroundTaskQueue _taskQueue,
                             ILogger<EmailJob> logger) =>
{
    // Adiciona uma função assíncrona à fila de tarefas em segundo plano.
    _taskQueue.QueueBackgroundWorkItem(async (serviceProvider, token) =>
    {
        // Obtém uma instância de IEmailService do escopo criado pelo background service.
        var emailService = serviceProvider.GetRequiredService<IEmailService>();

        logger.LogInformation("Email job started");

        try
        {
            // Envia o e-mail usando os dados recebidos na requisição.
            await emailService.SendEmailAsync(emailRequest.To, emailRequest.Subject, emailRequest.Body, token);
            logger.LogInformation("Email job completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while sending email.");
        }
    });

    return Results.Accepted(
       value: new
       {
           Message = "Email job has been queued."
       });

});

app.Run();

public class EmailRequest
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}
public sealed class EmailJob;



//{
//    "to": "example@example.com",
//    "subject": "Test Email",
//    "body": "This is a test email body."
//}