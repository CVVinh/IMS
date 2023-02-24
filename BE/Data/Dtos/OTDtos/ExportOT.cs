using BE.Data.Enum.OTEnums;
using BE.Data.Enum;

namespace BE.Data.Dtos.OTDtos
{
    public class ExportOT
    {
        public int id { get; set; }
        public Types HinhThucOT { get; set; }
        public DateTime NgayOT { get; set; }
        public string NgayBatDau { get; set; }
        public string NgayKetThuc { get; set; }
        public string ThoiGianThucTeOT { get; set; }
        public StatusOT TrangThai { get; set; }
        public string MoTa { get; set; }
        public int? TruongNhom { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? NguoiCapNhat { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public string? LyDoHuy { get; set; }
        public int NguoiOT { get; set; }
        public int TenDuAn { get; set; }
    }
}
