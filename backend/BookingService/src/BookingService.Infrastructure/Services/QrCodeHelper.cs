using QRCoder;
using System.Drawing.Imaging;

namespace BookingService.Infrastructure.Services
{
    /// <summary>
    /// Helper utility for generating PNG QR code images from text inputs.
    /// </summary>
    public class QrCodeHelper
    {
        /// <summary>
        /// Generates a PNG byte array representing the QR code for the given text.
        /// </summary>
        /// <param name="qrText">The raw text or payload to encode into the QR code.</param>
        /// <returns>A byte array containing the PNG image representation.</returns>
        public static byte[] GenerateQrCode(string qrText)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrCodeData);
            using var qrCodeImage = qrCode.GetGraphic(20);
            using var stream = new MemoryStream();
            qrCodeImage.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
    }
}