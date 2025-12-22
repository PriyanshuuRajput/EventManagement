using Applications.Interfaces.IService;
using System.Net;
using System.Net.Mail;

namespace EventBooking_TicketManagement_API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"]!);
            var smtpUser = _configuration["Smtp:Username"]!;
            var smtpPass = _configuration["Smtp:Password"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true

            };

            var mail = new MailMessage
            {
                From = new MailAddress(smtpUser, "EventiGo Manager"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }
        public async Task SendEmailWithQrAsync(
    string toEmail,
    string subject,
    string htmlBody,
    byte[] qrImage)
        {
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"]!);
            var smtpUser = _configuration["Smtp:Username"]!;
            var smtpPass = _configuration["Smtp:Password"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(smtpUser, "EventiGO"),
                Subject = subject,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            // HTML view
            var view = AlternateView.CreateAlternateViewFromString(
                htmlBody, null, "text/html");

            // QR as inline image
            var qrStream = new MemoryStream(qrImage);
            var qrResource = new LinkedResource(qrStream, "image/png")
            {
                ContentId = "bookingQr"
            };

            view.LinkedResources.Add(qrResource);
            mail.AlternateViews.Add(view);

            await client.SendMailAsync(mail);
        }

    }
}
