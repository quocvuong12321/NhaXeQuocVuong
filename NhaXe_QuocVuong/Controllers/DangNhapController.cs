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









    }
}