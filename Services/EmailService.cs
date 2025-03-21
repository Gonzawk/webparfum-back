// Services/EmailService.cs
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace WebParfum.API.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPass = _configuration["Email:SmtpPass"];
            var enableSSL = bool.Parse(_configuration["Email:EnableSSL"]);

            // Crear el mensaje de correo usando MimeKit
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtpUser, smtpUser));
            message.To.Add(new MailboxAddress(toEmail, toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Timeout = 30000; // Timeout de 30 segundos

                // Conectar utilizando TLS implícito (para puerto 465)
                await client.ConnectAsync(smtpHost, smtpPort, enableSSL);

                // Autenticar
                await client.AuthenticateAsync(smtpUser, smtpPass);

                // Enviar el mensaje
                await client.SendAsync(message);

                // Desconectar
                await client.DisconnectAsync(true);
            }
        }
    }
}
