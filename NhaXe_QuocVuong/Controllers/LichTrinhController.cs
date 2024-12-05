using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NhaXe_QuocVuong.Models;
using QRCoder;
using System.IO;
using System.Drawing.Imaging;

namespace NhaXe_QuocVuong.Controllers
{
    public class LichTrinhController : Controller
    {
        // GET: LichTrinh
        NhaXeDataContext db = new NhaXeDataContext();
        public ActionResult Index()
        {
            List<LichTrinh_display> model = new List<LichTrinh_display>();
            List< LichTrinh > lst = db.LichTrinhs.ToList();
            foreach(var item in lst)
            {
                LichTrinh_display ltvm = new LichTrinh_display
                {
                    LichTrinh = item,
                    SoChoConLai = LaySoChoCon(item.MA_LICH_TRINH)
                };
                model.Add(ltvm);
            }

            ViewBag.DiaDiem = db.DiaDiems.ToList();

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
            if (Session["khach"] == null)
            {
                Session["ReturnUrl"] = "~/LichTrinh/DatVe/?MaLichTrinh="+MaLichTrinh;
                return RedirectToAction("Login_Nguoidung", "DangNhap");
            }
            List<Ghe> lstGhe = db.Ghes.Where(t => t.MA_LICH_TRINH.Equals(MaLichTrinh)).ToList();
            ViewBag.TuyenDuong = db.LichTrinhs.FirstOrDefault(t => t.MA_LICH_TRINH.Equals(MaLichTrinh)).TuyenDuong.TEN_TUYEN;
            int noidi = db.LichTrinhs.FirstOrDefault(t => t.MA_LICH_TRINH.Equals(MaLichTrinh)).TuyenDuong.DiaDiem.ID_DIADIEM;
            int noiden = db.LichTrinhs.FirstOrDefault(t => t.MA_LICH_TRINH.Equals(MaLichTrinh)).TuyenDuong.DiaDiem1.ID_DIADIEM;
            ViewBag.ThoiGianXuatPhat = db.LichTrinhs.FirstOrDefault(t => t.MA_LICH_TRINH.Equals(MaLichTrinh)).KHOI_HANH;
            ViewBag.GiaVe = db.LichTrinhs.FirstOrDefault(t => t.MA_LICH_TRINH.Equals(MaLichTrinh)).GIA_VE;
            ViewBag.TramDungDi = db.TramDungChans.Where(t=>t.ID_DIADIEM==noidi).ToList();
            ViewBag.TramDungDen = db.TramDungChans.Where(t=>t.ID_DIADIEM==noiden).ToList();
            ViewBag.PTThanhToan = db.PHUONG_THUC_THANH_TOANs.ToList();
            ViewBag.KH = Session["khach"] as userAccount;
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
                return RedirectToAction("ThongBaoDaDat");
            }
            return View();
        }

