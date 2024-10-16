
namespace EmailService
{
    public class EmailMessage
    {
        public EmailMessage(IEnumerable<string> to, string subject, string content)
        {
            To = to;
            Subject = subject;
            Content = content;
        }

        public IEnumerable<string> To { get; }
        public string Subject { get; }
        public string Content { get; }
    }
}
