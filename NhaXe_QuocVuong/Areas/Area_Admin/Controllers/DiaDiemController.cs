using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhaXe_QuocVuong.Models;
namespace NhaXe_QuocVuong.Areas.Area_Admin.Controllers
{
    [AuthorizeSession]
    public class DiaDiemController : Controller
    {
        NhaXeDataContext db = new NhaXeDataContext();
        //Danh sách địa điểm
        public ActionResult DanhSachDD(string search)
        {
            var dd = from d in db.DiaDiems
                     select d;

            if (!string.IsNullOrEmpty(search))
            {
                dd = dd.Where(e => e.TEN_TINH_THANH.Contains(search));
            }

            return View(dd.ToList());
        }
        // GET: Thêm Địa Điểm
        public ActionResult ThemDD()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckSessionRole("")]
        public ActionResult ThemDD(DiaDiem id)
        {
            if (ModelState.IsValid)
            {

                var existingDiaDiem = db.DiaDiems.FirstOrDefault(dd => dd.TEN_TINH_THANH == id.TEN_TINH_THANH);

                if (existingDiaDiem != null)
                {
                    ModelState.AddModelError("TEN_TINH_THANH", "Địa điểm này đã tồn tại.");
                    return View(id);
                }

                db.DiaDiems.InsertOnSubmit(id);
                db.SubmitChanges();
                return RedirectToAction("DanhSachDD");
            }

            return View(id);
        }


        // Xóa Địa Điểm
        public ActionResult XoaDD(int id)
        {
            var diaDiem = db.DiaDiems.SingleOrDefault(dd => dd.ID_DIADIEM == id);
            if (diaDiem == null)
            {
                return HttpNotFound();
            }
            return View(diaDiem);
        }

        [HttpPost, ActionName("XoaDD")]
        [CheckSessionRole("")]
        public ActionResult XoaDDConfirmed(int id)
        {
            var diaDiem = db.DiaDiems.SingleOrDefault(dd => dd.ID_DIADIEM == id);
            if (diaDiem != null)
            {
                db.DiaDiems.DeleteOnSubmit(diaDiem);
                db.SubmitChanges();
                return RedirectToAction("DanhSachDD");
            }

            return HttpNotFound();
        }
        // Cập nhật địa điểm
        public ActionResult CapNhatDD(int id)
        {
            var diaDiem = db.DiaDiems.SingleOrDefault(d => d.ID_DIADIEM == id);
            if (diaDiem == null)
            {
                return HttpNotFound();
            }
            return View(diaDiem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckSessionRole("")]
        public ActionResult CapNhatDD(DiaDiem diaDiem)
        {
            if (ModelState.IsValid)
            {
                var existingDiaDiem = db.DiaDiems.SingleOrDefault(d => d.ID_DIADIEM == diaDiem.ID_DIADIEM);
                if (existingDiaDiem == null)
                {
                    ModelState.AddModelError("", "Địa điểm không tồn tại.");
                    return View(diaDiem);
                }
                existingDiaDiem.TEN_TINH_THANH = diaDiem.TEN_TINH_THANH;
                try
                {
                    db.SubmitChanges(); 
                    return RedirectToAction("DanhSachDD");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra trong quá trình cập nhật: " + ex.Message);
                }
            }
            return View(diaDiem);
        }
    }
}