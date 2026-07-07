using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL.Models
{
    [Table("HopDong")]
    public class HopDong
    {
        [Key]
        public int MaHD { get; set; }

        [Required, StringLength(50)]
        public string SoHopDong { get; set; }

        public int MaNV { get; set; }

        [Required, StringLength(100)]
        public string LoaiHopDong { get; set; }

        [DataType(DataType.Date)]
        public DateTime NgayBatDau { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgayKetThuc { get; set; }

        public decimal LuongHopDong { get; set; }

        [DataType(DataType.Date)]
        public DateTime NgayKy { get; set; }

        [StringLength(150)]
        public string NguoiKy { get; set; }

        public string NoiDung { get; set; }

        [Required, StringLength(50)]
        public string TrangThai { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
