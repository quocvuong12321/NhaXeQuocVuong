using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using NhaXe_QuocVuong.Models;

namespace NhaXe_QuocVuong.Areas.Area_Admin.Controllers
{
    public class QuanLyLichTrinhController : Controller
    {
        // GET: Area_Admin/QuanLyLịchTrinh
        NhaXeDataContext db = new NhaXeDataContext("");


        public ActionResult Index()
        {
            var user = Session["quanly"] as userAccount;
            if (user == null)
            {
                return RedirectToAction("Login_Admin", "DangNhapAdmin", new { area = "" });
            }
            var query = from LichTrinh in db.LichTrinhs
                        join tuyenDuong in db.TuyenDuongs on LichTrinh.ID_TUYEN_DUONG equals tuyenDuong.ID_TUYEN
                        join Xe in db.Xes on LichTrinh.ID_XE equals Xe.ID_XE
                        join DiemDau in db.DiaDiems on tuyenDuong.DIEM_DAU equals DiemDau.ID_DIADIEM
                        join DiemCuoi in db.DiaDiems on tuyenDuong.DIEM_CUOI equals DiemCuoi.ID_DIADIEM
                        join NHANVIEN in db.NHANVIENs on LichTrinh.TAIXE equals NHANVIEN.USERNAME
                        select new
                        {
                            MaLichTrinh= LichTrinh.MA_LICH_TRINH,
                            BienSo= Xe.BIEN_SO_XE,
                            TaiXe= NHANVIEN,
                            TG_KhoiHanh = LichTrinh.KHOI_HANH,
                            TG_KetThuc = LichTrinh.KET_THUC,
                            Gia = LichTrinh.GIA_VE,
                            ThoiGianDuyChuyen = tuyenDuong.THOI_GIAN_DUY_CHUYEN,
                            DiemKhoiHanh = DiemDau.TEN_TINH_THANH,
                            DiemKetThuc = DiemCuoi.TEN_TINH_THANH,
                            LoaiXe = Xe.LOAI_XE,
                            SoGhe = Xe.SO_GHE,
                            SoGheTrong = db.Ghes.Count(i => i.TINH_TRANG == false && i.MA_LICH_TRINH == LichTrinh.MA_LICH_TRINH)
                        };

            var result = query.ToList().Select(item => new LichTrinhViewModel
            {
                MaLichTrinh=item.MaLichTrinh,
                TG_KhoiHanh = item.TG_KhoiHanh.ToString("dd/MM/yyyy HH:mm:ss"),
                TG_KetThuc = item.TG_KetThuc.ToString("dd/MM/yyyy HH:mm:ss"),
                Gia = item.Gia.ToString("N0", CultureInfo.InvariantCulture).Replace(",", "."),
                ThoiGianDuyChuyen = item.ThoiGianDuyChuyen.ToString(),
                DiemKhoiHanh = item.DiemKhoiHanh,
                DiemKetThuc = item.DiemKetThuc,
                LoaiXe = item.LoaiXe,
                BienSoXe= item.BienSo,
                SoGhe = item.SoGhe.ToString(),
                SoGheTrong = item.SoGheTrong,
                TenNhanVien = item.TaiXe.TEN_NHANVIEN,
            }).ToList();



            return View(result);
           
        }

