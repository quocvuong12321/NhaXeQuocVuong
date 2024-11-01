using NhaXe_QuocVuong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NhaXe_QuocVuong.Controllers
{
    public class DangNhapAdminController : Controller
    {
        // GET: DangNhapAdmin
        private NhaXeDataContext db = new NhaXeDataContext();

        public ActionResult Logout()
        {
            Session["quanly"] = null;

            return RedirectToAction("Index","Home");
        }


        public ActionResult Login_Admin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login_Admin(string username, string password)
        {
            var user = db.userAccounts.FirstOrDefault(u => u.username == username && u.password == password);

            if (user != null)
            {
                if (user.role == "nha_xe")
                {
                    Session["quanly"] = user;
                    return Redirect("~/Area_Admin/Home/Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Tài khoản không có quyền truy cập với vai trò này!";
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Username hoặc Password không đúng.";
                return View();
            }
        }

        public ActionResult ThongTinTaiKhoan()
        {
            if (Session["quanly"] == null)
            {
                return RedirectToAction("Login_Admin");
            }

            var user = Session["quanly"] as userAccount;

            var quanly = db.NHANVIENs.FirstOrDefault(k => k.USERNAME == user.username);
            return View(quanly);
        }



    }
}