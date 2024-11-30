using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhaXe_QuocVuong.Models;

namespace NhaXe_QuocVuong.Areas.Area_Admin.Controllers
{
    public class TuyenDuongController : Controller
    {
        NhaXeDataContext db = new NhaXeDataContext("");
        // GET: Area_Admin/TuyenDuong
        public ActionResult Index(string search="")
        {
            List<TuyenDuong> td = db.TuyenDuongs.Where(row=>row.TEN_TUYEN.Contains(search)).ToList();
            ViewBag.Search = search;
            return View(td);
        }
        public ActionResult Create()
        {
            ViewBag.DiaDiemList = new SelectList(db.DiaDiems, "ID_DIADIEM", "TEN_TINH_THANH");
            return View();
        }
        [HttpPost]
        public ActionResult Create(TuyenDuong td)
        {
            var diemDau = db.DiaDiems.FirstOrDefault(d => d.ID_DIADIEM == td.DIEM_DAU)?.TEN_TINH_THANH;
            var diemCuoi = db.DiaDiems.FirstOrDefault(d => d.ID_DIADIEM == td.DIEM_CUOI)?.TEN_TINH_THANH;

            if (diemDau != null && diemCuoi != null)
            {
                td.TEN_TUYEN = diemDau + " - " + diemCuoi;
            }

            db.TuyenDuongs.InsertOnSubmit(td);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            TuyenDuong TD = db.TuyenDuongs.FirstOrDefault(td => td.ID_TUYEN == id);
            if (TD == null)
            {
                return HttpNotFound();
            }
            ViewBag.DiemDauList = new SelectList(db.DiaDiems, "ID_DIADIEM", "TEN_TINH_THANH", TD.DIEM_DAU);
            ViewBag.DiemCuoiList = new SelectList(db.DiaDiems, "ID_DIADIEM", "TEN_TINH_THANH", TD.DIEM_CUOI);
            return View(TD);
        }
        [HttpPost]
        public ActionResult Edit(TuyenDuong td)
        {
            TuyenDuong t = db.TuyenDuongs.Where(row => row.ID_TUYEN == td.ID_TUYEN).FirstOrDefault();
            t.TEN_TUYEN = td.TEN_TUYEN;
            t.DIEM_DAU = td.DIEM_DAU;
            t.DIEM_CUOI = td.DIEM_CUOI;
            t.KHOANG_CACH = td.KHOANG_CACH;
            t.THOI_GIAN_DUY_CHUYEN = td.THOI_GIAN_DUY_CHUYEN;
            t.TINH_TRANG = td.TINH_TRANG;
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            TuyenDuong td = db.TuyenDuongs.Where(row => row.ID_TUYEN == id).FirstOrDefault();
            return View(td);
        }
        [HttpPost]
        public ActionResult Delete(int id, TuyenDuong t)
        {
            TuyenDuong td = db.TuyenDuongs.Where(row => row.ID_TUYEN == id).FirstOrDefault();
            db.TuyenDuongs.DeleteOnSubmit(td);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}