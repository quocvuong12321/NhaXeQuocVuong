using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhaXe_QuocVuong.Models;

namespace NhaXe_QuocVuong.Controllers
{
    public class DangKyController : Controller
    {
        private NhaXeDataContext db = new NhaXeDataContext();
        // GET: Register

        public ActionResult DangKy_NguoiDung()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy_NguoiDung(string tenkhachhang, string sodienthoai,string register_email, string username, string password)
        {
            // Kiểm tra xem username đã tồn tại chưa
            var existingUser = db.userAccounts.FirstOrDefault(u => u.username == username);
            if (existingUser != null)
            {
                ViewBag.RegisterErrorMessage = "Username đã tồn tại. Vui lòng chọn username khác.";
                return View("DangKy_NguoiDung");
            }

            // Tạo đối tượng userAccount mới
            userAccount newUser = new userAccount
            {
                username = username,
                password = password,
                role = "khach" // Mặc định vai trò là khách hàng
            };

            db.userAccounts.InsertOnSubmit(newUser);
            db.SubmitChanges();

            // Thêm thông tin vào bảng KhachHang
            KhachHang newKhachHang = new KhachHang
            {
                USERNAME = username,
                TEN_KHACH_HANG = tenkhachhang,
                SO_DIEN_THOAI = sodienthoai,
                EMAIL = register_email
            };

            db.KhachHangs.InsertOnSubmit(newKhachHang);
            db.SubmitChanges();

            //Chuyển hướng đến trang đăng nhập sau khi đăng ký thành công
            return RedirectToAction("Login_NguoiDung", "DangNhap");

        }



    }
}