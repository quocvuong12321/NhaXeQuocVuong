using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NhaXe_QuocVuong.Areas.Area_Admin.Data
{
    public class ves
    {
        public string ID_VE { get; set; }
        public System.Nullable<System.DateTime> NGAY_DAT_VE { get; set; }
        public decimal TONG_TIEN { get; set; }
        public ves(string maDonHang, decimal tongTien, DateTime? ngayDat)
        {
            ID_VE = maDonHang;
            TONG_TIEN = tongTien;
            NGAY_DAT_VE = ngayDat;
        }
    }
}