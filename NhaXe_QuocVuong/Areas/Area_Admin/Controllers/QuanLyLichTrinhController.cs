using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Windows.Documents;
using NhaXe_QuocVuong.Models;

namespace NhaXe_QuocVuong.Areas.Area_Admin.Controllers
{
    [AuthorizeSession]
    public class QuanLyLichTrinhController : Controller
    {
        // GET: Area_Admin/QuanLyLịchTrinh
        NhaXeDataContext db = new NhaXeDataContext("");


        public ActionResult Index(string search="")
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
                            SoGheTrong = db.Ghes.Count(i => i.TINH_TRANG == false && i.MA_LICH_TRINH == LichTrinh.MA_LICH_TRINH),
                            TrangThai= LichTrinh.TRANG_THAI,
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
                TrangThai = item.TrangThai,
                IDTaiXe = item.TaiXe.USERNAME
            }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                DateTime sea = DateTime.Parse(search);
                result = result.Where(e => DateTime.TryParse(e.TG_KhoiHanh, out DateTime parsedDate) && parsedDate.Date == sea.Date).ToList();

            }

            if(user.NHANVIEN.LOAI_NV == "TAI_XE")
            {
                result = result.Where(e => e.IDTaiXe == user.username && e.TrangThai== "MO_BAN").ToList();
            }

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
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetTaiXe(int idTuyenDuong,string KHOI_HANH)
        {
            try
            {
                TuyenDuong td = db.TuyenDuongs.FirstOrDefault(tdd => tdd.ID_TUYEN == idTuyenDuong);
                DateTime KetThuc = DateTime.Parse(KHOI_HANH).AddHours(td.THOI_GIAN_DUY_CHUYEN);
                DateTime KhoiHanh = DateTime.Parse(KHOI_HANH);


                var nhanVienKhongCoLichTrinh = db.NHANVIENs
                .Where(nv => nv.LOAI_NV == "TAI_XE" && !db.LichTrinhs 
                    .Where(lt => lt.KHOI_HANH < KetThuc && lt.KET_THUC > KhoiHanh ) // Lọc lịch trình nằm trong khoảng thời gian
                    .Select(lt => lt.TAIXE)
                    .Contains(nv.USERNAME));

                var xeKhongCoLichTrinh = db.Xes
                    .Where(xe => !db.LichTrinhs
                        .Where(lt => lt.KHOI_HANH < KetThuc && lt.KET_THUC > KhoiHanh && lt.ID_XE == xe.ID_XE) // Lọc lịch trình nằm trong khoảng thời gian
                        .Select(lt => lt.ID_XE)
                        .Contains(xe.ID_XE));
                var query = new
                {
                    taixe = nhanVienKhongCoLichTrinh.Select(nv => new { USERNAME = nv.USERNAME, NAME = nv.TEN_NHANVIEN }),
                    xe = xeKhongCoLichTrinh.Select(nv => new { ID_XE = nv.ID_XE, NAME = $"{nv.BIEN_SO_XE} - {nv.LOAI_XE}" }),

                };
                Session["tai_xe"] = query;
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            catch
            {

                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [CheckSessionRole("")]
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
                LichTrinh newLichTrinh = new LichTrinh
                {
                    MA_LICH_TRINH = MaLichTrinh,
                    NGUOI_TAO_LICH_TRINH = user.username,
                    ID_TUYEN_DUONG = int.Parse(request.formData.ID_TUYEN_DUONG),
                    KHOI_HANH = DateTime.Parse(request.formData.KHOI_HANH),
                    KET_THUC = DateTime.Parse(request.formData.KHOI_HANH).AddHours(db.TuyenDuongs.FirstOrDefault(td => td.ID_TUYEN == int.Parse(request.formData.ID_TUYEN_DUONG)).THOI_GIAN_DUY_CHUYEN),
                    GIA_VE = double.Parse(request.formData.GIA_VE),
                    ID_XE = int.Parse(request.formData.ID_XE),
                    TAIXE = request.formData.TAIXE,
                    CHI_PHI_PHAT_SINH = 0,
                    NGAY_TAO_LICH_TRINH = DateTime.Now,
                    TRANG_THAI = "MO_BAN"
                };

                var stops = request.StopsData;
                string check="";
                stops.ForEach(sp =>
                {
                    DateTime timeStop = DateTime.Parse(sp.thoigianden);
                    if (timeStop < newLichTrinh.KHOI_HANH|| timeStop > newLichTrinh.KET_THUC)
                    {
                            check = "Thời gian Khởi hành và kết thúc không hợp lệ ";
                        return;
                    }

                });
                var duplicates = stops.GroupBy(x => new { x.Diachi })
                 .Where(g => g.Count() > 1)
                 .Select(g => g.Key);
                if (duplicates.Any())
                {
                    check = "Trạm dừng chân trùng ";
                }
                if (check.Length>0)
                {
                    return Json(new { success = false, message = check });
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
          
            /// vao trang chi tiet
            // Lấy đối tượng LichTrinh cần chỉnh sửa
            LichTrinh lt = db.LichTrinhs.FirstOrDefault(ltt => ltt.MA_LICH_TRINH == id);

            if (lt == null)
            {
                return HttpNotFound();  // Trường hợp không tìm thấy LichTrinh
            }
            //cap nhat trang thai ghe tai day
            if (user.NHANVIEN.LOAI_NV == "TAI_XE")
            {
                // kiem tra thoi gian
                if (lt.KET_THUC >= DateTime.Now)
                {
                     TempData["ThongBao"] = "Thời gian không hợp lệ!";
                    return RedirectToAction("Index"); // hoặc action khác
                }

                if (lt.TAIXE!= user.username)
                {
                    TempData["ThongBao"] = "Bạn không thể cập nhật chuyến đi này.";
                    return RedirectToAction("Index"); // hoặc action khác
                }
                // cap nhat trang thai 
                lt.TRANG_THAI = "KET_THUC";
                //db.LichTrinhs.InsertOnSubmit(lt);
                //db.SubmitChanges();
                //lt.CHI_PHI_PHAT_SINH = 1000;
                // tong ket doanh thu
                int tongve = lt.Ves.Where(ve => ve.TRANG_THAI == "da_thanh_toan").Count();
                DoanhThu dt = new DoanhThu
                {
                    MA_LICH_TRINH = id,
                    ID_DOANHTHU = "DT_" + id,
                    SO_VE_DA_DAT = tongve,
                    TONGTIEN = lt.GIA_VE * tongve
                };
                //db.DoanhThus.InsertOnSubmit(dt);
                // cap nhat ve thanh da su dung

                var vesToUpdate = db.Ves.Where(v => v.TRANG_THAI == "da_thanh_toan").ToList();

                // Duyệt qua các bản ghi và cập nhật trạng thái
                foreach (var item in vesToUpdate)
                {
                    item.TRANG_THAI = "da_su_dung";  // Cập nhật trạng thái
                }
                //db.Ves.InsertAllOnSubmit(vesToUpdate);
                db.SubmitChanges();
                
                return RedirectToAction("Index"); // hoặc action khác

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
        [CheckSessionRole("")]
        public JsonResult DeleteLichTrinh(string id)
        {
            try
            {
                var existingLichTrinh = db.LichTrinhs.FirstOrDefault(l => l.MA_LICH_TRINH == id);
                if (existingLichTrinh == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy lịch trình cần xóa." });
                }

                existingLichTrinh.TRANG_THAI = "DA_DONG";
                db.SubmitChanges();

                return Json(new { success = true, message = "Xóa lịch trình thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }


        [HttpPost]
        [CheckSessionRole("")]
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
                       

                        string check = "";
                        stops.ForEach(sp =>
                        {
                            DateTime timeStop = DateTime.Parse(sp.thoigianden);
                            if (!(timeStop >= existingLichTrinh.KHOI_HANH && timeStop <= existingLichTrinh.KET_THUC))
                            {
                                check = "Thời gian Khởi hành và kết thúc không hợp lệ ";
                                return;
                            }

                        });
                        var duplicates = stops.GroupBy(x => new { x.Diachi })
                         .Where(g => g.Count() > 1)
                         .Select(g => g.Key);
                        if (duplicates.Any())
                        {
                            check = "Trạm dừng chân trùng ";
                        }
                        if (check.Length > 0)
                        {
                            return Json(new { success = false, message = check });
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

        [HttpGet]
        public ActionResult DetailLichTrinh(string id)
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




            int diemdau = db.TuyenDuongs.First(td => td.ID_TUYEN == lt.ID_TUYEN_DUONG).DIEM_DAU;
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


            ViewBag.Ghe = from ghe in db.Ghes
                          join ctv in db.ChiTietVes on ghe.ID_GHE equals ctv.VI_TRI_NGOI into gheCtv
                          from ctvItem in gheCtv.DefaultIfEmpty()
                          join ve in db.Ves on ctvItem.ID_VE equals ve.ID_VE into ctvVe
                          from veItem in ctvVe.DefaultIfEmpty()
                          join kh in db.KhachHangs on veItem.ID_KHACH_HANG equals kh.USERNAME into veKh
                          from khItem in veKh.DefaultIfEmpty()
                          where ghe.MA_LICH_TRINH == id
                        select new AdminChiTietVe
                        {
                            ID_GHE= ghe.ID_GHE,
                            VI_TRI_NGOI = ghe.VI_TRI_NGOI,
                            TINH_TRANG= ghe.TINH_TRANG ?"Đã đặt":"Trống",
                            TEN_KHACH_HANG = khItem.TEN_KHACH_HANG,
                            SO_DIEN_THOAI = khItem.SO_DIEN_THOAI,
                            EMAIL = khItem.EMAIL,
                            DIEM_DOAN =db.TramDungChans.Where(ttdc=>ttdc.ID_TRAMDUNGCHAN == veItem.DIEM_DOAN).FirstOrDefault().DIA_CHI,
                           DIEM_TRA = db.TramDungChans.Where(ttdc => ttdc.ID_TRAMDUNGCHAN == veItem.DIEM_TRA).FirstOrDefault().DIA_CHI,
                        };
            return View(lt);

        }

    }
}