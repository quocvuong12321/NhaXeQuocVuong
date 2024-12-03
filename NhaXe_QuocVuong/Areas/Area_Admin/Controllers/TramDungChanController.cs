using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhaXe_QuocVuong.Models;

namespace NhaXe_QuocVuong.Areas.Area_Admin.Controllers
{
    public class TramDungChanController : Controller
    {
        NhaXeDataContext db = new NhaXeDataContext();
        // GET: Area_Admin/TramDungChan
        public ActionResult Index(string search)
        {
            List<TramDungChan> tdc;
            if (!string.IsNullOrEmpty(search))
            {
                tdc = db.TramDungChans.Where(row => row.TEN_TRAMDUNGCHAN.Contains(search)).ToList();
            }
            else
            {
                tdc = db.TramDungChans.ToList();
            }
            ViewBag.Search = search;
            return View(tdc);
        }

        public ActionResult Create()
        {
            ViewBag.DiaDiemList = new SelectList(db.DiaDiems, "ID_DIADIEM", "TEN_TINH_THANH");
            return View();
        }
        [HttpPost]
        public ActionResult Create(TramDungChan tdc)
        {
            int count = db.TramDungChans.Count() + 1;
            tdc.ID_TRAMDUNGCHAN = $"TDC{count:D3}";
            var diemDau = db.DiaDiems.FirstOrDefault(d => d.ID_DIADIEM == tdc.ID_DIADIEM)?.TEN_TINH_THANH;
            if (ModelState.IsValid)
            {
                db.TramDungChans.InsertOnSubmit(tdc);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View(tdc);
        }
        public ActionResult Edit(string id)
        {
            TramDungChan tdc = db.TramDungChans.FirstOrDefault(row => row.ID_TRAMDUNGCHAN == id);

            if (tdc == null)
            {
                return HttpNotFound();
            }
            ViewBag.DiaDiemList = new SelectList(db.DiaDiems, "ID_DIADIEM", "TEN_TINH_THANH", tdc.ID_DIADIEM);
            return View(tdc);
        }
        [HttpPost]
        public ActionResult Edit(TramDungChan tdc)
        {
            TramDungChan TDC = db.TramDungChans.Where(row => row.ID_TRAMDUNGCHAN == tdc.ID_TRAMDUNGCHAN).FirstOrDefault();
            TDC.TEN_TRAMDUNGCHAN = tdc.TEN_TRAMDUNGCHAN;
            TDC.ID_DIADIEM = tdc.ID_DIADIEM;
            TDC.DIA_CHI = tdc.DIA_CHI;
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(string id)
        {
            TramDungChan tdc = db.TramDungChans.Where(row => row.ID_TRAMDUNGCHAN == id).FirstOrDefault();
            return View(tdc);
        }
        [HttpPost]
        public ActionResult Delete(string id, TramDungChan TDC)
        {
            TramDungChan tdc = db.TramDungChans.Where(row => row.ID_TRAMDUNGCHAN == id).FirstOrDefault();
            db.TramDungChans.DeleteOnSubmit(tdc);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}