        public ActionResult CreateLichTrirnh()
        {
            var user = Session["quanly"] as userAccount;
            if (user == null)
            {
                return RedirectToAction("Login_Admin", "DangNhapAdmin", new { area = "" });
            }

            ViewBag.ID_TUYEN_DUONG = new SelectList(db.TuyenDuongs.ToList(), "ID_TUYEN", "TEN_TUYEN"); 
            ViewBag.ID_XE = new SelectList(db.Xes.ToList().Select(x => new
            {
                ID_XE = x.ID_XE,
                DisplayText = $"{x.BIEN_SO_XE} - {x.LOAI_XE}"
            }), "ID_XE", "DisplayText");
            ViewBag.TAIXE = new SelectList(db.NHANVIENs.Where(nv => nv.LOAI_NV == "TAI_XE")
                .ToList(), "USERNAME", "TEN_NHANVIEN");
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetDiemDungChan(int idTuyenDuong)
        {
            try {
                TuyenDuong td = db.TuyenDuongs.FirstOrDefault(tdd => tdd.ID_TUYEN == idTuyenDuong);
                var query = new
                {
                    khoihanh = db.TramDungChans.Where(tdc => tdc.ID_DIADIEM == td.DIEM_DAU).Select(tdc => new
                    {
                        ID_TRAMDUNGCHAN= tdc.ID_TRAMDUNGCHAN,
                        DD_TRAMDUNG= tdc.DIA_CHI,
                        TEN_TRAM=tdc.TEN_TRAMDUNGCHAN
                    }).ToList(),
                    ketthuc = db.TramDungChans.Where(tdc => tdc.ID_DIADIEM == td.DIEM_CUOI).Select(tdc => new
                    {
                        ID_TRAMDUNGCHAN = tdc.ID_TRAMDUNGCHAN,
                        DD_TRAMDUNG = tdc.DIA_CHI,
                        TEN_TRAM = tdc.TEN_TRAMDUNGCHAN
                    }).ToList(),
                };
                Session["tramduchan"] = query;
                return Json(query, JsonRequestBehavior.AllowGet);
            } catch { 
           
                return Json("",JsonRequestBehavior.AllowGet);
             }
        }

        [HttpPost]
        public ActionResult CreateLichTrinh( LichTrinhRequest request)
        {
            if (ModelState.IsValid)
            {
                var user = Session["quanly"] as userAccount;
                if (user == null) {
                    return RedirectToAction("Login_Admin", "DangNhapAdmin", new { area = "" });
                }
                string MaLichTrinh = DateTime.Now.ToString("ddMMyyyyHH") + GenerateRandomString(4);
                
                if (db.LichTrinhs.FirstOrDefault(lt => lt.MA_LICH_TRINH == MaLichTrinh) != null) {
                    MaLichTrinh += "(1)";
                };
                LichTrinh newLichTrinh = new LichTrinh { 
                MA_LICH_TRINH = MaLichTrinh,
                NGUOI_TAO_LICH_TRINH = user.username,
                ID_TUYEN_DUONG =int.Parse(request.formData.ID_TUYEN_DUONG),
                KHOI_HANH = DateTime.Parse(request.formData.KHOI_HANH),
                KET_THUC = DateTime.Parse(request.formData.KHOI_HANH).AddHours(db.TuyenDuongs.FirstOrDefault(td=>td.ID_TUYEN == int.Parse(request.formData.ID_TUYEN_DUONG)).THOI_GIAN_DUY_CHUYEN ),
                GIA_VE = double.Parse(request.formData.GIA_VE),
                ID_XE= int.Parse(request.formData.ID_XE),
                TAIXE=request.formData.TAIXE,
                CHI_PHI_PHAT_SINH=0,
                NGAY_TAO_LICH_TRINH= DateTime.Now,
                };

                var stops = request.StopsData;
                bool check=false;
                stops.ForEach(sp =>
                {
                    DateTime timeStop = DateTime.Parse(sp.thoigianden);
                    if (timeStop < newLichTrinh.KHOI_HANH|| timeStop > newLichTrinh.KET_THUC)
                    {
                        check = true;
                        return;
                    }
                });
                if (check)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ điêm dừng chân không hợp lệ!" });
                }
                db.LichTrinhs.InsertOnSubmit(newLichTrinh);
                db.SubmitChanges();

                var TramDungChan = stops.Select(stop => new ThemTramDungChan { 
                    ID_TRAMDUNGCHAN=stop.Diachi,
                    MA_LICH_TRINH= MaLichTrinh,
                    THOIGIANDEN = DateTime.Parse(stop.thoigianden),
                }).ToList();
                db.ThemTramDungChans.InsertAllOnSubmit(TramDungChan);
                db.SubmitChanges();
                return Json(new { success = true, message = "Lịch trình đã được tạo thành công!" });
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }


        static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder result = new StringBuilder(length);
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }
        //public ActionResult UpdateLichTrinh13132123(string id) {
        //    return View();
        //}
        public ActionResult UpdateLichTrinh(string id)
        {
            var user = Session["quanly"] as userAccount;
            if (user == null)
            {
                return RedirectToAction("Login_Admin", "DangNhapAdmin", new { area = "" });
            }

            // Lấy đối tượng LichTrinh cần chỉnh sửa
            LichTrinh lt = db.LichTrinhs.FirstOrDefault(ltt => ltt.MA_LICH_TRINH == id);

            if (lt == null)
            {
                return HttpNotFound();  // Trường hợp không tìm thấy LichTrinh
            }

            // Gán dữ liệu vào ViewBag cho dropdown
            // tạo selectList danh sách tuyến đường
            ViewBag.ID_TUYEN_DUONG = new SelectList(db.TuyenDuongs.ToList(), "ID_TUYEN", "TEN_TUYEN", lt.ID_TUYEN_DUONG); // Gán giá trị đã chọn
             // tạo selectList danh sách xe
            ViewBag.ID_XE = new SelectList(db.Xes.ToList().Select(x => new
            {
                ID_XE = x.ID_XE,
                DisplayText = $"{x.BIEN_SO_XE} - {x.LOAI_XE}"
            }), "ID_XE", "DisplayText", lt.ID_XE); // Gán giá trị đã chọn
            // tài xế
            ViewBag.TAIXE = new SelectList(db.NHANVIENs.Where(nv => nv.LOAI_NV == "TAI_XE").ToList(), "USERNAME", "TEN_NHANVIEN", lt.TAIXE); // Gán giá trị đã chọn
            // lấy ra điểm đầu và điểm cuối của lịch trình này.
           
            
          
            
            int diemdau = db.TuyenDuongs.First(td=>td.ID_TUYEN==lt.ID_TUYEN_DUONG).DIEM_DAU;
            int diemcuoi = db.TuyenDuongs.First(td => td.ID_TUYEN == lt.ID_TUYEN_DUONG).DIEM_CUOI;
            // từ điểm bắt đầu, lấy danh sách thêm trạm dừng chân của tuyến đường
            ViewBag.TramDiemDau = from ThemTramDungChan in db.ThemTramDungChans
                                  join TramDungChan in db.TramDungChans on ThemTramDungChan.ID_TRAMDUNGCHAN equals TramDungChan.ID_TRAMDUNGCHAN
                                  where TramDungChan.ID_DIADIEM == diemdau && ThemTramDungChan.MA_LICH_TRINH == lt.MA_LICH_TRINH
                                  select ThemTramDungChan;
            // từ điểm kết thúc lấy danh sách thêm trạm dừng chân của tuyến đường
            ViewBag.TramDiemCuoi = from ThemTramDungChan in db.ThemTramDungChans
                                  join TramDungChan in db.TramDungChans on ThemTramDungChan.ID_TRAMDUNGCHAN equals TramDungChan.ID_TRAMDUNGCHAN
                                  where TramDungChan.ID_DIADIEM == diemcuoi && ThemTramDungChan.MA_LICH_TRINH == lt.MA_LICH_TRINH
                                   select ThemTramDungChan;

            // lấy thông tin tuyết đường
            TuyenDuong tddfdf = db.TuyenDuongs.FirstOrDefault(tdd => tdd.ID_TUYEN == lt.ID_TUYEN_DUONG);

            var query = new
            {
                // lấy danh sách các trạm dừng chân có thể chọn trong tuyến đường này (khởi hành)
                khoihanh = db.TramDungChans.Where(tdc => tdc.ID_DIADIEM == tddfdf.DIEM_DAU).Select(tdc => new
                {
                    ID_TRAMDUNGCHAN = tdc.ID_TRAMDUNGCHAN,
                    DD_TRAMDUNG = tdc.DIA_CHI,
                    TEN_TRAM = tdc.TEN_TRAMDUNGCHAN
                }).ToList(),
                // lấy danh sách các trạm dừng chân có thể chọn trong tuyến đường này (kết thúc)
                ketthuc = db.TramDungChans.Where(tdc => tdc.ID_DIADIEM == tddfdf.DIEM_CUOI).Select(tdc => new
                {
                    ID_TRAMDUNGCHAN = tdc.ID_TRAMDUNGCHAN,
                    DD_TRAMDUNG = tdc.DIA_CHI,
                    TEN_TRAM = tdc.TEN_TRAMDUNGCHAN
                }).ToList(),
            };
            
            ViewBag.StartDiem = new SelectList(query.khoihanh, "ID_TRAMDUNGCHAN", "TEN_TRAM"); ;
            ViewBag.EndDiem = new SelectList(query.ketthuc, "ID_TRAMDUNGCHAN", "TEN_TRAM"); ;

            return View(lt);
        }

