namespace Applications.Interfaces.IService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendEmailWithQrAsync(string toEmail,string subject,string htmlBody,byte[] qrImage
    );
    }
}
