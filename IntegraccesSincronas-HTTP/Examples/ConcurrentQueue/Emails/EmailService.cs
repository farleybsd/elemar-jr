namespace ConcurrentQueue.Emails;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken token);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken token)
    {
        _logger.LogInformation($"Sending email to {to} with subject {subject}.");

        // Simulate email sending delay
        await Task.Delay(2000, token);

        _logger.LogInformation($"Email to {to} sent successfully.");
    }
}