        [HttpPost]
        public ActionResult UpdateLichTrinh(LichTrinhRequest model)
        {

 
            //return Json(new { success = false, message = "Dữ liệu không hợp lệ điêm dừng chân không hợp lệ!" }); ;
            var user = Session["quanly"] as userAccount;
            if (user == null)
            {
                return RedirectToAction("Login_Admin", "DangNhapAdmin",new { area = "" });
            }

            // Kiểm tra tính hợp lệ của dữ liệu
            if (ModelState.IsValid)
            {
                // Tìm kiếm LichTrinh theo MA_LICH_TRINH
                LichTrinh existingLichTrinh = db.LichTrinhs.FirstOrDefault(l => l.MA_LICH_TRINH == model.formData.MA_LICH_TRINH);
                TuyenDuong td = db.TuyenDuongs.FirstOrDefault(tdd=>tdd.ID_TUYEN == existingLichTrinh.ID_TUYEN_DUONG);
                if (td == null) {
                    ModelState.AddModelError("", "Không tìm thấy lịch trình cần cập nhật.");
                }
                if (existingLichTrinh != null)
                {
                    //existingLichTrinh.ID_TUYEN_DUONG = model.ID_TUYEN_DUONG;
              
                    existingLichTrinh.KHOI_HANH = DateTime.Parse(model.formData.KHOI_HANH);
                    existingLichTrinh.KET_THUC = DateTime.Parse(model.formData.KHOI_HANH).AddHours(td.THOI_GIAN_DUY_CHUYEN);
                    existingLichTrinh.GIA_VE = double.Parse( model.formData.GIA_VE);
                    if (int.Parse(model.formData.ID_XE) != 0)
                    {
                    existingLichTrinh.ID_XE = int.Parse(model.formData.ID_XE);
                    }
                    if (model.formData.TAIXE != null)
                    {
                        existingLichTrinh.TAIXE = model.formData.TAIXE;
                    }
                    

                    //db.ThemTramDungChans.DeleteAllOnSubmit();
                    var itemsToDelete = db.ThemTramDungChans.Where(item => item.MA_LICH_TRINH==existingLichTrinh.MA_LICH_TRINH);
                     db.ThemTramDungChans.DeleteAllOnSubmit(itemsToDelete);
                    // // Xóa các mục được tìm thấy
                    //foreach ( var item in itemsToDelete)
                    // {
                    //     db
                    // }
                    var stops = model.StopsData;
                    if (stops != null)
                    {
                        bool check = false;
                        stops.ForEach(sp =>
                        {
                            if (sp.thoigianden == null)
                            {
                                check = true;
                                return;
                            }
                            DateTime timeStop = DateTime.Parse(sp.thoigianden);

                            if (!(timeStop >= existingLichTrinh.KHOI_HANH && timeStop <= existingLichTrinh.KET_THUC))
                            {
                                check = true;
                                return;
                            }
                        });
                        if (check)
                        {
                            return Json(new { success = false, message = "Dữ liệu không hợp lệ điểm dừng chân không hợp lệ!" });
                        }
                        //db.SubmitChanges();
                        var TramDungChan = stops.Select(stop => new ThemTramDungChan
                        {
                            ID_TRAMDUNGCHAN = stop.Diachi,
                            MA_LICH_TRINH = existingLichTrinh.MA_LICH_TRINH,
                            THOIGIANDEN = DateTime.Parse(stop.thoigianden),
                        }).ToList();
                        db.ThemTramDungChans.InsertAllOnSubmit(TramDungChan);
                        //db.SubmitChanges();

                    }
                    db.SubmitChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy lịch trình cần cập nhật.");
                }
            }
            return View(model);
        }

    }
}