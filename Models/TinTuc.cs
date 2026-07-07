using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL.Models
{
    [Table("TinTuc")]
    public class TinTuc
    {
        [Key]
        public int MaTin { get; set; }

        [Required, StringLength(255)]
        public string TieuDe { get; set; }

        [StringLength(500)]
        public string TomTat { get; set; }

        public string NoiDung { get; set; }

        [StringLength(300)]
        public string HinhAnh { get; set; }

        [Required, StringLength(50)]
        public string LoaiTin { get; set; }

        public DateTime NgayDang { get; set; }

        [StringLength(100)]
        public string TacGia { get; set; }

        public bool NoiBat { get; set; }

        public bool IsDeleted { get; set; }
    }
}
