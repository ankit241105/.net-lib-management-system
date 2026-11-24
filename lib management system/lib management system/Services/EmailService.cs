using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using lib_management_system.Common;

namespace lib_management_system.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                _logger.LogWarning("Email not sent: no recipient address.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_settings.Host) || string.IsNullOrWhiteSpace(_settings.User))
            {
                _logger.LogInformation(
                    "[EMAIL log-mode] To: {To} | Subject: {Subject}\n{Body}", toEmail, subject, htmlBody);
                return;
            }

            var fromAddress = string.IsNullOrWhiteSpace(_settings.From) ? _settings.User : _settings.From;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, fromAddress));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            var socketOptions = _settings.UseStartTls
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.Auto;

            await client.ConnectAsync(_settings.Host, _settings.Port, socketOptions);
            if (!string.IsNullOrWhiteSpace(_settings.User))
                await client.AuthenticateAsync(_settings.User, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent to {To}: {Subject}", toEmail, subject);
        }
    }
}
