using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NhaXe_QuocVuong.Models
{
    public class QuanLyTuyenDuong
    {

    }
    public class StopData
    {
        public string Diachi { get; set; }
        public string thoigianden { get; set; }
    }
    public class LichTrinhModelRequest
    {
        public string MA_LICH_TRINH {get; set; }
        public string ID_TUYEN_DUONG { get; set; }
        public string KHOI_HANH { get; set; }
        public string KET_THUC { get; set; }
        public string ID_XE { get; set; }
        public string TAIXE { get; set; }
        public string GIA_VE { get; set; }
    }
    public class LichTrinhRequest
    {
        public LichTrinhModelRequest formData { get; set; }
        public List<StopData> StopsData { get; set; }
    }



    public class LichTrinhViewModel
    {
        public string MaLichTrinh { get; set; }
        public string TG_KhoiHanh { get; set; }
        public string TG_KetThuc { get; set; }
        public string Gia { get; set; }
        public string ThoiGianDuyChuyen { get; set; }
        public string DiemKhoiHanh { get; set; }
        public string DiemKetThuc { get; set; }
        public string LoaiXe { get; set; }
        public string SoGhe { get; set; }
        public int SoGheTrong { get; set; }
        public string TenNhanVien {  get; set; }
        public string BienSoXe { get; set; }
    }

}