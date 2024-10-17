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
                Session["Username"] = user.username;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Username hoặc Password không đúng.";
                return View();
            }
        }


       


    }
}