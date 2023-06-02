namespace Genesis.App.Contract.Common.Services
{
    public record EmailMessage(string Subject, string Text, string ReceiverEmail, string ReceiverName);

    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
