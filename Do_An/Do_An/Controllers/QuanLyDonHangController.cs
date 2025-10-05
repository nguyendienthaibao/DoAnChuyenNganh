using Do_An.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity;

namespace Do_An.Controllers
{
    public class QuanLyDonHangController : Controller
    {
        private QuanLyBanSachEntities db = new QuanLyBanSachEntities();

        // Hiển thị danh sách đơn hàng cho admin
        public ActionResult Index()
        {
            var donHangs = db.DonHangs.ToList();
            return View(donHangs);
        }

        // Hiển thị chi tiết đơn hàng theo MaDonHang
        public ActionResult ChiTietDonHang(int maDonHang)
        {
            // Lấy chi tiết đơn hàng theo MaDonHang
            var chiTietDonHangs = db.ChiTietDonHangs.Where(c => c.MaDonHang == maDonHang).ToList();

            if (chiTietDonHangs == null || chiTietDonHangs.Count == 0)
            {
                return HttpNotFound();
            }

            return View(chiTietDonHangs);
        }
    }

}
