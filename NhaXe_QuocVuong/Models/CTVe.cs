using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NhaXe_QuocVuong.Models
{
    public class CTVe
    {
        public string TenKH { get; set; }
        public DateTime NgayDat { get; set; }
        public string TuyenDuong { get; set; }
        public List<GheDTO> DanhSachGhe { get; set; }
        public DateTime TgianKhoiHanh { get; set; }
        public float TongTien { get; set; }
        public string Qrcode { get; set; }
    }
}