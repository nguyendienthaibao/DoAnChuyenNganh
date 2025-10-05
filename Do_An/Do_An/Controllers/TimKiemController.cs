using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Do_An.Models;
using PagedList;

namespace Do_An.Controllers
{
    public class TimKiemController : Controller
    {
        private QuanLyBanSachEntities db = new QuanLyBanSachEntities();

        [HttpPost]
        public ActionResult KetQuaTimKiem(FormCollection f,int? page)
        {
            // Thực hiện tìm kiếm sách dựa trên từ khóa txtTimKiem
            string sTuKhoa = f["txtTimKiem"].ToString();
            List<Sach> sachList = db.Saches.Where(n => n.TenSach.Contains(sTuKhoa)).ToList();
            int pageNumber = (page ?? 1);
            int pageSize = 12;
            if (sachList.Count == 0)
            {
                ViewBag.ThongBao = "Không tìm thấy";
                return View(db.Saches.OrderBy(n=>n.TenSach).ToPagedList(pageNumber,pageSize));
            }
            // Trả về view chứa kết quả tìm kiếm
            return View(sachList.OrderBy(n=>n.TenSach).ToPagedList(pageNumber,pageSize));
        }
    }

}