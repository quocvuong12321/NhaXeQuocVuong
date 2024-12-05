using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhaXe_QuocVuong.Models;

namespace NhaXe_QuocVuong.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        NhaXeDataContext db = new NhaXeDataContext();
        public ActionResult Index()
        {
            List<TuyenDuong> lst = db.TuyenDuongs.Take(3).ToList();
            var today = DateTime.Today;
            //var upcomingSchedules = db.LichTrinhs
            //.Where(schedule => schedule.KHOI_HANH.Date >= today
            //               && schedule.KHOI_HANH.Date <= today.AddDays(7))
            //.OrderBy(schedule => schedule.KHOI_HANH)
            //.ToList();

            //// Truyền dữ liệu vào ViewBag để sử dụng trong View
            //ViewBag.UpcomingSchedules = upcomingSchedules;

            // Lấy tất cả các lịch trình
            var allSchedules = db.LichTrinhs
                .OrderBy(schedule => schedule.KHOI_HANH) // Sắp xếp theo thời gian khởi hành
                .Take(3)
                .Select(l => new LichTrinh_Home
                {
                    MaLichTrinh = l.MA_LICH_TRINH,
                    TenTuyenDuong = l.TuyenDuong.TEN_TUYEN,
                    GiaVe = (float)l.GIA_VE,
                    NgayKhoiHanh = l.KHOI_HANH
                })
        .ToList();

            // Truyền dữ liệu vào ViewBag để sử dụng trong View
            ViewBag.AllSchedules = allSchedules;
            return View(lst);
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult LienHe()
        {
            return View();
        }
    }
}