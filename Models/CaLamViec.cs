using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL.Models
{
    [Table("CaLamViec")]
    public class CaLamViec
    {
        [Key]
        public int MaCa { get; set; }

        [Required, StringLength(100)]
        public string TenCa { get; set; }

        public TimeSpan GioBatDau { get; set; }

        public TimeSpan GioKetThuc { get; set; }

        [StringLength(255)]
        public string GhiChu { get; set; }

        public bool TrangThai { get; set; }
    }
}
