using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NhaXe_QuocVuong.Models
{
    public class LichSuMuaVe
    {
        public string Id_Ve { get; set; }
        public DateTime NgayDat { get; set; }
        public float TongTien{ get; set; }
        public string TrangThai { get; set; }
    }
}