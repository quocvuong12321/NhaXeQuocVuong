using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NhaXe_QuocVuong.Models;
namespace NhaXe_QuocVuong.Controllers
{
    public class LichTrinhController : Controller
    {
        // GET: LichTrinh
        NhaXeDataContext db = new NhaXeDataContext();
        public ActionResult Index()
        {
            List<LichTrinhViewModel> model = new List<LichTrinhViewModel>();
            List< LichTrinh > lst = db.LichTrinhs.ToList();
            foreach(var item in lst)
            {
                LichTrinhViewModel ltvm = new LichTrinhViewModel
                {
                    LichTrinh = item,
                    SoChoConLai = LaySoChoCon(item.MA_LICH_TRINH)
                };
                model.Add(ltvm);
            }


            return View(model);
        }

        private int LaySoChoCon(string MaLichTrinh)
        {
            LichTrinh lt = db.LichTrinhs.FirstOrDefault(t => t.MA_LICH_TRINH.Equals(MaLichTrinh));
            return lt.Ghes.Count(t => t.TINH_TRANG == false);
        }

        public ActionResult DatVe(string  MaLichTrinh)
        {
            //LichTrinh lt = db.LichTrinhs.FirstOrDefault(t => t.MA_LICH_TRINH.Equals(MaLichTrinh));

            List<Ghe> lstGhe = db.Ghes.Where(t => t.MA_LICH_TRINH.Equals(MaLichTrinh)).ToList();

            return PartialView(lstGhe);
        }

        public ActionResult HienThiDSGhe(int SoGhe)
        {
            try
            {
                // Lấy danh sách ghế dựa trên số ghế
                var danhSachGhe = LayDanhSachGhe(SoGhe);

                // Trả về partial view với danh sách ghế
                return PartialView("_DanhSachGhe", danhSachGhe);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        private List<string> LayDanhSachGhe(int soGhe)
        {
            // Lấy danh sách ghế từ cơ sở dữ liệu hoặc nguồn khác
            var danhSachGhe = new List<string>();
            for (int i = 1; i <= soGhe; i++)
            {
                danhSachGhe.Add($"Ghế {i}");
            }
            return danhSachGhe;
        }
    }
}