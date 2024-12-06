using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhaXe_QuocVuong.Models;

namespace NhaXe_QuocVuong.Controllers
{
    public class DangNhapController : Controller
    {
        private NhaXeDataContext db = new NhaXeDataContext();
        // GET: DangNhap
        
        public ActionResult XacNhanRole()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session["khach"] = null;
            return RedirectToAction("Index", "Home");
        }


        public ActionResult Login_NguoiDung()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login_NguoiDung(string username, string password)
        {
            var user = db.userAccounts.FirstOrDefault(u => u.username == username && u.password == password);

            if (user != null)
            {
                if (user.role == "khach")
                {
                    Session["khach"] = user;
                    // Kiểm tra nếu có URL cần quay lại
                    if (Session["ReturnUrl"] != null)
                    {
                        string returnUrl = Session["ReturnUrl"].ToString();
                        Session["ReturnUrl"] = null; // Xóa dữ liệu sau khi sử dụng
                        return Redirect(returnUrl);
                    }

                    // Nếu không có URL lưu trước, chuyển hướng đến trang chính
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Bạn không được phép đăng nhập với vai trò này !";
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Username hoặc Password không đúng !";
                return View();
            }
        }


        public ActionResult DangKy_NguoiDung()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy_NguoiDung(string tenkhachhang, string sodienthoai, string register_email, string username, string password)
        {
            var existingUser = db.userAccounts.FirstOrDefault(u => u.username == username);
            if (existingUser != null)
            {
                ViewBag.RegisterErrorMessage = "Username đã tồn tại. Vui lòng chọn username khác.";
                return View("DangKy_NguoiDung");
            }

            userAccount newUser = new userAccount
            {
                username = username,
                password = password,
                role = "khach"
            };

            db.userAccounts.InsertOnSubmit(newUser);
            db.SubmitChanges();

            KhachHang newKhachHang = new KhachHang
            {
                USERNAME = username,
                TEN_KHACH_HANG = tenkhachhang,
                SO_DIEN_THOAI = sodienthoai,
                EMAIL = register_email
            };

            db.KhachHangs.InsertOnSubmit(newKhachHang);
            db.SubmitChanges();


            return RedirectToAction("Login_NguoiDung", "DangNhap");

        }


        public ActionResult ThongTinTaiKhoan()
        {
            if (Session["khach"] == null)
            {
                return RedirectToAction("Login_NguoiDung");
            }

            var user = Session["khach"] as userAccount;

            var khachHang = db.KhachHangs.FirstOrDefault(k => k.USERNAME == user.username);
            return View(khachHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(KhachHang updatedKhachHang)
        {
            if (Session["khach"] == null)
            {
                return RedirectToAction("Login_NguoiDung");
            }

            if (ModelState.IsValid)
            {
                var existingKhachHang = db.KhachHangs.SingleOrDefault(k => k.USERNAME == updatedKhachHang.USERNAME);
                if (existingKhachHang != null)
                {
                    // Cập nhật thông tin
                    existingKhachHang.TEN_KHACH_HANG = updatedKhachHang.TEN_KHACH_HANG;
                    existingKhachHang.SO_DIEN_THOAI = updatedKhachHang.SO_DIEN_THOAI;
                    existingKhachHang.EMAIL = updatedKhachHang.EMAIL;

                    db.SubmitChanges();

                    Session.Clear();
                    return RedirectToAction("Login_NguoiDung");
                }
                return HttpNotFound();
            }
            return View("ThongTinTaiKhoan", updatedKhachHang);
        }

        //lịch sử mua vé
        public ActionResult LichSuMuaVe()
        {
            if (Session["khach"] == null)
            {
                return RedirectToAction("Login_NguoiDung");
            }

            var user = Session["khach"] as userAccount;
            var lichSuMuaVe = (from ve in db.Ves
                               
                               where ve.ID_KHACH_HANG == user.username
                               select new LichSuMuaVe
                               {
                                   Id_Ve = ve.ID_VE,
                                   NgayDat = ve.NGAY_DAT_VE,
                                   TongTien = (float)ve.TONG_TIEN,
                                   TrangThai = ve.TRANG_THAI
                               }).ToList();

            return View(lichSuMuaVe);
        }
        //chi tiết vé
        public ActionResult Details(string idVe)
        {
            if (Session["khach"] == null)
            {
                return RedirectToAction("Login_NguoiDung");
            }
            if (string.IsNullOrEmpty(idVe))
            {
                return HttpNotFound();
            }

            var ticketDetailBase = (from ve in db.Ves
                                    join kh in db.KhachHangs on ve.ID_KHACH_HANG equals kh.USERNAME
                                    join lt in db.LichTrinhs on ve.ID_LICH_TRINH equals lt.MA_LICH_TRINH
                                    join td in db.TuyenDuongs on lt.ID_TUYEN_DUONG equals td.ID_TUYEN
                                    where ve.ID_VE == idVe
                                    select new
                                    {
                                        TenKH = kh.TEN_KHACH_HANG,
                                        NgayDat = ve.NGAY_DAT_VE,
                                        TuyenDuong = td.TEN_TUYEN,
                                        Qrcode = ve.QR_CODE,
                                        TongTien = ve.TONG_TIEN,
                                        TgianKhoiHanh = lt.KHOI_HANH,
                                        VeId = ve.ID_VE,
                                        LichTrinhId = ve.ID_LICH_TRINH
                                    }).FirstOrDefault();


            if (ticketDetailBase != null)
            {
                var danhSachGheEntities = (from ctv in db.ChiTietVes
                                           join g in db.Ghes on ctv.VI_TRI_NGOI equals g.ID_GHE
                                              //join g in db.Ghes                  
                                           where ctv.ID_VE == ticketDetailBase.VeId
                                           select g).ToList();

                List<GheDTO> newGhe = danhSachGheEntities.Select(gl => new GheDTO
                {
                    IdGhe = gl.ID_GHE,
                    ViTriNgoi = gl.VI_TRI_NGOI

                }).ToList();


                var ticketDetail = new CTVe
                {
                    TenKH = ticketDetailBase.TenKH,
                    NgayDat = ticketDetailBase.NgayDat,
                    TuyenDuong = ticketDetailBase.TuyenDuong,
                    Qrcode = ticketDetailBase.Qrcode,
                    TongTien = (float)ticketDetailBase.TongTien,
                    TgianKhoiHanh = ticketDetailBase.TgianKhoiHanh,
                   
                    DanhSachGhe = newGhe
                };

                return View(ticketDetail);
            }

            return View(new CTVe()); // Trả về một đối tượng trống nếu không tìm thấy vé.

        }

        public ActionResult VeCuaToi()
        {
            if (Session["khach"] == null)
            {
                return RedirectToAction("Login_NguoiDung");
            }

            var user = Session["khach"] as userAccount;
            var veCuaToi = (from ve in db.Ves
                               join lt in db.LichTrinhs on ve.ID_LICH_TRINH equals lt.MA_LICH_TRINH
                               join kh in db.KhachHangs on ve.ID_KHACH_HANG equals kh.USERNAME
                               join td in db.TuyenDuongs on lt.ID_TUYEN_DUONG equals td.ID_TUYEN
                               join xe in db.Xes on lt.ID_XE equals xe.ID_XE
                               where ve.ID_KHACH_HANG == user.username && lt.TRANG_THAI == "MO_BAN"
                               
                               select new VeCuaToi
                               {
                                   IdVe = ve.ID_VE,
                                   NgayKhoiHanh = lt.KHOI_HANH,
                                   TenTuyen = td.TEN_TUYEN,
                                   BienSoXe = xe.BIEN_SO_XE,
                                   TrangThai = ve.TRANG_THAI
                               }).ToList();

            return View(veCuaToi);
        }
        
        public ActionResult TicketDetail(string idVe)
        {
            var ve = db.Ves.FirstOrDefault(x => x.ID_VE == idVe);
            if (ve == null)
            {
                TempData["ErrorMessage"] = "Vé không tồn tại.";
                return RedirectToAction("VeCuaToi", "DangNhap");
            }

            var tddfdf = db.TuyenDuongs.FirstOrDefault(t => t.ID_TUYEN == ve.LichTrinh.ID_TUYEN_DUONG);
            if (tddfdf == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tuyến đường.";
                return RedirectToAction("VeCuaToi", "DangNhap");
            }

            var startDiem = db.TramDungChans
                .Where(tdc => tdc.ID_DIADIEM == tddfdf.DIEM_DAU)
                .Select(tdc => new { tdc.ID_TRAMDUNGCHAN, tdc.TEN_TRAMDUNGCHAN })
                .ToList();

            var endDiem = db.TramDungChans
                .Where(tdc => tdc.ID_DIADIEM == tddfdf.DIEM_CUOI)
                .Select(tdc => new { tdc.ID_TRAMDUNGCHAN, tdc.TEN_TRAMDUNGCHAN })
                .ToList();

            // Truyền các giá trị này vào ViewBag hoặc ViewModel
            ViewBag.StartDiem = new SelectList(startDiem, "ID_TRAMDUNGCHAN", "TEN_TRAMDUNGCHAN");
            ViewBag.EndDiem = new SelectList(endDiem, "ID_TRAMDUNGCHAN", "TEN_TRAMDUNGCHAN");
            var veCuaToi = new VeCuaToi
            {
                IdVe = ve.ID_VE,
                NgayKhoiHanh = ve.LichTrinh.KHOI_HANH,
                TenTuyen = ve.LichTrinh.TuyenDuong.TEN_TUYEN,
                BienSoXe = ve.LichTrinh.Xe.BIEN_SO_XE,
                TrangThai = ve.TRANG_THAI,
                DiemDon = ve.DIEM_DOAN,
                DiemTra = ve.DIEM_TRA,
                NgayKetThuc = ve.LichTrinh.KET_THUC
            };

            return View(veCuaToi);
        }

        [HttpPost]
        public JsonResult UpdateTicket(string idVe, string diemDon, string diemTra)
        {
            System.Diagnostics.Debug.WriteLine("idVe: " + idVe);
            var ve = db.Ves.FirstOrDefault(x => x.ID_VE == idVe);
            if (ve == null)
            {
                return Json(new { success = false, message = "Vé không tồn tại." });
            }

            ve.DIEM_DOAN = diemDon;
            ve.DIEM_TRA = diemTra;
            try
            {
                // Lưu thay đổi vào cơ sở dữ liệu
                db.SubmitChanges();
                return Json(new { success = true, message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }
    }
}