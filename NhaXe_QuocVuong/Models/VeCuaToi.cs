using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NhaXe_QuocVuong.Models
{
    public class VeCuaToi
    {
        public string IdVe { get; set; }
        public DateTime NgayKhoiHanh { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string TenTuyen { get; set; }
        public string BienSoXe { get; set; }
        public string TrangThai { get; set; }
        public string DiemDon { get; set; }
        public string DiemTra { get; set; }
    }
}