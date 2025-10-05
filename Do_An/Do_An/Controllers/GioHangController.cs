using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Text;
using Do_An.Models;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace Do_An.Controllers
{
    public class GioHangController : Controller
    {
        QuanLyBanSachEntities db = new QuanLyBanSachEntities();
        //Một đống controller về
        #region Giỏ hàng

        // GET: GioHang
        public List<GioHang> LayGioHang()
        {
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if(listGioHang == null)
            {
                //Nếu giỏ hàng chưa tồn tại thì tiến hành khởi tạo list giỏ hàng(sessionGioHang)
                listGioHang = new List<GioHang>();
                Session["GioHang"] = listGioHang;
            }
            return listGioHang;
        }

        //Thêm giỏ hàng
        public ActionResult ThemGioHang(int iMaSach,string strURL)
        {
            Sach sach = db.Saches.SingleOrDefault(n => n.MaSach == iMaSach);
            if(sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy ra session giỏ hàng
            List<GioHang> listGioHang = LayGioHang();
            //Kiểm tra sách nếu đã tồn tại hay không trong session chưa
            GioHang sp = listGioHang.Find(n=>n.iMaSach == iMaSach);
            if(sp == null)
            {
                sp = new GioHang(iMaSach);
                //Add sản phẩm mới thêm vào list
                listGioHang.Add(sp);
                return Redirect(strURL);
            }
            else
            {
                sp.iSoLuong++;
                return Redirect(strURL);
            }
        }
        //Cập nhập giỏ hàng
        public ActionResult CapNhapGioHang(int iMaSP,FormCollection f)
        {
            //Kiểm tra ma sp
            Sach sach = db.Saches.SingleOrDefault(n=>n.MaSach == iMaSP);
            //Nếu get sai máp thì trả về trang 404
            if(sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng từ session
            List<GioHang> listGioHang = LayGioHang();
            //Kiểm tra sp có tồn tại trong  trong session Giỏ hàng
            GioHang sp = listGioHang.SingleOrDefault(n => n.iMaSach == iMaSP);
            //Nếu mà tồn tại thì cho sửa số lượng
            if (sp != null)
            {
                sp.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");
        }

        //Xóa giỏ hàng
        public ActionResult XoaGioHang(int iMaSP)
        {
            //Kiểm tra ma sp
            Sach sach = db.Saches.SingleOrDefault(n => n.MaSach == iMaSP);
            //Nếu get sai máp thì trả về trang 404
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng từ session
            List<GioHang> listGioHang = LayGioHang();
            GioHang sp = listGioHang.SingleOrDefault(n => n.iMaSach == iMaSP);
            //Nếu mà tồn tại thì cho sửa số lượng
            if (sp != null)
            {
                listGioHang.RemoveAll(n => n.iMaSach == iMaSP);
            }
            if (listGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("GioHang", "GioHang"); // Chuyển hướng về trang Giỏ hàng
        }


        //Xây dựng trang giỏ hàng
        public ActionResult GioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> listGioHang = LayGioHang();
            return View(listGioHang);
        }
        //Tính tổng số lượng và tổng tiền

        //Tính tổng số lượng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if(listGioHang != null)
            {
                iTongSoLuong = listGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }

        //Tính tổng thành tiền
        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if(listGioHang != null)
            {
                dTongTien = listGioHang.Sum(n => n.ThanhTien);
            }
            return dTongTien;
        }

        //Tạo paritial giỏ hàng
        public ActionResult GioHangPartial()
        {
            if(TongSoLuong() == 0)
            {
                return PartialView(); 
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }

        //Xây dụng view cho người dùng chỉnh sửa giỏ hàng
        public ActionResult SuaGioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> listGioHang = LayGioHang();
            return View(listGioHang);
        }
        #endregion
        
        //Một controller về 
        #region Đặt hàng
        //Xây dựng chưc năng đặt hàng
        [HttpPost]
        public ActionResult DatHang()
        {
            //Kiểm tra đang đăng nhập
            if (Session["TaiKhoan"] == null|| Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            //Kiểm tra giỏ hàng
            if (Session["GioHang"] == null)
            {
                RedirectToAction("Index", "Home");
            }

            //Thêm đơn hàng vào giỏ hàng
            DonHang ddh = new DonHang();
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            List<GioHang> gh = LayGioHang();
            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            db.DonHangs.Add(ddh);

            //Thêm chi tiết đơn đặt hàng
            foreach(var item in gh)
            {
                ChiTietDonHang CTDH = new ChiTietDonHang();
                CTDH.MaDonHang = ddh.MaDonHang;
                CTDH.MaSach = item.iMaSach;
                CTDH.SoLuong = item.iSoLuong;
                CTDH.DonGia = item.dDonGia.ToString();

                db.ChiTietDonHangs.Add(CTDH);
            }

            db.SaveChanges();
            return RedirectToAction("Index","Home");
        }

        #endregion



        // Action hiển thị trang thanh toán
        [HttpGet]
        public ActionResult ThanhToan()
        {
            return View();
        }

        // Action để xử lý dữ liệu sau khi người dùng nhập thông tin và hiển thị mã QR
        [HttpPost]
        public ActionResult ThanhToan(string hoTen, string diaChi, string sdt)
        {
            // Nội dung cần mã hóa vào mã QR (ví dụ: thông tin đơn hàng)
            string qrContent = $"Họ tên: {hoTen}\nĐịa chỉ: {diaChi}\nSố điện thoại: {sdt}";

            // Tạo mã QR
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            // Chuyển mã QR thành hình ảnh
            using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult ThanhToanWithScreenshot(string hoTen, string email, string sdt, string diaChi, string phuongThucThanhToan, HttpPostedFileBase paymentScreenshot)
        {
            // Xử lý đơn hàng: lưu thông tin đơn hàng vào cơ sở dữ liệu

            // Gửi email thông báo
            SendOrderConfirmationEmail(hoTen, email, phuongThucThanhToan);

            // Trả về view thành công hoặc redirect tới trang xác nhận thành công
            TempData["SuccessMessage"] = "Đơn hàng của bạn đã được đặt thành công và thông báo đã được gửi tới email của bạn.";
            return RedirectToAction("OrderConfirmation");
        }

        private void SendOrderConfirmationEmail(string hoTen, string email, string phuongThucThanhToan)
        {
            string subject = "Xác nhận đơn hàng từ VietQRPaymentAPI";
            string body = $"Xin chào {hoTen},\n\n"
                        + $"Cảm ơn bạn đã đặt hàng với phương thức thanh toán: {phuongThucThanhToan}.\n"
                        + $"Chúng tôi sẽ xử lý đơn hàng của bạn sớm nhất có thể.\n\n"
                        + "Trân trọng,\n"
                        + "Đội ngũ VietQRPaymentAPI";

            try
            {
                var smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential("nguynbao513@gmail.com", "yqzvzndmnliohihl")
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("nguynbao513@gmail.com"),
                    Subject = "Test Email",
                    Body = "This is a test email.",
                    IsBodyHtml = false,
                };

                mailMessage.To.Add("recipient@example.com");
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine($"Gửi email thất bại: {ex.Message}");
            }


            
        }

        public ActionResult OrderConfirmation()
        {
            ViewBag.Message = TempData["SuccessMessage"];
            return View();
        }

    }
}