        public bool TaoVe(FormCollection c)
        {
            try
            {
                userAccount kh = Session["khach"] as userAccount;
                string maLichTrinh = c["maLichTrinh"];
                int soVe = db.Ves.Where(t => t.ID_LICH_TRINH.Equals(maLichTrinh)).Count() +1;
                string maVe = maLichTrinh + "VE" + soVe.ToString("D3");
                string id_KH = kh.username;
                DateTime ngayDat = DateTime.Now;
                string diemdon= c["diemdon"];
                string diemtra = c["diemtra"];
                string pttt =c["phuongthuc"];
                float tongTien = float.Parse(c["TONG_TIEN"]);
                Ve ve = new Ve();
                ve.ID_VE = maVe;
                ve.ID_LICH_TRINH = maLichTrinh;
                ve.ID_KHACH_HANG = id_KH;
                ve.TONG_TIEN = tongTien;
                ve.PHUONG_THUC_THANH_TOAN = pttt;
                ve.DIEM_DOAN = diemdon;
                ve.DIEM_TRA = diemtra;
                ve.NGAY_DAT_VE = DateTime.Today;
                ve.TRANG_THAI = "da_thanh_toan";

                string tentuyenduong = db.LichTrinhs.First(t => t.MA_LICH_TRINH.Equals(maLichTrinh)).TuyenDuong.TEN_TUYEN;
                string khachhang = db.KhachHangs.First(t => t.USERNAME.Equals(id_KH)).TEN_KHACH_HANG;
                string qrContent = $"Vé: {maVe}\nLịch trình: {tentuyenduong}\nKhách hàng: {khachhang}-{ve.ID_KHACH_HANG}";
                string qrpath = GenerateQRCode(qrContent, maVe);
                ve.QR_CODE = qrpath;
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

        private string GenerateQRCode(string content, string fileName)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new QRCode(qrCodeData))
                {
                    using (var bitmap = qrCode.GetGraphic(20))
                    {
                        string folderPath = Server.MapPath("~/img");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        string filePath = Path.Combine(folderPath, $"{fileName}.png");
                        bitmap.Save(filePath, ImageFormat.Png);
                        return $"/img/{fileName}.png"; // Đường dẫn lưu trong DB
                    }
                }
            }
        }


        public ActionResult SapXep(string Sort)
        {
            List<LichTrinh_display> model = new List<LichTrinh_display>();
            List<LichTrinh> lst = db.LichTrinhs.ToList();
            foreach (var item in lst)
            {
                LichTrinh_display ltvm = new LichTrinh_display
                {
                    LichTrinh = item,
                    SoChoConLai = LaySoChoCon(item.MA_LICH_TRINH)
                };
                model.Add(ltvm);
            }
            switch (Sort)
            {
                case "Giờ đi sớm nhất":
                    model = model.OrderBy(m => m.LichTrinh.KHOI_HANH).ToList();
                    break;
                case "Giờ đi muộn nhất":
                    model = model.OrderByDescending(m => m.LichTrinh.KHOI_HANH).ToList();
                    break;
                case "Giá tăng dần":
                    model = model.OrderBy(m => m.LichTrinh.GIA_VE).ToList();
                    break;
                case "Giá giảm dần":
                    model = model.OrderByDescending(m => m.LichTrinh.GIA_VE).ToList();
                    break;
                default:
                    model = model.ToList();// Sắp xếp mặc định
                    break;
            }
            return View("Index", model);
        }

        public ActionResult TimKiemXe(int ?noidi,int ?noiden,DateTime ?ngaydi)
        {
            if (noidi == null || noiden == null || ngaydi == null)
            {
                // Trả về một kết quả mặc định hoặc thông báo lỗi nếu có giá trị null
                ViewBag.ErrorMessage = "Vui lòng cung cấp đầy đủ thông tin.";
                ViewBag.DiaDiem = db.DiaDiems.ToList();
                return View("Index",new List<LichTrinh_display>());
            }

            // Nếu tất cả tham số đều có giá trị, thực hiện tìm kiếm
            List<LichTrinh_display> model = new List<LichTrinh_display>();
            List<LichTrinh> lst = db.LichTrinhs.Where(t => t.TuyenDuong.DiaDiem.ID_DIADIEM == noidi
                                                        && t.TuyenDuong.DiaDiem1.ID_DIADIEM == noiden
                                                        && t.KHOI_HANH.Date == ngaydi.Value.Date).ToList();

            foreach (var item in lst)
            {
                LichTrinh_display ltvm = new LichTrinh_display
                {
                    LichTrinh = item,
                    SoChoConLai = LaySoChoCon(item.MA_LICH_TRINH)
                };
                model.Add(ltvm);
            }

            ViewBag.DiaDiem = db.DiaDiems.ToList();
            return View("Index", model);
        }

        public ActionResult ThongBaoDaDat()
        {
            return View();
        }

        public ActionResult HuyVe(string id)
        {
            var ve = db.Ves.FirstOrDefault(t => t.ID_VE.Equals(id));
            ve.TRANG_THAI = "huy_ve";
            db.SubmitChanges();
            var ChiTietVe = db.ChiTietVes.Where(t => t.ID_VE.Equals(id)).ToList();
            foreach(var item in ChiTietVe)
            {
                Ghe g = db.Ghes.FirstOrDefault(t => t.ID_GHE == item.VI_TRI_NGOI);
                g.TINH_TRANG = false;
                db.SubmitChanges();
            }
            return Redirect("~/DangNhap/VeCuaToi");
        }
    }
}