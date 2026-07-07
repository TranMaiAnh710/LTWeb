using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL.Models
{
    [Table("LienHe")]
    public class LienHe
    {
        [Key]
        public int MaLH { get; set; }

        [Required, StringLength(150)]
        [Display(Name = "Ho ten")]
        public string HoTen { get; set; }

        [Required, StringLength(150), EmailAddress]
        public string Email { get; set; }

        [StringLength(20)]
        [Display(Name = "So dien thoai")]
        public string SoDienThoai { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Tieu de")]
        public string TieuDe { get; set; }

        [Required]
        [Display(Name = "Noi dung")]
        public string NoiDung { get; set; }

        public DateTime NgayGui { get; set; }

        public bool DaXuLy { get; set; }
    }
}
