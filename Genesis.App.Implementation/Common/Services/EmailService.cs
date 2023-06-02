using Genesis.App.Contract.Common.Services;
using Genesis.App.Implementation.Utils;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Genesis.App.Implementation.Common.Services
{
    public class EmailService : IEmailService
    {
        private readonly string password = Environment.GetEnvironmentVariable("GENESIS_SENDER_PASSWORD");
        private readonly EmailSettings settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            this.settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var email = CreateEmailMessage(message);

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(settings.SmtpServer, settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(settings.Email, password);
            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }

        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(settings.Name, settings.Email));
            mimeMessage.To.Add(new MailboxAddress(message.ReceiverName, message.ReceiverEmail));
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = new TextPart(TextFormat.Html) { Text = message.Text };

            return mimeMessage;
        }
    }
}
