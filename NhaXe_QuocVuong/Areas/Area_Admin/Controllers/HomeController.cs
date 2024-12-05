using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NhaXe_QuocVuong.Areas.Area_Admin.Controllers
{
    [AuthorizeSession]
    public class HomeController : Controller
    {
        // GET: Area_Admin/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}