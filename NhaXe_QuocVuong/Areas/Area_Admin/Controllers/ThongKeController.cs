using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhaXe_QuocVuong.Areas.Area_Admin.Data;
using NhaXe_QuocVuong.Models;

namespace NhaXe_QuocVuong.Areas.Area_Admin.Controllers
{
    public class ThongKeController : Controller
    {
        // GET: Area_Admin/ThongKe
        NhaXeDataContext db = new NhaXeDataContext();
        public ActionResult Index()
        {
            var ve = db.Ves.Where(v => v.TRANG_THAI == "da_su_dung").ToList();
            var doanhThuTheoThang = new decimal[12];
            List<ves> tong = new List<ves>();
            foreach (var v in ve)
            {
                int thang = v.NGAY_DAT_VE.Month - 1;
                decimal tongGia = decimal.Parse(v.TONG_TIEN.ToString());
                doanhThuTheoThang[thang] += tongGia;
                tong.Add(new ves(v.ID_VE, tongGia, v.NGAY_DAT_VE));
            }
            ViewBag.ListTongVe = tong;
            ViewBag.TongGia = tong.Sum(t => t.TONG_TIEN);
            ViewBag.DoanhThu = doanhThuTheoThang;
            return View();
        }
    }
}