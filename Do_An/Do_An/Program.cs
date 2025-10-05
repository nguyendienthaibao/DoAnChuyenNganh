using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Do_An
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Nội dung của mã QR
            string qrContent = "https://www.example.com";

            // Khởi tạo đối tượng QRCodeGenerator
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);

            // Tạo mã QR
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            // Đường dẫn thư mục mới
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "QRCodeImages");

            // Kiểm tra và tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Console.WriteLine($"Đã tạo thư mục mới tại: {folderPath}");
            }

            // Đường dẫn file ảnh mã QR
            string filePath = Path.Combine(folderPath, "qr_code.png");

            // Lưu mã QR vào file ảnh
            qrCodeImage.Save(filePath, ImageFormat.Png);

            Console.WriteLine($"Mã QR đã được tạo và lưu tại: {filePath}");
        }
    }
}