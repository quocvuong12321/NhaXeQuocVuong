using System;
using System.Collections.Generic;
using System.Linq;
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
            return View(db.LichTrinhs.ToList());
        }
    }
}