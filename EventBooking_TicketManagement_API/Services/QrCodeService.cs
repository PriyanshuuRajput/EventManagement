using Applications.Interfaces.IService;
using QRCoder;

namespace EventBooking_TicketManagement_API.Services
{
    public class QrCodeService : IQrCodeService
    {

        public byte[] GenerateQr(string content)
        { 
            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(data);
            return qrCode.GetGraphic(20);
        }
    }
}
