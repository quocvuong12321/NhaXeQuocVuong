using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhaXe_QuocVuong.Models;

namespace NhaXe_QuocVuong.Areas.Area_Admin.Controllers
{
    public class XeController : Controller
    {
        NhaXeDataContext db = new NhaXeDataContext();
        // GET: Area_Admin/Xe
        public ActionResult Index(string search="")
        {
            List<Xe> XE = db.Xes.Where(row=>row.BIEN_SO_XE.Contains(search)).ToList();
            ViewBag.Search = search;
            return View(XE);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Xe x)
        {
            x.NGAY_THEM_XE = DateTime.Now;
            db.Xes.InsertOnSubmit(x);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            Xe xe = db.Xes.FirstOrDefault(x => x.ID_XE == id);
            if (xe == null)
            {
                return HttpNotFound();
            }
            return View(xe);
        }
        [HttpPost]
        public ActionResult Edit(Xe x)
        {
            Xe XE = db.Xes.Where(row => row.ID_XE == x.ID_XE).FirstOrDefault();
            XE.BIEN_SO_XE = x.BIEN_SO_XE;
            XE.LOAI_XE = x.LOAI_XE;
            XE.SO_GHE = x.SO_GHE;
            XE.NGAY_THEM_XE = x.NGAY_THEM_XE;
            db.SubmitChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            Xe xe = db.Xes.Where(row => row.ID_XE == id).FirstOrDefault();
            return View(xe);
        }
        [HttpPost]
        public ActionResult Delete(int id, Xe x)
        {
            Xe xe = db.Xes.Where(row => row.ID_XE == id).FirstOrDefault();
            db.Xes.DeleteOnSubmit(xe);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}