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

        [HttpGet]
        public ActionResult DatVe(string  MaLichTrinh)
        {
            List<Ghe> lstGhe = db.Ghes.Where(t => t.MA_LICH_TRINH.Equals(MaLichTrinh)).ToList();
            ViewBag.TuyenDuong = db.LichTrinhs.FirstOrDefault(t => t.MA_LICH_TRINH.Equals(MaLichTrinh)).TuyenDuong.TEN_TUYEN;
            ViewBag.ThoiGianXuatPhat = db.LichTrinhs.FirstOrDefault(t => t.MA_LICH_TRINH.Equals(MaLichTrinh)).KHOI_HANH;
            ViewBag.GiaVe = db.LichTrinhs.FirstOrDefault(t => t.MA_LICH_TRINH.Equals(MaLichTrinh)).GIA_VE;
            return View(lstGhe);
        }

        [HttpPost]
        public ActionResult ThongTinVe(FormCollection c)
        {
            if (Session["khach"] == null)
            {
                return View();
            }
            if (TaoVe(c))
            {
                return RedirectToAction("Index");
            }
            return View();


        }

        public bool TaoVe(FormCollection c)
        {
            try
            {
                userAccount kh = Session["khach"] as userAccount;
                string maLichTrinh = c["maLichTrinh"];
                int soVe = db.Ves.Where(t => t.ID_KHACH_HANG.Equals(maLichTrinh)).Count() +1;
                string maVe = maLichTrinh + "VE" + soVe.ToString("D3");
                string id_KH = kh.username;
                DateTime ngayDat = DateTime.Now;
                float tongTien = float.Parse(c["TONG_TIEN"]);
                Ve ve = new Ve();
                ve.ID_VE = maVe;
                ve.ID_LICH_TRINH = maLichTrinh;
                ve.ID_KHACH_HANG = id_KH;
                ve.TONG_TIEN = tongTien;
                ve.PHUONG_THUC_THANH_TOAN = "PT1";
                ve.DIEM_DOAN = "TDC001";
                ve.DIEM_TRA = "TDC002";
                ve.QR_CODE = "abc.jpg";
                ve.NGAY_DAT_VE = DateTime.Today;
                ve.TRANG_THAI = "da_thanh_toan";
                db.Ves.InsertOnSubmit(ve);
                db.SubmitChanges();


                string vitringoi = c["arrayGhe"];
                string[] arrayGhe = vitringoi.Split(new string[] { ", " },StringSplitOptions.None);
                foreach(string item in arrayGhe)
                {
                    string maGhe = db.Ghes.FirstOrDefault(t => t.MA_LICH_TRINH.Equals(maLichTrinh) && t.VI_TRI_NGOI.Equals(item)).ID_GHE;
                    ChiTietVe ct = new ChiTietVe(){
                        ID_VE = maVe,
                        VI_TRI_NGOI = maGhe
                    };

                    db.ChiTietVes.InsertOnSubmit(ct);
                    db.SubmitChanges();
                }

                return true;
            }

            catch (Exception ex){
                throw new Exception("Lỗi: "+ex);
            }
        }

      
    }
}