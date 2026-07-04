using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using BookStore.Models;

namespace BookStore.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("BookStore", _settings.Email));

            message.ReplyTo.Add(new MailboxAddress("BookStore", _settings.Email));

            message.To.Add(MailboxAddress.Parse(to));

            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = htmlMessage
            };

            using var smtp = new SmtpClient();

            smtp.Connected += (s, e) =>
            {
                Console.WriteLine("SMTP CONNECTED");
            };

            smtp.MessageSent += (s, e) =>
            {
                Console.WriteLine("SMTP MESSAGE SENT");
            };

            await smtp.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.SslOnConnect
            );

            await smtp.AuthenticateAsync(
                _settings.Email,
                _settings.Password
            );

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}