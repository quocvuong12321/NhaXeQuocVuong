using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhaXe_QuocVuong.Models;
namespace NhaXe_QuocVuong.Areas.Area_Admin.Controllers
{
    public class NhanVienController : Controller
    {
        NhaXeDataContext db = new NhaXeDataContext("");

        public ActionResult DanhSachNV(string search)
        {
            var nv = from n in db.NHANVIENs
                     select n;

            if (!string.IsNullOrEmpty(search))
            {
                nv = nv.Where(e => e.TEN_NHANVIEN.Contains(search));
            }

            return View(nv.ToList());
        }
        //Xóa nhân viên
        public ActionResult XoaNV(string username)
        {
            var nhanVien = db.NHANVIENs.SingleOrDefault(n => n.USERNAME == username);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }

        [HttpPost, ActionName("XoaNV")]
        public ActionResult XoaNVConfirmed(string username)
        {
            var nhanVien = db.NHANVIENs.SingleOrDefault(n => n.USERNAME == username);
            if (nhanVien != null)
            {
                db.NHANVIENs.DeleteOnSubmit(nhanVien);
                var userAccount = db.userAccounts.SingleOrDefault(u => u.username == username);
                if (userAccount != null)
                {
                    db.userAccounts.DeleteOnSubmit(userAccount);
                }
                db.SubmitChanges();
                TempData["Message"] = "Nhân viên đã được xóa thành công.";
                return RedirectToAction("DanhSachNV");
            }
            else
            {
                TempData["Error"] = "Không thể xóa nhân viên, không tìm thấy nhân viên với tên đăng nhập này.";
                return RedirectToAction("DanhSachNV");
            }
        }

        //Thêm nhân viên
        public ActionResult ThemNV()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemNV(NHANVIEN model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = db.userAccounts.SingleOrDefault(u => u.username == model.USERNAME);
                    if (existingUser == null)
                    {
                        string password = model.USERNAME; 
                        string role = model.LOAI_NV == "QUAN_LY" || model.LOAI_NV == "TAI_XE" ? "nha_xe": "khach";
                        db.userAccounts.InsertOnSubmit(new userAccount
                        {
                            username = model.USERNAME,
                            password = password,  
                            role = role
                        });
                    }
                    db.NHANVIENs.InsertOnSubmit(model);
                    db.SubmitChanges(); 
                    return RedirectToAction("DanhSachNV"); 
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                }
            }
            return View(model); 
        }

        //Sửa thông tin nhân viên
        public ActionResult SuaNV(string username)
        {
            var nhanVien = db.NHANVIENs.SingleOrDefault(n => n.USERNAME == username);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaNV(NHANVIEN model)
        {
            if (ModelState.IsValid)
            {
                var nhanVien = db.NHANVIENs.SingleOrDefault(n => n.USERNAME == model.USERNAME);
                if (nhanVien != null)
                {
                    nhanVien.TEN_NHANVIEN = model.TEN_NHANVIEN;
                    nhanVien.EMAIL = model.EMAIL;
                    nhanVien.SDT = model.SDT;
                    nhanVien.LOAI_NV = model.LOAI_NV;

                    db.SubmitChanges();
                    return RedirectToAction("DanhSachNV");
                }
                return HttpNotFound();
            }
            return View(model);
        }
    }
}