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
            List<LichTrinhViewModel> model = new List<LichTrinhViewModel>();
            List< LichTrinh > lst = db.LichTrinhs.ToList();
            foreach(var item in lst)
            {
                LichTrinhViewModel ltvm = new LichTrinhViewModel
                {
                    LichTrinh = item,
                    SoChoConLai = LaySoChoCon(item.MA_LICH_TRINH)
                };
                model.Add(ltvm);
            }


            return View(model);
        }

        private int LaySoChoCon(string MaLichTrinh)
        {
            LichTrinh lt = db.LichTrinhs.FirstOrDefault(t => t.MA_LICH_TRINH.Equals(MaLichTrinh));
            return lt.Ghes.Count(t => t.TINH_TRANG == false);
        }
    }
}