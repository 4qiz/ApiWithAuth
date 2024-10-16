using MailKit.Net.Smtp;
using MimeKit;

namespace EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration configuration;

        public EmailSender(EmailConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendEmail(EmailMessage emailMessage)
        {
            var message = CreateEmailMessage(emailMessage);
            await Send(message);
        }

        private async Task Send(MimeMessage message)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(configuration.SmtpServer, configuration.Port, false);

                if (!string.IsNullOrEmpty(configuration.UserName) && !string.IsNullOrEmpty(configuration.Password))
                {
                    await client.AuthenticateAsync(configuration.UserName, configuration.Password);
                }

                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки email: {ex.Message}");
                throw;  // Или повторная генерация исключения
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }

        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("", configuration.From));
            mimeMessage.To.AddRange(message.To.Select(email => new MailboxAddress("", email)));
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return mimeMessage;
        }
    }
}
