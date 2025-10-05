using Do_An.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Web.UI;
using System.Data.Entity;
using System.Web.Configuration;

namespace Do_An.Controllers
{
    public class QuanLySanPhamController : Controller
    {
        QuanLyBanSachEntities db = new QuanLyBanSachEntities();

        // GET: QuanLySanPham
        public ActionResult Index(int? page)
        {
            KhachHang kh = Session["TaiKhoan"] as KhachHang;
            if (kh != null && kh.quyen == 1)
            {
                // Tạo biến số sản phẩm trên trang
                int pageSize = 12;
                // Tạo biến số trang
                int pageNumber = (page ?? 1);
                return View(db.Saches.Where(n => n.Moi == 1).OrderBy(n => n.MaSach).ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            var chuDeList = db.ChuDes.ToList();
            ViewBag.MaChuDe = new SelectList(chuDeList, "MaChuDe", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Sach sach)
        {
            if (ModelState.IsValid)
            {
                db.Saches.Add(sach);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var chuDeList = db.ChuDes.ToList();
            ViewBag.MaChuDe = new SelectList(chuDeList, "MaChuDe", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB", sach.MaNXB);
            return View(sach);
        }

        [HttpGet]
        public ActionResult ChinhSua(int MaSach)
        {
            Sach sach = db.Saches.SingleOrDefault(n => n.MaSach == MaSach);
            if (sach == null)
            {
                return HttpNotFound(); // Thay vì Response.StatusCode = 404; return null;
            }
            var chuDeList = db.ChuDes.ToList();
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB", sach.MaNXB);
            ViewBag.MaChuDe = new SelectList(chuDeList, "MaChuDe", "TenChuDe");
            return View(sach);
        }


        [HttpPost]
        public ActionResult ChinhSua(Sach sach, HttpPostedFileBase AnhBiaFile = null)
        {
            if (ModelState.IsValid)
            {
                var existingSach = db.Saches.SingleOrDefault(n => n.MaSach == sach.MaSach);
                if (existingSach == null)
                {
                    return HttpNotFound();
                }

                if (AnhBiaFile != null && AnhBiaFile.ContentLength > 0)
                {
                    var fileName = System.IO.Path.GetFileName(AnhBiaFile.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/HinhAnhSP"), fileName);
                    AnhBiaFile.SaveAs(path);
                    existingSach.AnhBia = fileName;
                }

                existingSach.TenSach = sach.TenSach;
                existingSach.GiaBan = sach.GiaBan;
                existingSach.MoTa = sach.MoTa;
                existingSach.NgayCapNhat = sach.NgayCapNhat;
                existingSach.SoLuongTon = sach.SoLuongTon;
                existingSach.MaNXB = sach.MaNXB;
                existingSach.MaChuDe = sach.MaChuDe;
                existingSach.Moi = sach.Moi;

                db.Entry(existingSach).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var chuDeList = db.ChuDes.ToList();
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB", sach.MaNXB);
            ViewBag.MaChuDe = new SelectList(chuDeList, "MaChuDe", "TenChuDe");
            return View(sach);
        }



        [HttpGet]
        public ActionResult Xoa(int MaSach)
        {
            Sach sach = db.Saches.Find(MaSach);
            if (sach == null)
            {
                return HttpNotFound();
            }

            return View(sach); // Show the confirmation view for deletion
        }

        [HttpPost, ActionName("Xoa")]
        public ActionResult XacNhanXoa(int MaSach)
        {
            Sach sach = db.Saches.Find(MaSach);
            if (sach == null)
            {
                return HttpNotFound();
            }

            db.Saches.Remove(sach);
            db.SaveChanges();

            return RedirectToAction("Index");
        }



    }
}