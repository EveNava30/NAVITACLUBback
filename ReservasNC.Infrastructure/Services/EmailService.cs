using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using ReservasNC.Domain.Interfaces.Services;

namespace ReservasNC.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _fromEmail;
        private readonly string _fromPassword;

        public EmailService(IConfiguration configuration)
        {
            _smtpHost = configuration["EmailSettings:SmtpHost"];
            _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);
            _fromEmail = configuration["EmailSettings:FromEmail"];
            _fromPassword = configuration["EmailSettings:FromPassword"];
        }

        public async Task SendResetEmailAsync(string emailDestino, string token)
        {
            var resetLink = $"http://127.0.0.1:5500/index.html?token={token}";

            using var client = new SmtpClient(_smtpHost, _smtpPort);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(_fromEmail, _fromPassword);

            var mensaje = new MailMessage
            {
                From = new MailAddress(_fromEmail, "Soporte ReservasNC"),
                Subject = "Recuperación de contraseña",
                Body = $@"
Hola, solicitaste recuperar tu contraseña.
Haz clic en el siguiente enlace para restablecerla:

{resetLink}

Este enlace expirará en 1 hora.
Si no solicitaste esto, ignora este mensaje.
                ",
                IsBodyHtml = false
            };

            mensaje.To.Add(emailDestino);
            await client.SendMailAsync(mensaje);
        }
    }
}
