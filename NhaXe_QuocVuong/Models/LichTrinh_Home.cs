using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NhaXe_QuocVuong.Models
{
    public class LichTrinh_Home
    {
        public string MaLichTrinh { get; set; }
        public string TenTuyenDuong { get; set; }
        public float GiaVe { get; set; }
        public DateTime NgayKhoiHanh { get; set; }
    }
}